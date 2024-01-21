using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle
{
    public class RectangleProcessor : BaseRectangleProcessor, IRectangleProcessor
    {
        public RectangleProcessor(List<IPoint> points) : base(points)
        {

        }

        public List<IRectangle> Process()
        {
            var pointsGroupedByY = GroupPointsByY(Points);
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
            Console.WriteLine($"pointsGroupedByY = {pointsGroupedByY.Count}");
            return pointsGroupedByY;
        }

        public List<ILine> CreateLines(Dictionary<int, List<IPoint>> pointsGroupedByY)
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

        public List<IRectangle> FindPotentialRectangles(List<ILine> lines)
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
    }
}
