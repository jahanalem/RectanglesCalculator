using Nineteen.Rectangle.Core.Models;
using Nineteen.Rectangle.Core.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle.Test
{
    public class RectangleProcessorParallelTests
    {
        [Fact]
        public void GroupPointsByY_GroupsPointsCorrectly()
        {
            // Arrange
            var points = new List<Point>
            {
                new Point(0, 0),
                new Point(1, 0),
                new Point(0, 1),
                new Point(1, 1),
                new Point(0, 2),
                new Point(1, 2),
                new Point(2, 2),
                new Point(3, 2),
            };
            var processor = new RectangleProcessorParallel(points);

            // Act
            var groupedPoints = processor.GroupPointsByY(points);

            // Assert
            Assert.Equal(3, groupedPoints.Count);
            Assert.Contains(groupedPoints, g => g.Key == 0 && g.Value.Count == 2);
            Assert.Contains(groupedPoints, g => g.Key == 1 && g.Value.Count == 2);
            Assert.Contains(groupedPoints, g => g.Key == 2 && g.Value.Count == 4);
        }

        [Fact]
        public void CreateLines_CreatesCorrectNumberOfLines()
        {
            // Arrange
            var groupedPoints = new Dictionary<int, List<Point>>
    {
        { 0, new List<Point> { new Point(0, 0), new Point(1, 0) } },
        { 1, new List<Point> { new Point(0, 1), new Point(1, 1) } }
    };
            var processor = new RectangleProcessorParallel(new List<Point>());

            // Act
            var lines = processor.CreateLines(groupedPoints);

            // Assert
            Assert.Equal(2, lines.Count);
        }

        [Fact]
        public void FindPotentialRectangles_CorrectNumberOfPotentialRectangles()
        {
            // Arrange
            var lines = new List<Line>
            {
                // Create a rectangle
                new Line(new Point(0, 0), new Point(1, 0)),
                new Line(new Point(0, 1), new Point(1, 1)),
                new Line(new Point(0, 2), new Point(1, 2)),

            };
            var processor = new RectangleProcessorParallel(new List<Point>());

            // Act
            var rectangles = processor.FindPotentialRectangles(lines);

            // Assert
            Assert.NotEmpty(rectangles);
            Assert.Equal(3, rectangles.Count);
        }

        [Fact]
        public void GetMatchingXGroups_ReturnsCorrectGroups()
        {
            // Arrange
            var linesGroupedByY = new Dictionary<int, List<Line>>
            {
                { 0, new List<Line>
                    {
                        new Line(new Point(1, 0), new Point(2, 0)),
                        new Line(new Point(2, 0), new Point(3, 0))
                    }
                },
                { 1, new List<Line>
                    {
                        new Line(new Point(1, 1), new Point(2, 1)),
                        new Line(new Point(2, 1), new Point(3, 1))
                    }
                }
            };

            var processor = new RectangleProcessorParallel(new List<Point>());
            int baseLineX1 = 1, baseLineX2 = 2;

            // Act
            var matchingGroups = processor.GetMatchingXGroups(linesGroupedByY, baseLineX1, baseLineX2);

            // Assert
            Assert.NotEmpty(matchingGroups);
            Assert.All(matchingGroups, group =>
                group.Value.Any(line =>
                    line.Point1.X == baseLineX1 && line.Point2.X == baseLineX2)
            );
        }
    }
}
