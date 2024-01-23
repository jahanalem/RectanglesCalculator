// See https://aka.ms/new-console-template for more information
using Nineteen.Rectangle.ConsoleApp.IO;
using Nineteen.Rectangle.ConsoleApp.Processing;
using Nineteen.Rectangle.ConsoleApp.UI;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        UserInterface.WelcomeMessage();

        var allPoints = FileOperations.GetPointsFromFile("data_points.json");

        if (allPoints.Count == 0)
        {
            UserInterface.PrintInColor("No points loaded. Program is ended.", ConsoleColor.Red);
            return;
        }

        UserInterface.ProcessingStartMessage();
        Stopwatch stopwatch = Stopwatch.StartNew();
        var distinctRectangles = DataProcessor.ProcessData(allPoints);
        stopwatch.Stop();

        UserInterface.PrintResultsSummary(allPoints.Count, distinctRectangles.Count, stopwatch.ElapsedMilliseconds);

        UserInterface.AskToSaveResults(distinctRectangles);
        UserInterface.AskToDisplayResults(distinctRectangles);
    }
}