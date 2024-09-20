using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using api_process_mp_pdfs.Utils;
using api_process_mp_pdfs.Models;

namespace api_process_mp_pdfs.Function
{
    public class BlobTriggerMP_ProcessPDFs
    {
        private readonly ILogger<BlobTriggerMP_ProcessPDFs> _logger;
        private readonly Kernel _kernel;
        private readonly AIHelper _aiHelper;

        public BlobTriggerMP_ProcessPDFs(ILogger<BlobTriggerMP_ProcessPDFs> logger, ILogger<AIHelper> aiLogger, Kernel kernel)
        {
            _logger = logger;
            _kernel = kernel;
            _aiHelper = new AIHelper(_kernel, aiLogger);
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

            MissingPerson result = await _aiHelper.GenerateJSONStructureAsync(memoryStream, name);
            string sqlConnectionString = Environment.GetEnvironmentVariable("DatabaseConnection")?? "";
            
            SQLMissingPersonHelper sqlmissingpersonhelper = new SQLMissingPersonHelper(sqlConnectionString);
            await sqlmissingpersonhelper.InsertMissingPersonAsync(result);
               
            Console.WriteLine($@"Result: {result}");
            _logger.LogInformation($"Result: {result}"); 
        }
    }
}
