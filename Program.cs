using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

Dictionary<ConsoleKey, string> KeyToNoteMap = new()
{
    { ConsoleKey.A, "C" },
    { ConsoleKey.S, "D" },
    { ConsoleKey.D, "E" },
    { ConsoleKey.F, "F" },
    { ConsoleKey.G, "G" },
    { ConsoleKey.H, "A" },
    { ConsoleKey.J, "B" }
};

Dictionary<string, double> NoteToFrequencyMap = new()
{
    { "C", 261.63 }, // C4
    { "D", 293.66 }, // D4
    { "E", 329.63 }, // E4
    { "F", 349.23 }, // F4
    { "G", 392.00 }, // G4
    { "A", 440.00 }, // A4
    { "B", 493.88 }  // B4
};

// Shared WaveOutEvent and WaveProvider
WaveOutEvent waveOut = new WaveOutEvent();
BufferedWaveProvider bufferProvider;

DisplayPiano();
DisplayKeyMapping();

Console.WriteLine("Press F1 for help or keys A, S, D, F, G, H, J to play notes. Press ESC to quit.");

bool running = true;
while (running)
{
    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

    if (keyInfo.Key == ConsoleKey.Escape)
    {
        running = false;
    }
    else if (keyInfo.Key == ConsoleKey.F1)
    {
        DisplayHelp();
    }
    else if (KeyToNoteMap.ContainsKey(keyInfo.Key))
    {
        PlayNoteForKey(keyInfo.Key);
    }
    else
    {
        Console.WriteLine("Invalid key.");
    }
}

void DisplayPiano()
{
    Console.Clear();
    Console.WriteLine("Virtual Piano");
    Console.WriteLine("| C | D | E | F | G | A | B |");
    Console.WriteLine("|---|---|---|---|---|---|---|");
    Console.WriteLine("|   | # |   | # |   | # |   |");
}

void DisplayKeyMapping()
{
    Console.WriteLine("\nKey Mapping:");
    Console.WriteLine("A -> C");
    Console.WriteLine("S -> D");
    Console.WriteLine("D -> E");
    Console.WriteLine("F -> F");
    Console.WriteLine("G -> G");
    Console.WriteLine("H -> A");
    Console.WriteLine("J -> B");
}

void DisplayHelp()
{
    Console.WriteLine("\nHelp:");
    Console.WriteLine("Use keys A, S, D, F, G, H, J to play notes.");
    Console.WriteLine("Press ESC to quit.");
}

void PlayNoteForKey(ConsoleKey key)
{
    if (KeyToNoteMap.TryGetValue(key, out string? note) && NoteToFrequencyMap.TryGetValue(note, out double frequency))
    {
        // Stop any currently playing note
        waveOut.Stop();

        // Create a new buffer and generate the tone
        bufferProvider = GenerateToneBuffer(frequency, 44100, 500);
        waveOut.Init(bufferProvider);
        waveOut.Play();

        Console.WriteLine($"Playing note: {note}");
    }
}

BufferedWaveProvider GenerateToneBuffer(double frequency, int sampleRate, int durationMs)
{
    WaveFormat waveFormat = new WaveFormat(sampleRate, 16, 1);
    var buffer = new BufferedWaveProvider(waveFormat);

    int samplesToGenerate = (int)(sampleRate * (durationMs / 1000.0));
    short[] sampleBuffer = new short[samplesToGenerate];

    for (int i = 0; i < sampleBuffer.Length; i++)
    {
        double t = (double)i / sampleRate;
        sampleBuffer[i] = (short)(Math.Sin(2 * Math.PI * frequency * t) * short.MaxValue);
    }

    byte[] byteBuffer = new byte[sampleBuffer.Length * sizeof(short)];
    Buffer.BlockCopy(sampleBuffer, 0, byteBuffer, 0, byteBuffer.Length);
    buffer.AddSamples(byteBuffer, 0, byteBuffer.Length);

    return buffer;
}
