// See https://aka.ms/new-console-template for more information
using Nineteen.Rectangle;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        var allPoints = Data.POINTS;

        Stopwatch stopwatch = Stopwatch.StartNew();

        var rectangleProcessor = new RectangleProcessorParallel(allPoints);
        var distinctRectangles = rectangleProcessor.Process();

        Console.WriteLine($"The number of rectangles: {distinctRectangles.Count}");
        foreach (var rectangle in distinctRectangles)
        {
            Console.WriteLine(rectangle.ToString());
        }

        Console.WriteLine($"The number of rectangles: {distinctRectangles.Count}");
        stopwatch.Stop();
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
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
}