// See https://aka.ms/new-console-template for more information
using Nineteen.Rectangle;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var allPoints = Data.POINTS;

        var distinctYValues = allPoints.Select(point => point.Y).Distinct().ToList();

        var pointsGroupedByYValue = new Dictionary<int, List<IPoint>>();
        foreach (var yValue in distinctYValues)
        {
            var pointsWithSameY = allPoints.Where(point => point.Y == yValue).ToList();
            pointsGroupedByYValue.Add(yValue, pointsWithSameY);
        }

        var linesGroupedByYValue = new Dictionary<int, List<ILine>>();
        var lineId = 0;
        foreach (var group in pointsGroupedByYValue)
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
                linesGroupedByYValue.Add(lineId, horizontalLinesAtY);
            }
        }

        var potentialRectangles = new List<IRectangle>();

        var linesWithSameYValueList = linesGroupedByYValue.Where(pair => pair.Value.Count > 0).ToList();
        foreach (var yValueLinesPair in linesWithSameYValueList)
        {
            foreach (var baseLine in yValueLinesPair.Value)
            {
                var baseLineX1 = baseLine.Point1.X;
                var baseLineX2 = baseLine.Point2.X;

                foreach (var comparisonPair in linesGroupedByYValue)
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

        var distinctRectangles = potentialRectangles.Distinct().ToList();
        Console.WriteLine($"The number of rectangles: {distinctRectangles.Count}");
        foreach (var rectangle in distinctRectangles)
        {
            Console.WriteLine(rectangle.ToString());
        }
    }
}
