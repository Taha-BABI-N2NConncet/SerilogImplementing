using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SerilogImplementing.Logger;
using static SerilogImplementing.Logger.BackgroundQueueLogger;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace SerilogImplementing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        string str;
        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
            str = GenerateRandomString(10000);
        }
        [HttpGet]
        public void Get()
        {
            for (int i = 0; i < 10000; i++)
            {
                int value = i;
                BackgroundQueueLogger.AddLoggingTaskToQueue(() => {
                    _logger.LogInformation(value.ToString()+"  "+ str + "\n\n\n\n\n\n\n\n");
                });
            }
        }

        //this method is secondary one just for testing nothing more
        [NonAction]
        public string GenerateRandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
