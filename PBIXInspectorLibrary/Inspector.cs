using Json.Logic;
using Json.More;
using Json.Pointer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
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
        private const string JSONPOINTERSTART = "/";
        private const string CONTEXTARRAY = ".";

        private PbixFile _pbixFile;
        private InspectionRules? _inspectionRules;

        public event EventHandler<MessageIssuedEventArgs>? MessageIssued;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbixFilePath"></param>
        /// <param name="inspectionRules"></param>
        public Inspector(string pbixFilePath, InspectionRules inspectionRules) : base(pbixFilePath, inspectionRules)
        {
            using var pbixFile = new PbixFile(pbixFilePath);
            this._pbixFile = pbixFile;
            this._inspectionRules = inspectionRules;
            AddCustomRulesToRegistry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pbixFilePath">Local PBIX file path</param>
        /// <param name="rulesFilePath">Local rules json file path</param>
        public Inspector(string pbixFilePath, string rulesFilePath) : base(pbixFilePath, rulesFilePath)
        {
            using var pbixFile = new PbixFile(pbixFilePath);
            this._pbixFile = pbixFile;

            try
            {
                this._inspectionRules = this.DeserialiseRules<InspectionRules>(rulesFilePath);
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new PBIXInspectorException(string.Format("Rules file with path \"{0}\" not found.", rulesFilePath), e);
            }

            AddCustomRulesToRegistry();
        }

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

            Json.Logic.RuleRegistry.AddRule<CustomRules.CountRule>();
        }

        /// <summary>
        /// Core method
        /// </summary>
        public IEnumerable<TestResult> Inspect()
        {
            if (this._pbixFile == null || this._inspectionRules == null)
            {
                OnMessageIssued(MessageTypeEnum.Error, string.Format("Cannot start inpesction as either the PBIX file or the Inspection Rules are not instantiated."));
            }
            else
            {
                using ZipArchive za = this._pbixFile.Archive;

                foreach (var entry in this._inspectionRules.PbixEntries)
                {

                    var zae = za.GetEntry(entry.Path);

                    if (zae == null)
                    {
                        OnMessageIssued(MessageTypeEnum.Error, string.Format("PBIX entry \"{0}\" with path \"{1}\" is not valid or does not exist, resuming to next PBIX Entry iteration if any.", entry.Name, entry.Path));
                        continue;
                    }

                    using Stream entryStream = zae.Open();

                    Encoding encoding = GetEncodingFromCodePage(entry.CodePage);

                    using StreamReader sr = new(entryStream, encoding);

                    //TODO: use TryParse instead but better still use stringenumconverter upon deserialising
                    EntryContentTypeEnum contentType;
                    if (!Enum.TryParse(entry.ContentType, out contentType))
                    {
                        throw new PBIXInspectorException(string.Format("ContentType \"{0}\" defined for entry \"{1}\" is not valid.", entry.ContentType, entry.Name));
                    }

                    switch (contentType)
                    {
                        case EntryContentTypeEnum.json:
                            {
                                using JsonTextReader reader = new(sr);

                                var jo = (JObject)JToken.ReadFrom(reader);

                                OnMessageIssued(MessageTypeEnum.Information, string.Format("Running rules for PBIX entry \"{0}\"...", entry.Name));
                                foreach (var rule in entry.Rules)
                                {
                                    Json.Logic.Rule? jrule = null;

                                    try
                                    {
                                        jrule = System.Text.Json.JsonSerializer.Deserialize<Json.Logic.Rule>(rule.Test.Logic);
                                    }
                                    catch (System.Text.Json.JsonException e)
                                    {
                                        throw new PBIXInspectorException(string.Format("Parsing of logic for rule \"{0}\" failed.", rule.Name), e);
                                    }

                                    var tokens = ExecutePath(jo, rule);

                                    //HACK: I don't like it.
                                    var contextNodeArray = ConvertToJsonArray(tokens);

                                    foreach (var node in contextNodeArray)
                                    {
                                        bool result = false;

                                        var newdata = MapRuleDataPointersToValues(node, rule, contextNodeArray);

                                        //TODO: the following commented line does not work with the variableRule implementation with context array passed in.
                                        //var jruleresult = jrule.Apply(newdata, contextNodeArray);
                                        var jruleresult = jrule.Apply(newdata);
                                        result = rule.Test.Expected.IsEquivalentTo(jruleresult);
                                        string resultString = string.Format("Rule \"{0}\" {1} with result: {2} and data: {3}.", rule != null ? rule.Name : string.Empty, result ? "PASSED" : "FAILED", jruleresult != null ? jruleresult.ToString() : string.Empty, newdata.AsJsonString().Length > 100 ? string.Concat(newdata.AsJsonString().Substring(0, 99), "[first 100 characters]") : newdata.AsJsonString());
                                        //TODO: return jruleresult in TestResult so that we can compose test from other tests.
                                        yield return new TestResult { Name = rule.Name, Result = result, ResultMessage = resultString };
                                    }
                                }
                                break;
                            }
                        case EntryContentTypeEnum.text:
                            {
                                throw new PBIXInspectorException("PBIX entries with text content are not currently supported.");
                            }
                        default:
                            {
                                throw new PBIXInspectorException("Only Json PBIX entries are supported.");
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Validate json object
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="rule"></param>
        /*
        public IEnumerable<TestResult> Validate(JObject? jo, Rule rule)
        {
            Json.Logic.Rule? jrule = null;

            try
            {
                jrule = System.Text.Json.JsonSerializer.Deserialize<Json.Logic.Rule>(rule.Test.Logic);
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new PBIXInspectorException(string.Format("Parsing of logic for rule \"{0}\" failed.", rule.Name), e);
            }

            var tokens = ExecutePath(jo, rule);

            //HACK: I don't like it.
            var contextNodeArray = ConvertToJsonArray(tokens);

            foreach (var node in contextNodeArray)
            {
                bool result = false;

                var newdata = MapRuleDataPointersToValues(node, rule);
                
                var jruleresult = jrule.Apply(newdata, contextNodeArray);
                result = rule.Test.Expected.IsEquivalentTo(jruleresult);
                string resultString = string.Format("Rule \"{0}\" {1} with data: {2}.", rule.Name, result ? "PASSED" : "FAILED", newdata.AsJsonString().Length > 100 ? string.Concat(newdata.AsJsonString().Substring(0, 99), "[first 100 characters]") : newdata.AsJsonString());
                OnMessageIssued(MessageTypeEnum.Information,  resultString);
                yield return new TestResult {  Name = rule.Name, Result= result, ResultMessage = resultString };
            }
        } */



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
        private List<JToken>? ExecutePath(JObject? jo, Rule rule)
        {
            string parentPath, childPath = string.Empty;
            List<JToken>? tokens = new List<JToken>();

            //TODO: a regex match would be better
            if (rule.Path.Contains(SUBJPATHSTART)) //check for subpath syntax
            {
                if (rule.Path.EndsWith("}"))
                {
                    //TODO: currently subpath is assumed to be the last path (i.e. the whole string end in "})" but we should be able to resolve inner subpath and return to parent path
                    var index = rule.Path.IndexOf(SUBJPATHSTART);
                    parentPath = rule.Path.Substring(0, index);
                    childPath = rule.Path.Substring(index + 1, rule.Path.Length - index - 2);
                    var parentTokens = SelectTokens(jo, rule.Name, parentPath, rule.PathErrorWhenNoMatch);

                    foreach (var t in parentTokens)
                    {
                        // childtokens = SelectTokens((JObject)t, rule.Name, childPath, rule.PathErrorWhenNoMatch); //TODO: this seems better but throws InvalidCastException
                        var childtokens = SelectTokens(((JObject)JToken.Parse(t.ToString())), rule.Name, childPath, rule.PathErrorWhenNoMatch);
                        if (childtokens != null) tokens.AddRange(childtokens.ToList());
                    }
                }
                else
                {
                    throw new PBIXInspectorException(string.Format("Path \"{0}\" needs to end with \"{1}\" as it contains a subpath.", rule.Path, "}"));
                }
            }
            else
            {
                tokens = SelectTokens(jo, rule)?.ToList();
            }


            return tokens;
        }

        private IEnumerable<JToken>? SelectTokens(JObject? jo, Rule rule)
        {
            return SelectTokens(jo, rule.Name, rule.Path, rule.PathErrorWhenNoMatch);
        }

        private IEnumerable<JToken>? SelectTokens(JObject? jo, string ruleName, string rulePath, bool pathErrorWhenNoMatch)
        {
            IEnumerable<JToken>? tokens;

            //Faster without a Try catch block hence the conditional branching
            if (!pathErrorWhenNoMatch)
            {
                tokens = jo.SelectTokens(rulePath, false);
            }
            else
            {
                //TODO: for some reason I can't catch Newtonsoft.Json.JsonException when rule.PathErrorWhenNoMatch is true
                tokens = jo.SelectTokens(rulePath, false);
                if (tokens == null || tokens.Count() == 0)
                {
                    OnMessageIssued(MessageTypeEnum.Information, string.Format("Rule \"{0}\" with JPath \"{1}\" did not return any tokens.", ruleName, rulePath));
                }

                //try
                //{
                //    tokens = jo.SelectTokens(path, true);
                //}
                //catch (Newtonsoft.Json.JsonException e)
                //{
                //    Console.WriteLine("ERROR: {0}", e.Message);
                //}
                //catch
                //{
                //    Console.WriteLine("ERROR: Path \"{0}\" not found.");
                //}
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
                                        var pointer = JsonPointer.Parse(value);
                                        var evalsuccess = pointer.TryEvaluate(target, out var newval);
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
                                                throw new PBIXInspectorException(string.Format("Rule \"{0}\" - Could not evaluate json pointer \"{1}\"", rule.Name, value));
                                            }
                                        }
                                    }
                                    catch (PointerParseException e)
                                    {
                                        throw new PBIXInspectorException(string.Format("Rule \"{0}\" - Pointer exception for value \"{1}\"", rule.Name, value), e);
                                    }
                                }
                                else if (value.StartsWith(CONTEXTARRAY))
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


        private Encoding GetEncodingFromCodePage(int codePage)
        {
            var enc = Encoding.Unicode;

            try
            {
                enc = Encoding.GetEncoding(codePage);
            }
            catch
            {
                OnMessageIssued(MessageTypeEnum.Information, string.Format("Encoding code page value {0} for PBIX entry is not valid, defaulting to {1}.", codePage, enc.EncodingName));
            }

            return enc;
        }

        protected void OnMessageIssued(MessageTypeEnum messageType, string message)
        {
            var args = new MessageIssuedEventArgs { MessageType = messageType, Message = message };
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
    }
}
