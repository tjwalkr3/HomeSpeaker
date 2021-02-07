using System;
using System.IO;

namespace KeyboardListener
{
    class Program
    {
        static void Main(string[] args)
        {
            log("KeyboardListener starting up.  Listening to key pressed.");

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
                {
                    Console.WriteLine("Ctrl+C pressed.  Closing.");
                    break;
                }

                switch(key.Key)
                {
                    case ConsoleKey.NumPad0:
                        Console.WriteLine("NumPad0");
                        break;
                    default:
                        Console.WriteLine($"Key {key.Key} modifiers {key.Modifiers}");
                        break;
                }
            }
        }
        static void log(string message)
        {
            var logMessage = $"{DateTime.Now:yyyy-MM-dd at hh:mm:ss} | {message}";
            Console.WriteLine(logMessage);
            File.AppendAllText("keylog.txt", logMessage);
        }
    }
}
