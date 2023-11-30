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
    public class TranscriptionService
    {
        private readonly OpenAIService _sdk;

        public TranscriptionService(OpenAIClientService aiClient)
        {
            _sdk = aiClient.GetClient();
        }

        public async Task<string> Transcribe(string filepath)
        {
            string filename = Path.GetFileName(filepath);

            var sampleFile = await FileExtensions.ReadAllBytesAsync(filepath);
            var audioResult = await _sdk.Audio.CreateTranscription(new AudioCreateTranscriptionRequest
            {
                FileName = filename,
                File = sampleFile,
                Model = Models.WhisperV1,
                ResponseFormat = StaticValues.AudioStatics.ResponseFormat.VerboseJson
            });
            if (audioResult.Successful)
            {
                //Console.WriteLine(string.Join("\n", audioResult.Text));
                return audioResult.Text;
            }
            else
            {
                if (audioResult.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
                Console.WriteLine($"{audioResult.Error.Code}: {audioResult.Error.Message}");
            }

            return string.Empty;
        }

        // This method returns a byte array given a filename
    }
}
