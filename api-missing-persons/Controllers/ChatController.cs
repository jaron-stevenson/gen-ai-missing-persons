using api_missing_persons.Interfaces;
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
        private readonly IAzureDbService _azureDbService;

        public ChatController(
            ILogger<ChatController> logger, 
            IConfiguration configuration,
             Kernel kernel,
             IChatCompletionService chat,
             IChatHistoryManager chathistorymanager,
             IAzureDbService azuredbservice)
        {
            _kernel = kernel;
            _chat = chat;
            _chatHistoryManager = chathistorymanager;
            _logger = logger;
            _configuration = configuration;
            _azureDbService = azuredbservice;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ChatProviderRequest chatRequest)
        {
            var response = new ChatProviderResponse();

            try
            {
                if (string.IsNullOrEmpty(chatRequest.SessionId))
                {
                    // needed for new chats
                    chatRequest.SessionId = Guid.NewGuid().ToString();
                }

                if (string.IsNullOrEmpty(chatRequest.Prompt))
                {
                    _logger.LogWarning("Chat request is missing prompt.");
                    return new BadRequestResult();
                }

                var sessionId = chatRequest.SessionId;
                var chatHistory = _chatHistoryManager.GetOrCreateChatHistory(sessionId);

                _kernel.ImportPluginFromObject(new DBQueryPlugin(_azureDbService));

                var jsonSchema = await GetDatabaseSchemaAsync();

                chatHistory.AddUserMessage(NLPSqlPluginPrompts.GetNLPToSQLSystemPrompt(jsonSchema));
                chatHistory.AddUserMessage(chatRequest.Prompt);

                ChatMessageContent? result = null;

                result = await _chat.GetChatMessageContentAsync(
                      chatHistory,
                      executionSettings: new OpenAIPromptExecutionSettings { Temperature = 0.8, TopP = 0.0, ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions },
                      kernel: _kernel);

                response.ChatResponse = result.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat request");
                return new BadRequestResult();
            }                      

            return new OkObjectResult(response);
        }

        private async Task<string> GetDatabaseSchemaAsync()
        {
            var sqlHarness = new SqlSchemaProviderHarness(_configuration);
            var jsonSchema = string.Empty;
            var tables = _configuration.GetValue<string>("Tables") ?? throw new ArgumentNullException("Tables");
            var tableNames = tables.Split("|");
            jsonSchema = await sqlHarness.ReverseEngineerSchemaJSONAsync(tableNames);

            return jsonSchema;
        }
    }
}
