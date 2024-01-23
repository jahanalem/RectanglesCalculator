using Nineteen.Rectangle.Core;

namespace Nineteen.Rectangle.ConsoleApp.TestData
{
    public static class TestDataGenerator
    {
        private static List<Point> GeneratePoints(int count)
        {
            var random = new Random();
            var points = new List<Point>();
            for (int i = 0; i < count; i++)
            {
                int x = random.Next(1, 1000);
                int y = random.Next(1, 1000);
                points.Add(new Point(x, y));
            }

            return points;
        }
    }
}
