// See https://aka.ms/new-console-template for more information
using Nineteen.Rectangle;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var allPoints = Data.POINTS;

        var pointsGroupedByY = GroupPointsByY(allPoints);

        var linesGroupedByY = CreateLines(pointsGroupedByY);

        var potentialRectangles = FindPotentialRectangles(linesGroupedByY);

        var distinctRectangles = DeduplicateRectangles(potentialRectangles);

        Console.WriteLine($"The number of rectangles: {distinctRectangles.Count}");
        foreach (var rectangle in distinctRectangles)
        {
            Console.WriteLine(rectangle.ToString());
        }
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

    private static Dictionary<int, List<ILine>> CreateLines(Dictionary<int, List<IPoint>> pointsGroupedByY)
    {
        var linesGroupedByY = new Dictionary<int, List<ILine>>();
        var lineId = 0;
        foreach (var group in pointsGroupedByY)
        {
            for (int i = 0; i < group.Value.Count; i++)
            {
                var horizontalLinesAtY = new List<ILine>();
                for (int j = i + 1; j < group.Value.Count; j++)
                {
                    var newLine = new Line(group.Value[i], group.Value[j]);
                    horizontalLinesAtY.Add(newLine);
                }
                lineId++;
                linesGroupedByY.Add(lineId, horizontalLinesAtY);
            }
        }

        return linesGroupedByY;
    }

    private static List<IRectangle> FindPotentialRectangles(Dictionary<int, List<ILine>> linesGroupedByY)
    {
        var potentialRectangles = new List<IRectangle>();

        var linesWithSameYValueList = linesGroupedByY.Where(pair => pair.Value.Count > 0).ToList();
        foreach (var yValueLinesPair in linesWithSameYValueList)
        {
            foreach (var baseLine in yValueLinesPair.Value)
            {
                var baseLineX1 = baseLine.Point1.X;
                var baseLineX2 = baseLine.Point2.X;

                foreach (var comparisonPair in linesGroupedByY)
                {
                    if (comparisonPair.Key == yValueLinesPair.Key)
                    {
                        continue; // Skip lines on the same Y level
                    }

                    foreach (var comparisonLine in comparisonPair.Value)
                    {
                        var comparisonLineX1 = comparisonLine.Point1.X;
                        var comparisonLineX2 = comparisonLine.Point2.X;

                        if ((baseLineX1 == comparisonLineX1 && baseLineX2 == comparisonLineX2) || (baseLineX1 == comparisonLineX2 && baseLineX2 == comparisonLineX1))
                        {
                            ILine lowerLine = yValueLinesPair.Key < comparisonPair.Key ? baseLine : comparisonLine;
                            ILine upperLine = yValueLinesPair.Key < comparisonPair.Key ? comparisonLine : baseLine;
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
}