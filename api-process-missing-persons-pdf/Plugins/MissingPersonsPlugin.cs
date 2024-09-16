using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using api_process_mp_pdfs.Models;
using api_process_mp_pdfs.Prompts;


// This is just a stub, I may or may not use it.
namespace api_process_mp_pdfs.Plugins.MissingPersonsPlugin
{
    internal class MissingPersonsPlugin
    {
        private IChatCompletionService _chatService;

        private readonly Kernel _kernel;
        public MissingPersonsPlugin(IChatCompletionService chatService, Kernel kernel)
        {
            _chatService = chatService;
            _kernel = kernel.Clone();  // Let's clone the kernel so we have a fresh copy to work with with zero plugins registered
        }

        [Microsoft.SemanticKernel.KernelFunction, Description("Extract fields from provided text")]
        public async IAsyncEnumerable<string> ExtractFieldsFromTextAsync(
            [Description("TextData"), Required] string textData)
        {
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                ResponseFormat = "json_object"
            };
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(MissingPersonsPluginPrompts.GetMissingPersonsExtractPrompt(textData));
            chatHistory.AddUserMessage("Generate the JSON Data for the provided textData");

            var assistantResponse = "";
            await foreach (var chatUpdate in _chatService.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings,_kernel))
            {      
                   assistantResponse += chatUpdate.ToString();          
                   yield return chatUpdate.ToString();
            }
            chatHistory.AddSystemMessage(assistantResponse); 
            
            // !!!!!!!! This is where the background task should be started !!!!!!!!!
            // But, I am just not sure it's need as this data comes back farily quickly, just it's JSON data but the client could be designed to handle that.
            // Also read my comments about this in the ChatStreamerController.cs file
            // Start the background task to process the trip and add the result to the cache
            // you would then need to expose an API to retrieve the results from the cache, the client would call this 
            // var jobId = Guid.NewGuid().ToString();  // generate the JobId
            // yield return $" JobId: {jobId}"; // return the JobId to the client
            // var result = StartBackgroundTripProcessing(jobId, location, categories, travelCompanions);
        }        
    }
}