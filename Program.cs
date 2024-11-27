using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace KleraPiano
{
    class PianoApp
    {
        private static readonly Dictionary<ConsoleKey, string> KeyToNoteMap = new()
        {
            { ConsoleKey.A, "A" },
            { ConsoleKey.S, "B" },
            { ConsoleKey.D, "C" },
            { ConsoleKey.F, "D" },
            { ConsoleKey.G, "E" },
            { ConsoleKey.H, "F" },
            { ConsoleKey.J, "G" }
        };

        private static readonly Dictionary<string, string> NoteToSoundFileMap = new()
        {
            { "C", "C.wav" },
            { "D", "D.wav" },
            { "E", "E.wav" },
            { "F", "F.wav" },
            { "G", "G.wav" },
            { "A", "A.wav" },
            { "B", "B.wav" }
        };

        static void Main(string[] args)
        {
            DisplayPiano();
            DisplayKeyMapping();

            Console.WriteLine("Stiskněte F1 pro zobrazení nápovědy nebo klávesy A, S, D, F, G, H, J pro hraní not. Stiskněte ESC pro ukončení.");

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
                    Console.WriteLine("Neplatná klávesa.");
                }
            }
        }

        private static void DisplayPiano()
        {
            Console.Clear();
            Console.WriteLine("Virtuální Piano");
            Console.WriteLine("| C | D | E | F | G | A | B |");
            Console.WriteLine("|---|---|---|---|---|---|---|");
            Console.WriteLine("|   | # |   | # |   | # |   |");
        }

        private static void DisplayKeyMapping()
        {
            Console.WriteLine("\nMapa kláves:");
            Console.WriteLine("A -> C");
            Console.WriteLine("S -> D");
            Console.WriteLine("D -> E");
            Console.WriteLine("F -> F");
            Console.WriteLine("G -> G");
            Console.WriteLine("H -> A");
            Console.WriteLine("J -> B");
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("\nNápověda:");
            Console.WriteLine("Použijte klávesy A, S, D, F, G, H, J pro hraní not.");
            Console.WriteLine("Stiskněte ESC pro ukončení.");
        }

        private static void PlayNoteForKey(ConsoleKey key)
        {
            if (KeyToNoteMap.TryGetValue(key, out string note))
            {
                Console.WriteLine($"Hraje nota: {note}");
                PlaySound(NoteToSoundFileMap[note]);
            }
        }

        private static void PlaySound(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "ffplay",
                    Arguments = $"-nodisp -autoexit {filePath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při přehrávání souboru {filePath}: {ex.Message}");
            }
        }
    }
}
