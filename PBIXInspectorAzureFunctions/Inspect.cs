
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace PBIXInspectorAzureFunctions
{
    public class Inspect
    {
        private readonly ILogger<Inspect> _logger;

        public Inspect(ILogger<Inspect> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Inspect))]
        public async Task Run([BlobTrigger("report-definitions/{name}", Connection = "BlobStorageSetting")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name}");
            //_logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
