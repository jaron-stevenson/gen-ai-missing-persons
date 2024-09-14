using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using api_process_mp_pdfs.Utils;

namespace api_process_mp_pdfs.Function
{
    public class BlobTriggerMP_ProcessPDFs
    {
        private readonly ILogger<BlobTriggerMP_ProcessPDFs> _logger;
        private readonly Kernel _kernel;
        private readonly AIHelper _aiHelper;


        public BlobTriggerMP_ProcessPDFs(ILogger<BlobTriggerMP_ProcessPDFs> logger, Kernel kernel, AIHelper aiHelper)
        {
            _logger = logger;
            _kernel = kernel;
            _aiHelper = new AIHelper(_kernel);
        }

        [Function(nameof(BlobTriggerMP_ProcessPDFs))]
        public async Task Run([BlobTrigger("inboundmppdfs/{name}", Connection = "stggannettpoc_STORAGE")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            stream.Position = 0;
            
            MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");

            var result = await _aiHelper.GenerateJSONStructureAsync(memoryStream, name);
            _logger.LogInformation($"Result: {result}");
        }
    }
}
