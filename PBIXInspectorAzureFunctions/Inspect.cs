using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Exceptions;
using PBIXInspectorLibrary.Output;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;

namespace PBIXInspectorAzureFunctions
{
    public static class Inspect
    {
        /// <summary>
        /// Azure function that inspect a report layout json files for compliance with the rules defined in the supplied Rules.json file
        /// </summary>
        /// <param name="reportDefinitionTrigger">The Azure function blob trigger</param>
        /// <param name="rulesInput">The rules file</param>
        /// <param name="context">The Azure Function context e.g. to retrieve the logger instance</param>
        /// <returns>A json string with the test results</returns>
        /// TODO: Add support for multiple rules files
        [Function(nameof(Inspect))]
        [BlobOutput("pbi-inspector-output/{name}")]
        public static string Run(
            [BlobTrigger("pbi-report-definitions/{name}")] BlobClient reportDefinitionTriggerClient,
            [BlobInput("pbi-inspector-rules/Rules.json")] Stream rulesInputStream,
            FunctionContext context)
        {
            var logger = context.GetLogger("Inspect");

            var inspector = new Inspector();
            inspector.MessageIssued += (sender, e) => logger.LogInformation(e.Message);

            try
            {
                //TODO: reinstate correct inspect call.
                //var inspectionResults = inspector.Inspect(reportDefinitionTriggerClient.OpenRead(), rulesInputStream);
                var inspectionResults = inspector.Inspect();
                

                var testRun = new TestRun() { CompletionTime = DateTime.Now, TestedFilePath = reportDefinitionTriggerClient.Uri.ToString(), RulesFilePath = "pbi-inspector-rules/Rules.json", Verbose = true, Results = inspectionResults };

                var jsonTestRun = System.Text.Json.JsonSerializer.Serialize(testRun);

                // Blob Output
                return jsonTestRun;
            }
            catch (ArgumentNullException ex)
            {
                logger.LogError(ex, "An error occurred during inspection");
                return string.Empty;
            }
            catch (FileNotFoundException ex)
            {
                logger.LogError(ex, "An error occurred during inspection");
                return string.Empty;
            }
            catch (PBIXInspectorException ex)
            {
                logger.LogError(ex, "An error occurred during inspection");
                return string.Empty;
            }
            finally
            {
                logger.LogInformation("Inspection completed");
                inspector.MessageIssued -= (sender, e) => logger.LogInformation(e.Message);
            }
        }
    }
}