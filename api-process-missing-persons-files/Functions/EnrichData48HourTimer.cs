using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace api_process_missing_persons_files.Functions
{
    public class EnrichData48HourTimer
    {
        private readonly ILogger _logger;

        public EnrichData48HourTimer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EnrichData48HourTimer>();
        }

        [Function("EnrichData48HourTimer")]
        public void Run([TimerTrigger("0 0 */48 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
