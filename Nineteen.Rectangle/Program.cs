// See https://aka.ms/new-console-template for more information
using Nineteen.Rectangle;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        var allPoints = Data.POINTS;

        Stopwatch stopwatch = Stopwatch.StartNew();

        var pointsGroupedByY = GroupPointsByY(allPoints);

        var linesGroupedByY = CreateLines(pointsGroupedByY);

        var potentialRectangles = FindPotentialRectangles(linesGroupedByY);

        var distinctRectangles = DeduplicateRectangles(potentialRectangles);

        Console.WriteLine($"The number of rectangles: {distinctRectangles.Count}");
        foreach (var rectangle in distinctRectangles)
        {
            Console.WriteLine(rectangle.ToString());
        }

        Console.WriteLine($"The number of rectangles: {distinctRectangles.Count}");
        stopwatch.Stop();
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    private static Dictionary<int, List<IPoint>> GroupPointsByY(List<IPoint> points)
    {
        var distinctYValues = points.Select(point => point.Y).Distinct().ToList();

        var pointsGroupedByY = new Dictionary<int, List<IPoint>>();
        foreach (var yValue in distinctYValues)
        {
            var pointsWithSameY = points.Where(point => point.Y == yValue).ToList();
            pointsGroupedByY.Add(yValue, pointsWithSameY);
        }

        return pointsGroupedByY;
    }

    private static List<ILine> CreateLines(Dictionary<int, List<IPoint>> pointsGroupedByY)
    {
        var lines = new List<ILine>();

        foreach (var group in pointsGroupedByY)
        {
            for (int i = 0; i < group.Value.Count; i++)
            {
                for (int j = i + 1; j < group.Value.Count; j++)
                {
                    var newLine = new Line(group.Value[i], group.Value[j]);
                    lines.Add(newLine);
                }
            }
        }

        return lines;
    }

    private static List<IRectangle> FindPotentialRectangles(List<ILine> lines)
    {
        var potentialRectangles = new List<IRectangle>();

        var linesGroupedByY = lines.GroupBy(line => line.Point1.Y)
                                                            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var baseGroup in linesGroupedByY)
        {
            foreach (var baseLine in baseGroup.Value)
            {
                var baseLineX1 = baseLine.Point1.X;
                var baseLineX2 = baseLine.Point2.X;

                foreach (var comparisonGroup in linesGroupedByY)
                {
                    if (comparisonGroup.Key == baseGroup.Key)
                    {
                        continue; // Skip lines on the same Y level
                    }

                    foreach (var comparisonLine in comparisonGroup.Value)
                    {
                        var comparisonLineX1 = comparisonLine.Point1.X;
                        var comparisonLineX2 = comparisonLine.Point2.X;

                        if ((baseLineX1 == comparisonLineX1 && baseLineX2 == comparisonLineX2) || 
                            (baseLineX1 == comparisonLineX2 && baseLineX2 == comparisonLineX1))
                        {
                            ILine lowerLine = baseGroup.Key < comparisonGroup.Key ? baseLine : comparisonLine;
                            ILine upperLine = baseGroup.Key < comparisonGroup.Key ? comparisonLine : baseLine;
                            potentialRectangles.Add(new Rectangle(lowerLine, upperLine));
                        }
                    }
                }
            }
        }

        return potentialRectangles;
    }

    private static List<IRectangle> DeduplicateRectangles(List<IRectangle> potentialRectangles)
    {
        return potentialRectangles.Distinct().ToList();
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