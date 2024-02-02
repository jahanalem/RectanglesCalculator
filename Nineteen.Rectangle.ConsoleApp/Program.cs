// See https://aka.ms/new-console-template for more information
using Nineteen.Rectangle;
using Nineteen.Rectangle.ConsoleApp.IO;
using Nineteen.Rectangle.ConsoleApp.Processing;
using Nineteen.Rectangle.ConsoleApp.UI;
using Nineteen.Rectangle.Core.Models;
using Nineteen.Rectangle.Core.Utilities;
using System.Diagnostics;

class Program
{
    private static readonly string DataFilePath = "data_points_10000.json";
    private static readonly string JsonFilePath = "rectangles.json";
    private static readonly FileOperations fileOperations = new FileOperations();

    static void Main()
    {
        UserInterface.WelcomeMessage();

        var allPoints = LoadPoints();
        if (allPoints?.Count == 0)
        {
            UserInterface.PrintInColor("No points loaded. Program is ended.", ConsoleColor.Red);
            return;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        var (distinctRectangles, allDistinctPointsCount) = ProcessPoints(allPoints);
        stopwatch.Stop();

        SaveAndDisplayResults(allPoints, distinctRectangles, allDistinctPointsCount, stopwatch.ElapsedMilliseconds);
    }

    private static List<Point> LoadPoints()
    {
        var allPoints = fileOperations.GetPointsFromFile(DataFilePath);
        // Alternative data sources could be uncommented as needed
        // var allPoints = BigData.POINTS;
        // var allPoints = TestDataGenerator.GeneratePoints(1000);

        return allPoints;
    }

    private static (List<IRectangle>, long) ProcessPoints(List<Point> allPoints)
    {
        UserInterface.ProcessingStartMessage();
        var allDistinctPoints = allPoints.Distinct().ToList();
        var distinctRectangles = DataProcessor.ProcessData(allDistinctPoints);
        return (distinctRectangles, allDistinctPoints.LongCount());
    }

    private static void SaveAndDisplayResults(List<Point> allPoints,
        List<IRectangle> distinctRectangles,
        long allDistinctPointsCount,
        long elapsedMilliseconds)
    {
        fileOperations.SaveRectanglesToJson(distinctRectangles, JsonFilePath);

        UserInterface.PrintResultsSummary(
            allPoints.Count,
            allDistinctPointsCount,
            distinctRectangles.Count,
            elapsedMilliseconds
        );

        UserInterface.AskToSaveResults(distinctRectangles);
        UserInterface.AskToDisplayResults(distinctRectangles);
    }
}
