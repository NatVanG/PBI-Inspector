using Json.Logic;
using Json.More;
using Json.Path;
using Json.Pointer;
using PBIXInspectorLibrary.Exceptions;
using PBIXInspectorLibrary.Output;
using PBIXInspectorLibrary.Part;
using System.Data;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;

namespace PBIXInspectorLibrary
{
    /// <summary>
    /// Iterates through input rules and runs then against input PBIX files
    /// </summary>
    public class Inspector : InspectorBase
    {
        private const string JSONPOINTERSTART = "/";
        private const string CONTEXTNODE = ".";
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

                if (inspectionRules == null || inspectionRules.Rules == null || inspectionRules.Rules.Count == 0)
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

        public List<TestResult> Inspect()
        {
            var testResults = new List<TestResult>();
            var rules = this._inspectionRules.Rules.Where(_ => !_.Disabled);


            //TODO: determine which flavour of IPBIPartQuery to instantiate.
            IPBIPartQuery partQuery = new PBIRPartQuery(_pbiFilePath);
            ContextService.GetInstance().PartQuery = partQuery;
            

            foreach (var rule in rules)
            {
                ContextService.GetInstance().Part = partQuery.RootPart;

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

                bool result = false;
                
                try
                {
                   var parts = new List<Part.Part>();

                    if (!string.IsNullOrEmpty(rule.Part))
                    {
                        var part = partQuery.Invoke(rule.Part, ContextService.GetInstance().Part);

                        if (part is List<Part.Part>)
                        {
                            parts.AddRange((List<Part.Part>)part);
                        }
                        else
                        {
                            parts.Add((Part.Part)part);
                        }
                    }
                    else
                    {
                        parts.Add(partQuery.RootPart);
                    }

                    foreach (var part in parts)
                    {
                        ContextService.GetInstance().Part = part;
                        var node = partQuery.ToJsonNode(part);
                        var newdata = MapRuleDataPointersToValues(node, rule, node);

                        var parentName = partQuery.PartName(part);
                        var parentDisplayName = partQuery.PartDisplayName(part) ?? partQuery.PartName(part);

                        var jruleresult = jrule.Apply(newdata);
                        result = rule.Test.Expected.IsEquivalentTo(jruleresult);
                        var ruleLogType = ConvertRuleLogType(rule.LogType);
                        string resultString = string.Format("Rule \"{0}\" {1} with result: {2}, expected: {3}.", rule != null ? rule.Name : string.Empty, result ? "PASSED" : "FAILED", jruleresult != null ? jruleresult.ToString() : string.Empty, rule.Test.Expected != null ? rule.Test.Expected.ToString() : string.Empty);
                        testResults.Add(new TestResult { RuleId = rule.Id, RuleName = rule.Name, LogType = ruleLogType, RuleDescription = rule.Description, ParentName = parentName, ParentDisplayName = parentDisplayName, Pass = result, Message = resultString, Expected = rule.Test.Expected, Actual = jruleresult });
                     }
                    
                }
                catch (PBIXInspectorException e)
                {
                    testResults.Add(new TestResult { RuleId = rule.Id, RuleName = rule.Name, LogType = MessageTypeEnum.Error, RuleDescription = rule.Description, Pass = false, Message = e.Message, Expected = rule.Test.Expected, Actual = null });
                    continue;
                }
                
            }
            return testResults;
        }

        private string TryGetJsonNodeStringValue(JsonNode node, string query)
        {
            JsonPointer pt = JsonPointer.Parse(query);

            if (pt.TryEvaluate(node, out var result))
            {
                if (result is JsonValue val)
                {
                    return val.ToString();
                }
            }

            return null;
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
            Json.Logic.RuleRegistry.AddRule<CustomRules.PartRule>();
            Json.Logic.RuleRegistry.AddRule<CustomRules.QueryRule>();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private JsonNode? MapRuleDataPointersToValues(JsonNode? target, Rule rule, JsonNode? contextNode)
        {
            if (target == null || rule.Test.Data == null || rule.Test.Data is not JsonObject) return rule.Test.Data;

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
                                else if (value.Equals(CONTEXTNODE))
                                {
                                    //context array token was used so pass in the parent array
                                    newdata.Add(new KeyValuePair<string, JsonNode?>(item.Key, contextNode.Copy()));
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
            else if (pathString.Trim().Equals(CONTEXTNODE))
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

        #endregion
    }
}
