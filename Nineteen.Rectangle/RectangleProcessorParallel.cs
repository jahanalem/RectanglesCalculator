using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle
{
    public class RectangleProcessorParallel : BaseRectangleProcessor, IRectangleProcessor
    {
        public RectangleProcessorParallel(List<IPoint> points) : base(points)
        {

        }

        public List<IRectangle> Process()
        {
            var sortedPoint = this.Points.OrderBy(p => p.X).ToList();
            var pointsGroupedByY = GroupPointsByY(sortedPoint);
            var linesGroupedByY = CreateLines(pointsGroupedByY);
            var potentialRectangles = FindPotentialRectangles(linesGroupedByY);

            return DeduplicateRectangles(potentialRectangles);
        }

        public Dictionary<int, List<IPoint>> GroupPointsByY(List<IPoint> points)
        {
            var distinctYValues = points.Select(point => point.Y).Distinct().ToList();

            var pointsGroupedByY = new Dictionary<int, List<IPoint>>();
            foreach (var yValue in distinctYValues)
            {
                var pointsWithSameY = points.Where(point => point.Y == yValue).ToList();

                pointsGroupedByY.Add(yValue, pointsWithSameY);
            }
            Console.WriteLine($"Number of different Y values with associated points = {pointsGroupedByY.Count}");

            return pointsGroupedByY;
        }

        public List<ILine> CreateLines(Dictionary<int, List<IPoint>> pointsGroupedByY)
        {
            var lines = new ConcurrentBag<ILine>();

            Parallel.ForEach(pointsGroupedByY, group =>
            {
                for (int i = 0; i < group.Value.Count; i++)
                {
                    for (int j = i + 1; j < group.Value.Count; j++)
                    {
                        var newLine = new Line(group.Value[i], group.Value[j]);
                        lines.Add(newLine);
                    }
                }
            });

            return lines.ToList();
        }

        public List<IRectangle> FindPotentialRectangles(List<ILine> lines)
        {
            var potentialRectangles = new ConcurrentBag<IRectangle>();
            var linesGroupedByY = lines.GroupBy(line => line.Point1.Y)
                                       .ToDictionary(group => group.Key, group => group.ToList());

            long comparisonCount = 0;
            Parallel.ForEach(linesGroupedByY, baseGroup =>
            {
                foreach (var baseLine in baseGroup.Value)
                {
                    var baseLineX1 = baseLine.Point1.X;
                    var baseLineX2 = baseLine.Point2.X;

                    var matchingXGroups = GetMatchingXGroups(linesGroupedByY, baseLineX1, baseLineX2);

                    foreach (var comparisonGroup in matchingXGroups)
                    {
                        if (comparisonGroup.Key == baseGroup.Key)
                        {
                            continue; // Skip lines on the same Y level
                        }

                        foreach (var comparisonLine in comparisonGroup.Value)
                        {
                            comparisonCount++;
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
            });

            Console.WriteLine($"Number of comparison operations = {comparisonCount}");

            return potentialRectangles.ToList();
        }

        public List<KeyValuePair<int, List<ILine>>> GetMatchingXGroups(
            Dictionary<int, List<ILine>> linesGroupedByY, int baseLineX1, int baseLineX2)
        {
            var matchingXGroups = new List<KeyValuePair<int, List<ILine>>>();

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
