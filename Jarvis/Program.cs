using NAudio.Wave;
using System.Runtime.InteropServices;

var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
Directory.CreateDirectory(outputFolder);
var outputFilePath = Path.Combine(outputFolder, "recorded.wav");

WaveInEvent waveIn = null;
WaveFileWriter writer = null;

var recording = false;
Console.CursorVisible = false;

while (true)
{
	Console.Clear();
	Console.WriteLine("=====================================================================================");
	Console.WriteLine("                             Simple Audio Recorder                                   ");
	Console.WriteLine("=====================================================================================");

	Console.WriteLine("\n");

	Console.WriteLine("[1] " + (recording ? "Stop recording" : "Start recording"));
	Console.WriteLine("[2] Exit");


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
			// Start recording
			waveIn = new WaveInEvent();
			writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
			waveIn.StartRecording();

			waveIn.DataAvailable += (s, a) =>
			{
				writer.Write(a.Buffer, 0, a.BytesRecorded);
				if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
				{
					waveIn.StopRecording();
				}
			};

			waveIn.RecordingStopped += (s, a) =>
			{
				writer?.Dispose();
				waveIn?.Dispose();
				writer = null;

				using var audioFile = new AudioFileReader(outputFilePath);
				using var outputDevice = new WaveOutEvent();

				outputDevice.Init(audioFile);
				outputDevice.Play();
				while (outputDevice.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(1000);
				}

				outputDevice.Stop();
				audioFile.Close();

				outputDevice.Dispose();
				audioFile.Dispose();
			};
		}
		else
		{
			waveIn.StopRecording();
		}
	}

}

waveIn?.Dispose();
