using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NAudio.Wave;
using Spectre.Console;

namespace Jarvis.Services;

public class RecordingServiceRunner : BackgroundService
{
    private readonly TranscriptionService _transcriptionService;
    private readonly QueryService _queryService;
    private readonly OutputService _outputService;
    public RecordingServiceRunner(TranscriptionService transcriptionService, QueryService queryService, OutputService outputService)
    {
        _transcriptionService = transcriptionService;
        _queryService = queryService;
        _outputService = outputService;
    }

    protected async Task PlayAudioFile(string filepath)
    {
        using var audioFile = new AudioFileReader(filepath);
        using var outputDevice = new WaveOutEvent();

        outputDevice.Init(audioFile);
        outputDevice.Play();


        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            await Task.Delay(1000);
        }

        await Task.Delay(0);

        outputDevice.Stop();
        audioFile.Close();

        outputDevice.Dispose();
        audioFile.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            Directory.CreateDirectory(outputFolder);
            var outputFilePath = Path.Combine(outputFolder, "recorded.wav");
            var outputAudioFilePath = Path.Combine(outputFolder, "response.mp3");

            WaveInEvent waveIn = null;
            WaveFileWriter writer = null;

            var recording = false;
            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=====================================================================================");
                Console.WriteLine("                             Jarvisnator version 1.0.0                               ");
                Console.WriteLine("=====================================================================================");

                Console.WriteLine("\n");
                Console.WriteLine("[1] " + (recording ? "Stop recording" : "Start recording"));
                Console.WriteLine("[2] Exit");
                Console.WriteLine(
                    "FYI: Program is an endless loop, you can have a full conversation with the AI if you keep pressing 1");


                var input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.D2)
                {
                    break;
                }
                else if (input.Key == ConsoleKey.D1)
                {
                    recording = !recording;

                    if (recording)
                    {
                        //Start recording
                        waveIn = new WaveInEvent();
                        writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
                        waveIn.StartRecording();

                        waveIn.DataAvailable += (s, a) =>
                        {
                            writer.Write(a.Buffer, 0, a.BytesRecorded);
                            if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
                            {
                                waveIn.StopRecording();
                                Task<string> transcription = _transcriptionService.Transcribe(outputFilePath);
                                Console.WriteLine(transcription);
                            }
                        };

                        waveIn.RecordingStopped += (s, a) =>
                        {
                            writer?.Dispose();
                            waveIn?.Dispose();
                            writer = null;

                            PlayAudioFile(outputFilePath).GetAwaiter().GetResult();

                            var transcription = _transcriptionService
                                .Transcribe(outputFilePath)
                                .Result;

                            Console.WriteLine("User: " + transcription);


                            var fromGPT = _queryService
                                .ProcessQuery(transcription)
                                .Result;

                            Console.WriteLine("Chat GPT: " + fromGPT);

                            var whisper  =_outputService.Output(fromGPT, outputAudioFilePath).Result;

                            PlayAudioFile(outputAudioFilePath).GetAwaiter().GetResult();

                        };
                    }
                    else
                    {
                        waveIn.StopRecording();
                    }
                }

            }

            

            

            await Task.Delay(500, stoppingToken); // Delay for 500 milliseconds before asking again
        }
    }
}