using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Managers;

namespace Jarvis.Services
{
    public class OpenAIClientService
    {
        private readonly IConfiguration _config;

        public OpenAIClientService (IConfiguration config)
        {
            _config = config;
        }

        public OpenAIService GetClient()
        {
            var key = _config.GetSection("OpenAIKey").Get<string>() ?? string.Empty;

            var openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = key
            });

            return openAiService;
        }
    }
}
