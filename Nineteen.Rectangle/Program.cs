// See https://aka.ms/new-console-template for more information
using Nineteen.Rectangle;
using Nineteen.Rectangle.ConsoleApp.IO;
using Nineteen.Rectangle.ConsoleApp.Processing;
using Nineteen.Rectangle.ConsoleApp.UI;
using Nineteen.Rectangle.Core.Utilities;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        UserInterface.WelcomeMessage();
        var fileOperations = new FileOperations();

        var allPoints = fileOperations.GetPointsFromFile("data_points.json");
        //var allPoints = BigData.POINTS;
        //var dataSaver = new DataSaver();
        //var allPoints = TestDataGenerator.GeneratePoints(1000);

        if (allPoints.Count == 0)
        {
            UserInterface.PrintInColor("No points loaded. Program is ended.", ConsoleColor.Red);
            return;
        }

        var allDistinctPoints = allPoints.Distinct().ToList();
        UserInterface.ProcessingStartMessage();
        Stopwatch stopwatch = Stopwatch.StartNew();
        var distinctRectangles = DataProcessor.ProcessData(allDistinctPoints);
        stopwatch.Stop();

        string jsonFilePath = "rectangles.json";
        fileOperations.SaveRectanglesToJson(distinctRectangles, jsonFilePath);

        UserInterface.PrintResultsSummary(allPoints.LongCount(), allDistinctPoints.LongCount(), distinctRectangles.Count, stopwatch.ElapsedMilliseconds);

        UserInterface.AskToSaveResults(distinctRectangles);
        UserInterface.AskToDisplayResults(distinctRectangles);
    }
}