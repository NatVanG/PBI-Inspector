using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Output;
using System.Text.Json;

namespace PBIXInspectorAzureFunctions
{
    public static class Inspect
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportDefinitionTrigger"></param>
        /// <param name="rulesInput"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Function(nameof(Inspect))]
        [BlobOutput("pbi-inspector-output/{name}")]
        public static string Run(
            [BlobTrigger("pbi-report-definitions/{name}")] string reportDefinitionTrigger,
            [BlobInput("pbi-inspector-rules/Base-rules.json")] string rulesInput,
            FunctionContext context)
        {
            //TODO: Add support for multiple rules files
            var logger = context.GetLogger("Inspect");
            var inspector = new Inspector();
            inspector.MessageIssued += (sender, e) => logger.LogInformation(e.Message);

            var jt = JToken.Parse(reportDefinitionTrigger);
            var inspectionRules = Inspector.DeserialiseRulesFromString<InspectionRules>(rulesInput);
            var inspectionResults = inspector.Inspect(jt, inspectionRules);

            var testRun = new TestRun() { CompletionTime = DateTime.Now, TestedFilePath = "", RulesFilePath = "", Verbose = false, Results = inspectionResults };

            var jsonTestRun = JsonSerializer.Serialize(testRun);

            // Blob Output
            return jsonTestRun;
        }
    }
}