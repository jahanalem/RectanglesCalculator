// See https://aka.ms/new-console-template for more information
using Nineteen.Rectangle;
using Nineteen.Rectangle.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        WelcomeMessage();

        var allPoints = BigData.POINTS;

        PrintInColor("Please wait...", ConsoleColor.Yellow);

        Stopwatch stopwatch = Stopwatch.StartNew();

        var rectangleProcessor = new RectangleProcessorParallel(allPoints);
        var distinctRectangles = rectangleProcessor.Process();

        PrintInColor($"The number of rectangles = {distinctRectangles.Count}", ConsoleColor.Green);

        stopwatch.Stop();

        PrintInColor($"Execution Time: {stopwatch.ElapsedMilliseconds} ms", ConsoleColor.Cyan);
        Console.WriteLine();

        PrintInColor("Would you like to view the results? (y/n)", ConsoleColor.Magenta);
        string userResponse = Console.ReadLine();

        if (userResponse.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            long countRectangle = 0;
            foreach (var rectangle in distinctRectangles)
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

    private static List<IPoint> GeneratePoints(int count)
    {
        var random = new Random();
        var points = new List<IPoint>();
        for (int i = 0; i < count; i++)
        {
            int x = random.Next(1, 1000);
            int y = random.Next(1, 1000);
            points.Add(new Point(x, y));
        }

        return points;
    }

    static void PrintInColor(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static void WelcomeMessage()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

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
}