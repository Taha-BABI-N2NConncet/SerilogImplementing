using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SerilogImplementing.Logger;
using static System.Net.Mime.MediaTypeNames;

namespace SerilogImplementing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public void Get()
        {
            for (int i = 0; i < 5; i++)
            {
                BackgroundQueueLogger.AddLoggingTaskToQueue(() => {
                    Console.WriteLine(i.ToString());       
                });
            }
            Thread.Sleep(TimeSpan.FromSeconds(2));
            for (int i = 0; i < 5; i++)
            {
                int value = i;
                BackgroundQueueLogger.AddLoggingTaskToQueue(() => {
                    Console.WriteLine(value + "");       
                });
            }
            //_logger.LogInformation("Seri Log is Working");
        }
    }

}
