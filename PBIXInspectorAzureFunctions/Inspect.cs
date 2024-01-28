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
        [Function(nameof(Inspect))]
        [BlobOutput("pbi-inspector-output/{name}")]
        public static string Run(
            [BlobTrigger("report-definitions/{name}")] string reportDefinitionTrigger,
            [BlobInput("rules-input/Base-rules.json")] string rulesInput,
            FunctionContext context)
        {
            var logger = context.GetLogger("Inspect");
            //logger.LogInformation("Triggered Item = {reportDefinitionTrigger}", reportDefinitionTrigger);

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