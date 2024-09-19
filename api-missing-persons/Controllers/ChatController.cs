using api_missing_persons.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
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

        public ChatController(
            ILogger<ChatController> logger, 
            IConfiguration configuration,
             Kernel kernel)
        {
            _kernel = kernel;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ChatProviderRequest chatRequest)
        {
            var sqlHarness = new SqlSchemaProviderHarness(_configuration);
            var response = new ChatProviderResponse();

            var tableNames = "dbo.MissingPersons".Split(",");
            var jsonSchema = await sqlHarness.ReverseEngineerSchemaJSONAsync(tableNames);

            return Ok();
        }
    }
}
