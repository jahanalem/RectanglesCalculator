using Nineteen.Rectangle.Core.Models;

namespace Nineteen.Rectangle.Test
{
    public class RectangleTests
    {
        [Fact]
        public void Constructor_AssignLines()
        {
            Line line1 = new Line(new Point(0, 0), new Point(4, 0));
            Line line2 = new Line(new Point(0, 4), new Point(4, 4));

            var rectangle = new Core.Models.Rectangle(line1, line2);

            Assert.Equal(line1, rectangle.Line1);
            Assert.Equal(line2, rectangle.Line2);
        }

        [Fact]
        public void GetOrderedPoints_ReturnsPointsInCorrectOrder()
        {
            Line line1 = new Line(new Point(4, 0), new Point(0, 0));
            Line line2 = new Line(new Point(4, 4), new Point(0, 4));

            var rectangle = new Core.Models.Rectangle(line1, line2);

            var points = rectangle.GetOrderedPoints().ToList();

            Assert.Equal(new Point(0, 0), points[0]);
            Assert.Equal(new Point(0, 4), points[1]);
            Assert.Equal(new Point(4, 0), points[2]);
            Assert.Equal(new Point(4, 4), points[3]);
        }

        [Fact]
        public void Equals_ReturnsTrueForEqualRectangles()
        {
            // Arrange
            var rect1 = new Core.Models.Rectangle(new Line(new Point(0, 0), new Point(1, 0)), new Line(new Point(0, 2), new Point(2, 2)));
            var rect2 = new Core.Models.Rectangle(new Line(new Point(0, 0), new Point(1, 0)), new Line(new Point(0, 2), new Point(2, 2)));


            // Act & Assert
            Assert.True(rect1.Equals(rect2));
        }

        [Fact]
        public void GetHashCode_ReturnsSameHashCodeForEqualRectangles()
        {
            // Arrange
            var rect1 = new Core.Models.Rectangle(new Line(new Point(0, 0), new Point(1, 0)), new Line(new Point(0, 2), new Point(2, 2)));
            var rect2 = new Core.Models.Rectangle(new Line(new Point(0, 0), new Point(1, 0)), new Line(new Point(0, 2), new Point(2, 2)));

            // Act & Assert
            Assert.Equal(rect1.GetHashCode(), rect2.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var rectangle = new Core.Models.Rectangle(new Line(new Point(0, 0), new Point(1, 0)), new Line(new Point(1, 1), new Point(0, 1)));

            // Act
            var result = rectangle.ToString();

            // Assert
            Assert.Equal("[(0,0), (1,0), (1,1), (0,1)]", result);
        }
    }
}
