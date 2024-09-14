using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Azure.Storage.Blobs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api_process_mp_pdfs.Utils
{
    public class AIHelper
    {
        private Kernel _kernel;

        private string _azure_StroageConnectionString = Environment.GetEnvironmentVariable("stggannettpoc_STORAGE", EnvironmentVariableTarget.Process) ?? "";
        private string _inbound_MP_PdfContainer = Environment.GetEnvironmentVariable("INBOUND_MP_PDF_CONTAINER", EnvironmentVariableTarget.Process) ?? "";

        private string _jsonSchema = @"
        {
            ""$schema"": ""http://json-schema.org/draft-07/schema#"",
            ""type"": ""object"",
            ""properties"": {
                ""name"": { ""type"": ""string"" },
                ""race"": { ""type"": ""string"" },
                ""age"": { ""type"": ""integer"", ""minimum"": 0 },
                ""sex"": { ""type"": ""string"" },
                ""height"": { ""type"": ""string"" },
                ""weight"": { ""type"": ""string"" },
                ""eye_color"": { ""type"": ""string"" },
                ""hair"": { ""type"": ""string"" },
                ""alias"": { ""type"": ""string"" },
                ""tattoos"": { ""type"": ""string"" },
                ""last_seen"": { ""type"": ""string"", ""format"": ""date-time"" },
                ""date_reported"": { ""type"": ""string"", ""format"": ""date-time"" },
                ""missing_from"": { ""type"": ""string"" },
                ""conditions_of_disappearance"": { ""type"": ""string"" },
                ""officer_details"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""name"": { ""type"": ""string"" },
                        ""badge_number"": { ""type"": ""string"" },
                        ""department"": { ""type"": ""string"" }
                    },
                    ""required"": [""name"", ""badge_number"", ""department""]
                },
                ""phone"": { ""type"": ""string"", ""pattern"": ""^\\\\+?[0-9]{10,15}$"" }
            },
            ""required"": [
                ""name"", ""age"", ""sex"", ""last_seen"", ""date_reported"", ""missing_from"",
                ""officer_details"", ""phone""
            ]
        }";

        private string _details = @"name, race, age, sex, height, weight, eye color, hair, alias, tattoos, last seen, date reported, missing from, conditions of disappearance, officer details, phone";

        private string? _promptAnalyzePDFPrompt;
        private string _promptIdealCandidateResumePrompt = @"Based on the following job requirements, create a resume for an ideal candidate:\n\n{{$analysis}} and use Markdown format";

        public AIHelper(Kernel kernel)
        {
            this._kernel = kernel;
            InitializePrompts();  // Initialize the prompt strings here
        }

        private void InitializePrompts()
        {
            _promptAnalyzePDFPrompt = $@"Text: {{$pdfText}} <end of text> Analyze the text and extract the {_details} from the text. Using the JSON schema below, populate the properties with the data you extracted and only return the JSON result: {_jsonSchema}";
        }

        public async Task<string> GenerateJSONStructureAsync(Stream pdfStream, string name)
        {
            var test = "test";
            var test2 = $@"test2 {test}";

            string extractedText = ExtractTextFromPdf(pdfStream);

            // Step 2: Analyze the extracted text using Azure OpenAI
            string analysis = await AnalyzePdfText(extractedText);

            // Step 3: Generate the ideal resume using the analyzed text
            // string idealResume = await GenerateResume(analysis);

            // Step 4: Write the ideal resume to the "Ideal_Resumes" container
            // await WriteToBlob(outputContainerName, $"{Path.GetFileNameWithoutExtension(name)}_IdealResume.md", idealResume);

            return "Finished generating ideal resume.";
        }

        private string ExtractTextFromPdf(Stream pdfStream)
        {
            StringBuilder text = new StringBuilder();

            using (PdfReader pdfReader = new PdfReader(pdfStream))
            using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
            {
                for (int pageNumber = 1; pageNumber <= pdfDoc.GetNumberOfPages(); pageNumber++)
                {
                    var page = pdfDoc.GetPage(pageNumber);
                    var strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                    text.Append(pageText);
                }
            }

            return text.ToString();
        }

        private async Task<string> AnalyzePdfText(string pdfText)
        {
            try
            {
                var executionSettings = new OpenAIPromptExecutionSettings()
                {
                    MaxTokens = 2000,
                };

                KernelArguments arguments = new(executionSettings) { { "pdfText", pdfText } };
                var response = await _kernel.InvokePromptAsync(_promptAnalyzePDFPrompt ?? "", arguments);
                
                // var response = await _kernel.InvokePromptAsync(_promptAnalyzePDFPrompt, arguments);
                return response.GetValue<string>() ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task<string> GenerateResume(string analysis)
        {
            try
            {
                var executionSettings = new OpenAIPromptExecutionSettings()
                {
                    MaxTokens = 2000,
                };

                KernelArguments arguments = new(executionSettings) { { "analysis", analysis } };
                var response = await _kernel.InvokePromptAsync(_promptIdealCandidateResumePrompt, arguments);
                return response.GetValue<string>() ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task WriteToBlob(string containerName, string blobName, string content)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_azure_StroageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = blobContainerClient.GetBlobClient(blobName);

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
