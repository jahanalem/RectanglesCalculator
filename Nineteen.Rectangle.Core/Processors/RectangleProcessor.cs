using Nineteen.Rectangle.Core.Models;
using System.Runtime.InteropServices;

namespace Nineteen.Rectangle.Core.Processors
{
    public class RectangleProcessor : BaseRectangleProcessor, IRectangleProcessor
    {
        public RectangleProcessor(List<Point> points) : base(points)
        {

        }

        public List<IRectangle> Process()
        {
            var sortedPoints = Points.OrderBy(p => p.X).ToList();
            var pointsGroupedByY = GroupPointsByY(sortedPoints);
            var linesGroupedByY = CreateLines(pointsGroupedByY);
            var potentialRectangles = FindPotentialRectangles(linesGroupedByY);

            return potentialRectangles;
        }

        public Dictionary<int, List<Point>> GroupPointsByY(List<Point> points)
        {
            var pointsGroupedByY = new Dictionary<int, List<Point>>();
            var spanPoints = CollectionsMarshal.AsSpan(points);

            foreach (var point in spanPoints)
            {
                if (!pointsGroupedByY.TryGetValue(point.Y, out var group))
                {
                    group = new List<Point>();
                    pointsGroupedByY[point.Y] = group;
                }
                group.Add(point);
            }

            Console.WriteLine($"Number of different Y values with associated points = {pointsGroupedByY.Count}");
            return pointsGroupedByY;
        }

        public List<Line> CreateLines(Dictionary<int, List<Point>> pointsGroupedByY)
        {
            var lines = new List<Line>();

            foreach (var group in pointsGroupedByY.Values)
            {
                for (int i = 0; i < group.Count; i++)
                {
                    for (int j = i + 1; j < group.Count; j++)
                    {
                        var newLine = new Line(group[i], group[j]);
                        lines.Add(newLine);
                    }
                }
            }

            return lines;
        }

        public List<IRectangle> FindPotentialRectangles(List<Line> lines)
        {
            var potentialRectangles = new HashSet<IRectangle>();

            var linesGroupedByY = lines.GroupBy(line => line.Point1.Y)
                                                                .ToDictionary(group => group.Key, group => group.ToList());

            long comparisonCount = 0;

            foreach (var baseGroup in linesGroupedByY)
            {
                foreach (var baseLine in baseGroup.Value)
                {
                    var baseLineX1 = baseLine.Point1.X;
                    var baseLineX2 = baseLine.Point2.X;

                    var matchingXGroups = GetMatchingXGroups(linesGroupedByY, baseLineX1, baseLineX2);

                    foreach (var comparisonGroup in matchingXGroups)
                    {
                        if (comparisonGroup.Key <= baseGroup.Key)
                        {
                            continue; // Skip lines on the same Y level or smaller
                        }

                        var comparisonLines = comparisonGroup.Value;
                        foreach (var comparisonLine in comparisonLines)
                        {
                            comparisonCount++;
                            var comparisonLineX1 = comparisonLine.Point1.X;
                            var comparisonLineX2 = comparisonLine.Point2.X;

                            if (baseLineX1 == comparisonLineX1 && baseLineX2 == comparisonLineX2 ||
                                baseLineX1 == comparisonLineX2 && baseLineX2 == comparisonLineX1)
                            {
                                Line lowerLine = baseGroup.Key < comparisonGroup.Key ? baseLine : comparisonLine;
                                Line upperLine = baseGroup.Key < comparisonGroup.Key ? comparisonLine : baseLine;
                                potentialRectangles.Add(new Models.Rectangle(lowerLine, upperLine));
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"Number of comparison operations = {comparisonCount}");

            return potentialRectangles.ToList();
        }

        public List<KeyValuePair<int, List<Line>>> GetMatchingXGroups(
            Dictionary<int, List<Line>> linesGroupedByY, int baseLineX1, int baseLineX2)
        {
            var matchingXGroups = new List<KeyValuePair<int, List<Line>>>();

            foreach (var yGroup in linesGroupedByY)
            {
                foreach (var line in yGroup.Value)
                {
                    if (line.Point1.X == baseLineX1 && line.Point2.X == baseLineX2)
                    {
                        matchingXGroups.Add(yGroup);
                        break; //Break the inner loop when a match is found
                    }
                }
            }

            return matchingXGroups;
        }
    }
}
