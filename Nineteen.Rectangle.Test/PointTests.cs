using Nineteen.Rectangle.Core.Models;

namespace Nineteen.Rectangle.Test
{
    public class PointTests
    {
        [Fact]
        public void Constructor_SetsCoordinates()
        {
            // Arrange & Act
            var point = new Point(1, 2);

            // Assert
            Assert.Equal(1, point.X);
            Assert.Equal(2, point.Y);
        }

        [Fact]
        public void Equals_ReturnsTrueForSameCoordinates()
        {
            // Arrange
            var point1 = new Point(1, 2);
            var point2 = new Point(1, 2);

            // Act & Assert
            Assert.True(point1.Equals(point2));
        }

        [Fact]
        public void Equals_ReturnsFalseForDifferentCoordinates()
        {
            // Arrange
            var point1 = new Point(1, 2);
            var point2 = new Point(3, 4);

            // Act & Assert
            Assert.False(point1.Equals(point2));
        }

        [Fact]
        public void GetHashCode_ReturnsSameHashCodeForSameCoordinates()
        {
            // Arrange
            var point1 = new Point(1, 2);
            var point2 = new Point(1, 2);

            // Act & Assert
            Assert.Equal(point1.GetHashCode(), point2.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var point = new Point(1, 2);

            // Act
            var result = point.ToString();

            // Assert
            Assert.Equal("(1,2)", result);
        }
    }
}
