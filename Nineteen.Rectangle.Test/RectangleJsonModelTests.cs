using Nineteen.Rectangle.Core.Models;

namespace Nineteen.Rectangle.Test
{
    public class RectangleJsonModelTests
    {
        [Fact]
        public void Constructor_AssignsPointsCorrectly()
        {
            // Arrange
            var rectangle = new Core.Models.Rectangle(
                new Line(new Point(0, 0), new Point(0, 4)),
                new Line(new Point(4, 4), new Point(0, 4))
            );
            var expectedPoints = rectangle.GetOrderedPoints().ToList();

            // Act
            var jsonModel = new RectangleJsonModel(rectangle);

            // Assert
            Assert.Equal(expectedPoints[0], jsonModel.Point1);
            Assert.Equal(expectedPoints[1], jsonModel.Point2);
            Assert.Equal(expectedPoints[2], jsonModel.Point3);
            Assert.Equal(expectedPoints[3], jsonModel.Point4);
        }
    }
}
