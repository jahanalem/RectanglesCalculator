using Nineteen.Rectangle.Core.IO;
using Nineteen.Rectangle.Core.Models;
using System.Text.Json;

namespace Nineteen.Rectangle.Test
{
    public class CoreFileOperationsTests
    {
        [Fact]
        public void GetPointsFromFile_ReturnsPointsFromValidJson()
        {
            // Arrange
            var expectedPoints = new List<Point> { new Point(1, 1), new Point(3, 1) };
            var json = JsonSerializer.Serialize(expectedPoints);
            var tempFile = Path.GetTempFileName();

            File.WriteAllText(tempFile, json);
            var fileOperations = new CoreFileOperations();

            try
            {
                // Act
                var points = fileOperations.GetPointsFromFile(tempFile);

                // Assert
                Assert.Equal(expectedPoints.Count, points.Count);
                Assert.Equal(expectedPoints[0].X, points[0].X);
                Assert.Equal(expectedPoints[0].Y, points[0].Y);
                Assert.Equal(expectedPoints[1].X, points[1].X);
                Assert.Equal(expectedPoints[1].Y, points[1].Y);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ReadPointsFromJson_ReturnsCorrectPointsFromValidJson()
        {
            // Arrange
            var expectedPoints = new List<Point> { new Point(1, 2), new Point(3, 4) };
            var json = JsonSerializer.Serialize(expectedPoints);
            var tempFile = Path.GetTempFileName();

            File.WriteAllText(tempFile, json);
            var fileOperations = new CoreFileOperations();

            try
            {
                // Act
                var points = fileOperations.ReadPointsFromJson(tempFile);

                // Assert
                Assert.Equal(expectedPoints.Count, points.Count);
                Assert.Equal(expectedPoints[0].X, points[0].X);
                Assert.Equal(expectedPoints[0].Y, points[0].Y);
                Assert.Equal(expectedPoints[1].X, points[1].X);
                Assert.Equal(expectedPoints[1].Y, points[1].Y);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void SaveResultsToFile_WritesRectanglesToFileCorrectly()
        {
            // Arrange
            var rectangles = new List<IRectangle>
            {
                new Core.Models.Rectangle(new Line(new Point(0, 0), new Point(4, 0)), new Line(new Point(4, 4), new Point(0, 4))),
                new Core.Models.Rectangle(new Line(new Point(1, 1), new Point(2, 1)), new Line(new Point(2, 2), new Point(1, 2)))
            };
            var tempFile = Path.GetTempFileName();
            var fileOperations = new CoreFileOperations();

            try
            {
                // Act
                fileOperations.SaveResultsToFile(rectangles, tempFile);

                // Read the contents of the file
                var fileContents = File.ReadAllLines(tempFile);

                // Assert
                Assert.Equal(rectangles.Count, fileContents.Length);
                Assert.All(rectangles.Select((r, index) => r.ToString()), (result, index) =>
                {
                    Assert.Equal(result, fileContents[index]);
                });
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void SavePointsToFile_WritesPointsToJsonFileCorrectly()
        {
            // Arrange
            var points = new List<Point>
            {
                new Point(1, 2),
                new Point(3, 4)
            };
            var tempFile = Path.GetTempFileName();
            var fileOperations = new CoreFileOperations();

            try
            {
                // Act
                fileOperations.SavePointsToFile(points, tempFile);

                // Read the contents of the file
                var fileContents = File.ReadAllText(tempFile);
                var deserializedPoints = JsonSerializer.Deserialize<List<Point>>(fileContents);

                // Assert
                Assert.NotNull(deserializedPoints);
                Assert.Equal(points.Count, deserializedPoints.Count);
                for (int i = 0; i < points.Count; i++)
                {
                    Assert.Equal(points[i].X, deserializedPoints[i].X);
                    Assert.Equal(points[i].Y, deserializedPoints[i].Y);
                }
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void SaveRectanglesToJson_WritesRectanglesToJsonFileCorrectly()
        {
            // Arrange
            var rectangles = new List<IRectangle>
            {
                new Core.Models.Rectangle(new Line(new Point(0, 0), new Point(1, 0)), new Line(new Point(1, 1), new Point(0, 1))),
                new Core.Models.Rectangle(new Line(new Point(3, 3), new Point(4, 3)), new Line(new Point(4, 4), new Point(3, 4)))
            };
            var tempFile = Path.GetTempFileName();
            var fileOperations = new CoreFileOperations();

            try
            {
                // Act
                fileOperations.SaveRectanglesToJson(rectangles, tempFile);

                // Read the contents of the file
                var fileContents = File.ReadAllText(tempFile);
                var deserializedRectangles = JsonSerializer.Deserialize<List<RectangleJsonModel>>(fileContents);

                // Assert
                Assert.NotNull(deserializedRectangles);
                Assert.Equal(rectangles.Count, deserializedRectangles.Count);
                for (int i = 0; i < rectangles.Count; i++)
                {
                    var rectModel = new RectangleJsonModel(rectangles[i]);
                    var deserializedRectModel = deserializedRectangles[i];

                    Assert.Equal(rectModel.Point1.X, deserializedRectModel.Point1.X);
                    Assert.Equal(rectModel.Point1.Y, deserializedRectModel.Point1.Y);
                }
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }
    }
}
