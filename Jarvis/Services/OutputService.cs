using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace Jarvis.Services
{
    public class OutputService
    {
        private readonly OpenAIService _sdk;

        public OutputService(OpenAIClientService aiClient)
        {
            _sdk = aiClient.GetClient();
        }

        public async Task<string> Output(string text, string filepath)
        {
            try
            {
                var audioResult = await _sdk.Audio.CreateSpeech<Stream>(new AudioCreateSpeechRequest
                {
                    Model = Models.Tts_1,
                    Input = text,
                    Voice = StaticValues.AudioStatics.Voice.Alloy,
                    ResponseFormat = StaticValues.AudioStatics.CreateSpeechResponseFormat.Mp3,
                    Speed = 1.1f
                });

                if (audioResult.Successful)
                {
#if NET6_0_OR_GREATER
                    var audio = audioResult.Data!;
                    // save stream data as mp3 file
                    await using var fileStream = File.Create(filepath);
                    await audio.CopyToAsync(fileStream);
                    //await File.WriteAllBytesAsync("SampleData/speech.mp3", audioByteList);
#endif
                }
                else
                {
                    if (audioResult.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }

                    Console.WriteLine($"{audioResult.Error.Code}: {audioResult.Error.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return string.Empty;
        }
    }
}
