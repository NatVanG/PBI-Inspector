﻿using Json.Logic;
using Json.More;
using Json.Pointer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBIXInspectorLibrary.Output;
using System.Text;
using System.Text.Json.Nodes;

namespace PBIXInspectorLibrary
{
    /// <summary>
    /// Iterates through input rules and runs then against input PBIX files
    /// </summary>
    public class Inspector : InspectorBase
    {
        private const string SUBJPATHSTART = "{";
        private const string SUBJPATHEND = "}";
        private const string FILTEREXPRESSIONMARKER = "?";
        private const string JSONPOINTERSTART = "/";
        private const string CONTEXTARRAY = ".";
        internal const char DRILLCHAR = '>';

        private string? _pbiFilePath, _rulesFilePath;
        private InspectionRules? _inspectionRules;

        public event EventHandler<MessageIssuedEventArgs>? MessageIssued;

        public Inspector() : base()
        {
            AddCustomRulesToRegistry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbiFilePath"></param>
        /// <param name="inspectionRules"></param>
        public Inspector(string pbiFilePath, InspectionRules inspectionRules) : base(pbiFilePath, inspectionRules)
        {
            this._pbiFilePath = pbiFilePath;
            this._inspectionRules = inspectionRules;
            AddCustomRulesToRegistry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbiFilePath">Local PBI file path</param>
        /// <param name="rulesFilePath">Local rules json file path</param>
        public Inspector(string pbiFilePath, string rulesFilePath) : base(pbiFilePath, rulesFilePath)
        {
            this._pbiFilePath = pbiFilePath;
            this._rulesFilePath = rulesFilePath;

            try
            {
                var inspectionRules = this.DeserialiseRulesFromFilePath<InspectionRules>(rulesFilePath);

                if (inspectionRules == null || inspectionRules.PbiEntries == null || inspectionRules.PbiEntries.Count == 0)
                {
                    throw new PBIXInspectorException(string.Format("No rule definitions were found within rules file at \"{0}\".", rulesFilePath));
                }
                else
                {
                    this._inspectionRules = inspectionRules;
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new PBIXInspectorException(string.Format("Rules file with path \"{0}\" not found.", rulesFilePath), e);
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new PBIXInspectorException(string.Format("Could not deserialise rules file with path \"{0}\". Check that the file is valid json and following the correct schema for PBI Inspector rules.", rulesFilePath), e);
            }

            AddCustomRulesToRegistry();
        }

        /// <summary>
        /// Runs the inspection rules against the PBI file defined in the Inspector constructor
        /// </summary>
        public IEnumerable<TestResult> Inspect()
        {
            if (string.IsNullOrEmpty(_pbiFilePath)) throw new ArgumentNullException(nameof(_pbiFilePath));
            if (_inspectionRules == null) throw new ArgumentNullException(nameof(_inspectionRules));

            var testResults = new List<TestResult>();

            using (var pbiFile = PbiFileUtils.InitPbiFile(_pbiFilePath))
            {
                foreach (var entry in this._inspectionRules.PbiEntries)
                {
                    string pbiEntryPath;

                    switch (pbiFile.FileType)
                    {
                        case PbiFile.PBIFileTypeEnum.PBIX:
                            pbiEntryPath = entry.PbixEntryPath;
                            break;
                        case PbiFile.PBIFileTypeEnum.PBIPReport:
                            pbiEntryPath = entry.PbipEntryPath;
                            break;
                        default:
                            pbiEntryPath = null;
                            break;
                    }

                    if (string.IsNullOrEmpty(pbiEntryPath))
                    {
                        OnMessageIssued(MessageTypeEnum.Error, string.Format("Could not determine file type for entry \"{0}\", resuming to next entry.", entry.Name));
                        continue;
                    }
                    using (Stream entryStream = pbiFile.GetEntryStream(pbiEntryPath))
                    {
                        if (entryStream == null)
                        {
                            OnMessageIssued(MessageTypeEnum.Error, string.Format("PBI entry \"{0}\" with path \"{1}\" is not valid or does not exist, resuming to next PBI Entry iteration if any.", entry.Name, entry.PbixEntryPath));
                            continue;
                        }

                        //TODO: retrieve PBIP encoding from codepage to allow for different encoding for PBIX and PBIP
                        Encoding encoding = pbiFile.FileType == PbiFile.PBIFileTypeEnum.PBIX ? GetEncodingFromCodePage(entry.CodePage) : Encoding.UTF8;

                        using StreamReader sr = new(entryStream, encoding);

                        //TODO: use TryParse instead but better still use stringenumconverter upon deserialising
                        EntryContentTypeEnum contentType;
                        if (!Enum.TryParse(entry.ContentType, out contentType))
                        {
                            OnMessageIssued(MessageTypeEnum.Error, string.Format("ContentType \"{0}\" defined for entry \"{1}\" is not valid.", entry.ContentType, entry.Name));
                            continue;
                        }

                        switch (contentType)
                        {
                            case EntryContentTypeEnum.json:
                                {
                                    using JsonTextReader reader = new(sr);

                                    var jo = JToken.ReadFrom(reader);

                                    OnMessageIssued(MessageTypeEnum.Information, string.Format("Running rules for PBI entry \"{0}\"...", entry.Name));
                                    testResults.AddRange(Inspect(jo, entry));

                                    break;
                                }
                            case EntryContentTypeEnum.text:
                                {
                                    OnMessageIssued(MessageTypeEnum.Error, "PBI entries with \"text\" content type are not currently supported. Resuming to next entry.");
                                    continue;
                                }
                            default:
                                {
                                    OnMessageIssued(MessageTypeEnum.Error, "Only Json PBI entries are currently supported. Resuming to next entry");
                                    continue;
                                }
                        }
                    }
                }
            }

            return testResults;
        }


        /// <summary>
        /// Runs inspection rules against a PBI file. Only a PBIP report.json stream is currently supported as PBI file input.
        /// </summary>
        /// <param name="pbiInputStream"></param>
        /// <param name="rulesStream"></param>
        /// <param name="pbiInputFileType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PBIXInspectorException"></exception>
        public IEnumerable<TestResult> Inspect(Stream pbiInputStream, Stream rulesStream, PbiFile.PBIFileTypeEnum pbiInputFileType = PbiFile.PBIFileTypeEnum.PBIPReport)
        {
            if (pbiInputFileType != PbiFile.PBIFileTypeEnum.PBIPReport) throw new NotImplementedException("Only PBIPReport file type is currently supported by this method.");
            if (pbiInputStream == null) throw new ArgumentNullException(nameof(pbiInputStream));
            if (rulesStream == null) throw new ArgumentNullException(nameof(rulesStream));

            IEnumerable<TestResult> inspectionResults;

            using (var reader = new StreamReader(pbiInputStream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var jt = JToken.Load(jsonReader);

                try
                {
                    var inspectionRules = Inspector.DeserialiseRules<InspectionRules>(rulesStream);
                    inspectionResults = this.Inspect(jt, inspectionRules);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new PBIXInspectorException("An error occurred during inspection", ex);

                }
                return inspectionResults;
            }
        }

        protected void OnMessageIssued(MessageTypeEnum messageType, string message)
        {
            var args = new MessageIssuedEventArgs(message, messageType);
            OnMessageIssued(args);
        }

        protected virtual void OnMessageIssued(MessageIssuedEventArgs e)
        {
            EventHandler<MessageIssuedEventArgs>? handler = MessageIssued;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #region private methods

        private void AddCustomRulesToRegistry()
        {
            //TODO: Use reflection to add rules
            /*
            System.Reflection.Assembly assembly = Assembly.GetExecutingAssembly();
            string nspace = "PBIXInspectorLibrary.CustomRules";

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == nspace
                    select t;
            q.ToList().ForEach(t1 => Json.Logic.RuleRegistry.AddRule<t1>());
            */

            Json.Logic.RuleRegistry.AddRule<CustomRules.IsNullOrEmptyRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.CountRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.StringContains>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.ToString>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.ToRecordRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.DrillVariableRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.RectOverlapRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.SetIntersectionRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.SetUnionRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.SetDifferenceRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.SetSymmetricDifferenceRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.SetEqualRule>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private IEnumerable<TestResult> Inspect(JToken? jo, InspectionRules? rules)
        {
            if (jo == null) throw new ArgumentNullException(nameof(jo));
            if (rules == null) throw new ArgumentNullException(nameof(rules));

            var testResults = new List<TestResult>();

            foreach (var entry in rules.PbiEntries)
            {
                testResults.AddRange(Inspect(jo, entry));
            }

            return testResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private IEnumerable<TestResult> Inspect(JToken? jo, PbiEntry? entry)
        {
            if (jo == null) throw new ArgumentNullException(nameof(jo));
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            var testResults = new List<TestResult>();

            if (entry.ContentType.ToLower() != "json")
            {
                OnMessageIssued(MessageTypeEnum.Error, string.Format("PBI entry \"{0}\" with content type \"{1}\" is not supported, resuming to next entry.", entry.Name, entry.ContentType));
                return testResults;
            }

            OnMessageIssued(MessageTypeEnum.Information, string.Format("Running rules for PBI entry \"{0}\"...", entry.Name));
            foreach (var rule in entry.EnabledRules)
            {
                OnMessageIssued(MessageTypeEnum.Information, string.Format("Running Rule \"{0}\".", rule.Name));
                Json.Logic.Rule? jrule = null;

                try
                {
                    jrule = System.Text.Json.JsonSerializer.Deserialize<Json.Logic.Rule>(rule.Test.Logic);
                }
                catch (System.Text.Json.JsonException e)
                {
                    OnMessageIssued(MessageTypeEnum.Error, string.Format("Parsing of logic for rule \"{0}\" failed, resuming to next rule.", rule.Name));
                    continue;
                }

                //Check if there's a foreach iterator
                if (rule != null && !string.IsNullOrEmpty(rule.ForEachPath))
                {
                    var forEachTokens = ExecuteTokensPath(jo, rule.Name, rule.ForEachPath, rule.PathErrorWhenNoMatch);

                    foreach (var forEachToken in forEachTokens)
                    {

                        var forEachName = !string.IsNullOrEmpty(rule.ForEachPathName) ? ExecuteTokensPath((JObject?)forEachToken, rule.Name, rule.ForEachPathName, rule.PathErrorWhenNoMatch) : null;
                        var strForEachName = forEachName != null ? forEachName[0].ToString() : string.Empty;

                        var forEachDisplayName = !string.IsNullOrEmpty(rule.ForEachPathDisplayName) ? ExecuteTokensPath((JObject?)forEachToken, rule.Name, rule.ForEachPathDisplayName, rule.PathErrorWhenNoMatch) : null;
                        var strForEachDisplayName = forEachDisplayName != null ? forEachDisplayName[0].ToString() : string.Empty;

                        try
                        {
                            var tokens = ExecuteTokensPath(forEachToken, rule.Name, rule.Path, rule.PathErrorWhenNoMatch);

                            //HACK
                            var contextNodeArray = ConvertToJsonArray(tokens);

                            bool result = false;

                            //HACK: checking if the rule's intention is to return an array or a single object 
                            var node = rule.Path.Contains("*") || rule.Path.Contains("?") ? contextNodeArray : (contextNodeArray != null ? contextNodeArray.FirstOrDefault() : null);
                            var newdata = MapRuleDataPointersToValues(node, rule, contextNodeArray);

                            //TODO: the following commented line does not work with the variableRule implementation with context array passed in.
                            //var jruleresult = jrule.Apply(newdata, contextNodeArray);
                            var jruleresult = jrule.Apply(newdata);
                            result = rule.Test.Expected.IsEquivalentTo(jruleresult);
                            var ruleLogType = ConvertRuleLogType(rule.LogType);
                            string resultString = string.Concat("\"", strForEachDisplayName, "\" - ", string.Format("Rule \"{0}\" {1} with result: {2}, expected: {3}.", rule != null ? rule.Name : string.Empty, result ? "PASSED" : "FAILED", jruleresult != null ? jruleresult.ToString() : string.Empty, rule.Test.Expected != null ? rule.Test.Expected.ToString() : string.Empty));

                            //yield return new TestResult { RuleName = rule.Name, ParentName = strForEachName, ParentDisplayName = strForEachDisplayName, Pass = result, Message = resultString, Expected = rule.Test.Expected, Actual = jruleresult};
                            testResults.Add(new TestResult { RuleId = rule.Id, RuleName = rule.Name, LogType = ruleLogType, RuleDescription = rule.Description, ParentName = strForEachName, ParentDisplayName = strForEachDisplayName, Pass = result, Message = resultString, Expected = rule.Test.Expected, Actual = jruleresult });
                        }
                        catch (PBIXInspectorException e)
                        {
                            testResults.Add(new TestResult { RuleId = rule.Id, RuleName = rule.Name, LogType = MessageTypeEnum.Error, RuleDescription = rule.Description, ParentName = strForEachName, ParentDisplayName = strForEachDisplayName, Pass = false, Message = e.Message, Expected = rule.Test.Expected, Actual = null });
                            continue;
                        }
                    }
                }
                else
                {   //TODO: refactor else branch to reuse code from true branch
                    var tokens = ExecuteTokensPath(jo, rule.Name, rule.Path, rule.PathErrorWhenNoMatch);

                    //HACK
                    var contextNodeArray = ConvertToJsonArray(tokens);

                    bool result = false;

                    try
                    {
                        //HACK: checking if the rule's intention is to return an array or a single object 
                        var node = rule.Path.Contains("*") || rule.Path.Contains("?") ? contextNodeArray : (contextNodeArray != null ? contextNodeArray.FirstOrDefault() : null);
                        var newdata = MapRuleDataPointersToValues(node, rule, contextNodeArray);

                        //TODO: the following commented line does not work with the variableRule implementation with context array passed in.
                        //var jruleresult = jrule.Apply(newdata, contextNodeArray);
                        var jruleresult = jrule.Apply(newdata);
                        result = rule.Test.Expected.IsEquivalentTo(jruleresult);
                        var ruleLogType = ConvertRuleLogType(rule.LogType);
                        string resultString = string.Format("Rule \"{0}\" {1} with result: {2}, expected: {3}.", rule != null ? rule.Name : string.Empty, result ? "PASSED" : "FAILED", jruleresult != null ? jruleresult.ToString() : string.Empty, rule.Test.Expected != null ? rule.Test.Expected.ToString() : string.Empty);

                        //yield return new TestResult { RuleName = rule.Name, Pass = result, Message = resultString, Expected = rule.Test.Expected, Actual = jruleresult };
                        testResults.Add(new TestResult { RuleId = rule.Id, RuleName = rule.Name, LogType = ruleLogType, RuleDescription = rule.Description, Pass = result, Message = resultString, Expected = rule.Test.Expected, Actual = jruleresult });
                    }
                    catch (PBIXInspectorException e)
                    {
                        testResults.Add(new TestResult { RuleId = rule.Id, RuleName = rule.Name, LogType = MessageTypeEnum.Error, RuleDescription = rule.Description, Pass = false, Message = e.Message, Expected = rule.Test.Expected, Actual = null });
                        continue;
                    }
                }
            }

            return testResults;
        }

        private MessageTypeEnum ConvertRuleLogType(string ruleLogType)
                {
                    if (string.IsNullOrEmpty(ruleLogType)) return MessageTypeEnum.Warning;

                    MessageTypeEnum logType;

                    switch (ruleLogType.ToLower().Trim())
                    {
                        case "error":
                            logType = MessageTypeEnum.Error;
                            break;
                        case "warning":
                            logType = MessageTypeEnum.Warning;
                            break;
                        default:
                            logType = MessageTypeEnum.Warning;
                            break;
                    }

                    return logType;
                }

        private JsonArray ConvertToJsonArray(List<JToken>? tokens)
        {
            List<JsonNode>? nodes = new();

            if (tokens != null)
            {
                foreach (var token in tokens)
                {
                    JsonNode? node;

                    try
                    {
                        node = JsonNode.Parse(token.ToString());
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        node = token.ToString();
                    }

                    if (node != null) nodes.Add(node);
                }
            }

            return new JsonArray(nodes.ToArray());
        }

        /// <summary>
        /// Execute JPaths and sub JPaths
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private List<JToken>? ExecuteTokensPath(JToken jo, string ruleName, string rulePath, bool rulePathErrorWhenNoMatch)
        {
            string parentPath, queryPath = string.Empty;
            List<JToken>? tokens = new List<JToken>();

            //TODO: a regex match to extract the substring would be better
            if (rulePath.Contains(SUBJPATHSTART)) //check for subpath syntax
            {
                if (rulePath.EndsWith(SUBJPATHEND))
                {
                    //TODO: currently subpath is assumed to be the last path (i.e. the whole string end in "})" but we should be able to resolve inner subpath and return to parent path
                    var index = rulePath.IndexOf(SUBJPATHSTART);
                    parentPath = rulePath.Substring(0, index);
                    queryPath = rulePath.Substring(index + SUBJPATHSTART.Length, rulePath.Length - (index + SUBJPATHSTART.Length) - 1);
                    var parentTokens = SelectTokens(jo, ruleName, parentPath, rulePathErrorWhenNoMatch);

                    if (parentTokens == null || parentTokens.Count() == 0) { return tokens; }

                    if (parentPath.Contains(FILTEREXPRESSIONMARKER))
                    {
                        JArray ja = new JArray();
                        foreach (var t in parentTokens)
                        {
                            //HACK: why do I have to parse a token into a token to make the subsequent SelectTokens call work?
                            var jt = JToken.Parse(t.ToString());
                            ja.Add(jt);
                        }

                        tokens = ja.SelectTokens(queryPath, rulePathErrorWhenNoMatch)?.ToList();
                    }
                    else
                    {
                        foreach (var t in parentTokens)
                        {
                            //var childtokens = SelectTokens((JObject?)t, rule.Name, childPath, rule.PathErrorWhenNoMatch); //TODO: this seems better but throws InvalidCastException
                            var childtokens = SelectTokens(((JObject)JToken.Parse(t.ToString())), ruleName, queryPath, rulePathErrorWhenNoMatch);
                            //only return children tokens, the reference to parent tokens is lost. 
                            if (childtokens != null) tokens.AddRange(childtokens.ToList());
                        }
                    }
                }
                else
                {
                    throw new PBIXInspectorException(string.Format("Path \"{0}\" needs to end with \"{1}\" as it contains a subpath.", rulePath, "}"));
                }
            }
            else
            {
                tokens = SelectTokens(jo, ruleName, rulePath, rulePathErrorWhenNoMatch)?.ToList();
            }

            return tokens;
        }

        private IEnumerable<JToken>? SelectTokens(JToken? jo, string ruleName, string rulePath, bool rulePathErrorWhenNoMatch)
        {
            IEnumerable<JToken>? tokens;

            //TODO: for some reason I can't catch Newtonsoft.Json.JsonException when rule.PathErrorWhenNoMatch is true
            tokens = jo.SelectTokens(rulePath, false);


            if (tokens == null || tokens.Count() == 0)
            {
                var msgType = rulePathErrorWhenNoMatch ? MessageTypeEnum.Error : MessageTypeEnum.Information;
                OnMessageIssued(msgType, string.Format("Rule \"{0}\" with JPath \"{1}\" did not return any tokens.", ruleName, rulePath));
            }

            return tokens;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private JsonNode? MapRuleDataPointersToValues(JsonNode? target, Rule rule, JsonArray? contextNodeArray)
        {
            if (rule.Test.Data == null || rule.Test.Data is not JsonObject) return rule.Test.Data;

            var newdata = new JsonObject();

            var olddata = rule.Test.Data.AsObject();

            if (target != null)
            {
                try
                {
                    if (olddata != null && olddata.Count() > 0)
                    {
                        foreach (var item in olddata)
                        {
                            if (item.Value is JsonValue)
                            {
                                var value = item.Value.AsValue().Stringify();
                                if (value.StartsWith(JSONPOINTERSTART)) //check for JsonPointer syntax
                                {
                                    try
                                    {
                                        //var pointer = JsonPointer.Parse(value);
                                        //var evalsuccess = pointer.TryEvaluate(target, out var newval);
                                        var evalsuccess = EvalPath(value, target, out var newval);
                                        if (evalsuccess)
                                        {
                                            if (newval != null)
                                            {
                                                newdata.Add(new KeyValuePair<string, JsonNode?>(item.Key, newval.Copy()));
                                            }
                                            else
                                            {
                                                //TODO: handle null value?
                                            }
                                        }
                                        else
                                        {
                                            if (rule.PathErrorWhenNoMatch)
                                            {
                                                throw new PBIXInspectorException(string.Format("Rule \"{0}\" - Could not evaluate json pointer \"{1}\".", rule.Name, value));
                                            }
                                            else
                                            {
                                                OnMessageIssued(MessageTypeEnum.Information, string.Format("Rule \"{0}\" - Could not evaluate json pointer \"{1}\".", rule.Name, value));
                                                continue;
                                            }
                                        }
                                    }
                                    catch (PointerParseException e)
                                    {
                                        if (rule.PathErrorWhenNoMatch)
                                        {
                                            throw new PBIXInspectorException(string.Format("Rule \"{0}\" - Pointer parse exception for value \"{1}\".", rule.Name, value));
                                        }
                                        else
                                        {
                                            OnMessageIssued(MessageTypeEnum.Error, string.Format("Rule \"{0}\" - Pointer parse exception for value \"{1}\". Inner Exception: \"{2}\".", rule.Name, value, e.Message));
                                            continue;
                                        }
                                    }
                                }
                                else if (value.Equals(CONTEXTARRAY))
                                {
                                    //context array token was used so pass in the parent array
                                    newdata.Add(new KeyValuePair<string, JsonNode?>(item.Key, contextNodeArray.Copy()));
                                }
                                else
                                {
                                    //looks like a literal value
                                    newdata.Add(new KeyValuePair<string, JsonNode?>(item.Key, item.Value.Copy()));
                                }
                            }
                            else
                            {
                                //might be a JsonArray
                                newdata.Add(new KeyValuePair<string, JsonNode?>(item.Key, item.Value.Copy()));
                            }
                        }
                    }
                }
                catch (System.Text.Json.JsonException e)
                {
                    throw new PBIXInspectorException("JsonException", e);
                }

            }

            return newdata;
        }

        private bool EvalPath(string pathString, JsonNode? data, out JsonNode? result)
        {
            if (pathString.Contains(DRILLCHAR))
            {
                var leftString = pathString.Substring(0, pathString.IndexOf(DRILLCHAR));
                var rightString = pathString.Substring(pathString.IndexOf(DRILLCHAR) + 1);

                var leftStringPath = string.Concat(leftString.StartsWith(JSONPOINTERSTART) ? string.Empty : JSONPOINTERSTART, leftString.Replace('.', '/'));
                var pointer = JsonPointer.Parse(leftStringPath);
                if (pointer.TryEvaluate(data, out result))
                {
                    if (result is JsonValue val)
                    {
                        //remove single quotes from beginning and end of string if any.
                        string strVal;
                        if (val.ToString()!.StartsWith("'") && val.ToString()!.EndsWith("'"))
                        {
                            strVal = val.ToString()!.Substring(1, val.ToString()!.Length - 2);
                        }
                        else
                        {
                            strVal = val.ToString()!;
                        }

                        var pathEvalNode = JsonNode.Parse(strVal);
                        return EvalPath(rightString, pathEvalNode, out result);
                    }
                    else
                    {
                        return EvalPath(rightString, result, out result);
                    }
                }
            }
            else if (pathString.Trim().Equals(CONTEXTARRAY))
            {
                result = data;
                return true;
            }
            else
            {
                var pathStringPath = string.Concat(pathString.StartsWith(JSONPOINTERSTART) ? string.Empty : JSONPOINTERSTART, pathString.Replace('.', '/'));
                var pointer = JsonPointer.Parse(pathStringPath);
                return pointer.TryEvaluate(data, out result);
            }

            result = null;
            return false;
        }

        private Encoding GetEncodingFromCodePage(int codePage)
        {
            var enc = Encoding.Unicode;

            try
            {
                enc = Encoding.GetEncoding(codePage);
            }
            catch
            {
                OnMessageIssued(MessageTypeEnum.Warning, string.Format("Encoding code page value {0} for PBIentry is not valid, defaulting to {1}.", codePage, enc.EncodingName));
            }

            return enc;
        }

        #endregion
    }
}
