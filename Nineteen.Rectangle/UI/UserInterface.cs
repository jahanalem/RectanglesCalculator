using Nineteen.Rectangle.ConsoleApp.IO;
using Nineteen.Rectangle.Core;
using System.Text;

namespace Nineteen.Rectangle.ConsoleApp.UI
{
    public static class UserInterface
    {
        public static void WelcomeMessage()
        {
            Console.OutputEncoding = Encoding.UTF8;

            string[] messages = {
        "Willkommen beim Rechteck-Detektor-Programm!",
        "",
        "Dieses Werkzeug identifiziert Rechtecke, die aus einer Menge von gegebenen Punkten gebildet werden.",
        "",
        "So funktioniert es:",
        "- Eingabe: Eine Liste von Koordinatenpunkten.",
        "- Ausgabe: Alle identifizierten Rechtecke basierend auf diesen Punkten.",
        "",
        "Die Verarbeitungszeit variiert je nach Eingabegröße."
    };

            int maxLength = messages.Max(s => s.Length);

            PrintInColor(new string('═', maxLength + 4), ConsoleColor.DarkGreen);
            foreach (var message in messages)
            {
                string paddedMessage = message.PadRight(maxLength);
                PrintInColor("║ " + paddedMessage + " ║", ConsoleColor.DarkGreen);
            }
            PrintInColor(new string('═', maxLength + 4), ConsoleColor.DarkGreen);
            Console.WriteLine();
        }

        public static void PrintInColor(string message, ConsoleColor color = ConsoleColor.White, bool blankLine = false)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
            if (blankLine)
            {
                Console.WriteLine();
            }
        }

        public static void ProcessingStartMessage()
        {
            PrintInColor("Processing started. Please wait...", ConsoleColor.Yellow, true);
        }

        public static void PrintResultsSummary(int totalPoints, int totalRectangles, long executionTime)
        {
            PrintInColor($"Total number of points loaded: {totalPoints}", ConsoleColor.Blue, true);
            PrintInColor($"The number of rectangles = {totalRectangles}", ConsoleColor.Green);
            PrintInColor($"Execution Time: {executionTime} ms", ConsoleColor.Cyan);
            Console.WriteLine();
        }

        public static void DataReadingStartMessage(string filePath)
        {
            PrintInColor($"Reading data from '{filePath}'...", ConsoleColor.Gray, true);
        }

        public static void DataReadingCompletedMessage()
        {
            PrintInColor("Data reading completed.", ConsoleColor.Gray, true);
        }

        public static void AskToSaveResults(List<IRectangle> rectangles)
        {
            PrintInColor("Would you like to save the results to a file? (y/n)", ConsoleColor.Magenta);
            string saveResponse = Console.ReadLine() ?? string.Empty;

            if (saveResponse.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                string filePath = "results.txt";
                FileOperations.SaveResultsToFile(rectangles, filePath);
                PrintInColor($"Results saved to {filePath}", ConsoleColor.Green);
            }
        }

        public static void AskToDisplayResults(List<IRectangle> rectangles)
        {
            PrintInColor("Would you like to view the results? (y/n)", ConsoleColor.Magenta);
            string userResponse = Console.ReadLine() ?? string.Empty;

            if (userResponse.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                long countRectangle = 0;
                foreach (var rectangle in rectangles)
                {
                    countRectangle++;
                    PrintInColor($"{countRectangle}:  {rectangle.ToString()}");
                }
                PrintInColor("No more data. Press any key to exit.", ConsoleColor.Red);
                Console.ReadLine();
            }
            else
            {
                PrintInColor("Results are not displayed.", ConsoleColor.Red);
                Console.ReadLine();
            }
        }
    }
}
