using OpenAI.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace Jarvis.Services;

public class QueryService
{
    private readonly OpenAIService _sdk;

    public QueryService(OpenAIClientService aiClient)
    {
        _sdk = aiClient.GetClient();
    }

    public async Task<string> ProcessQuery(string query)
    {
        Console.WriteLine("\nAsking GPT...\n");

        var completionResult = await _sdk.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are a helpful assistant."),
                ChatMessage.FromUser(query)
            },
            Model = Models.Gpt_3_5_Turbo,
            MaxTokens = 50//optional
        });
        if (completionResult.Successful)
        {
            return completionResult.Choices.First().Message.Content;
        }

        return string.Empty;
    }
}

