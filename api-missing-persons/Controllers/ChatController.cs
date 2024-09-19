using api_missing_persons.Models;
using api_missing_persons.Plugins;
using api_missing_persons.Prompts;
using api_missing_persons.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernel.Data.Nl2Sql.Harness;
using System.Net.Mime;

namespace api_missing_persons.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chat;
        private readonly IChatHistoryManager _chatHistoryManager;

        public ChatController(
            ILogger<ChatController> logger, 
            IConfiguration configuration,
             Kernel kernel,
             IChatCompletionService chat,
             IChatHistoryManager chathistorymanager)
        {
            _kernel = kernel;
            _chat = chat;
            _chatHistoryManager = chathistorymanager;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ChatProviderRequest chatRequest)
        {
            if (string.IsNullOrEmpty(chatRequest.SessionId))
            {
                // needed for new chats
                chatRequest.SessionId = Guid.NewGuid().ToString();
            }

            var sessionId = chatRequest.SessionId;
            var chatHistory = _chatHistoryManager.GetOrCreateChatHistory(sessionId);
            var sqlHarness = new SqlSchemaProviderHarness(_configuration);

            _kernel.ImportPluginFromObject(new DBQueryPlugin(_configuration));
            var response = new ChatProviderResponse();

            var jsonSchema = string.Empty;
            string[]? tableNames = null;

            tableNames = "dbo.MissingPersons".Split(","); // You can have more that one table defined here
            jsonSchema = await sqlHarness.ReverseEngineerSchemaJSONAsync(tableNames);

            chatHistory.AddUserMessage(NLPSqlPluginPrompts.GetNLPToSQLSystemPrompt(jsonSchema));
            chatHistory.AddUserMessage(chatRequest.Prompt);

            ChatMessageContent? result = null;

            result = await _chat.GetChatMessageContentAsync(
                  chatHistory,
                  executionSettings: new OpenAIPromptExecutionSettings { Temperature = 0.8, TopP = 0.0, ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions },
                  kernel: _kernel);

            Console.WriteLine(result.Content);

            response.ChatResponse = result.Content;

            return new OkObjectResult(response);
        }
    }
}
