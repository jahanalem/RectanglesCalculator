using Nineteen.Rectangle.Core.Models;


namespace Nineteen.Rectangle.Test
{
    public class LineTests
    {
        [Fact]
        public void Constructor_AssignsPoints()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(1, 1);

            var line = new Line(point1, point2);

            Assert.Equal(point1, line.Point1);
            Assert.Equal(point2, line.Point2);
        }

        [Fact]
        public void Equals_ReturnsTrueForEqualLines()
        {
            // Arrange
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(0, 0), new Point(1, 1));

            // Act & Assert
            Assert.True(line1.Equals(line2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentLines()
        {
            // Arrange
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(1, 1), new Point(2, 2));

            // Act & Assert
            Assert.False(line1.Equals(line2));
        }

        [Fact]
        public void GetHashCode_ReturnsSameHashCodeForEqualLines()
        {
            // Arrange
            var line1 = new Line(new Point(0, 0), new Point(1, 1));
            var line2 = new Line(new Point(0, 0), new Point(1, 1));

            // Act & Assert
            Assert.Equal(line1.GetHashCode(), line2.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var line = new Line(new Point(0, 0), new Point(1, 1));

            // Act
            var result = line.ToString();

            // Assert
            Assert.Equal("(0,0), (1,1)", result);
        }
    }
}