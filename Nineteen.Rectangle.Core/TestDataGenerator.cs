﻿namespace Nineteen.Rectangle.Core
{
    public static class TestDataGenerator
    {
        public static List<Point> GeneratePoints(
            int count,
            int maxX = 100,
            int maxY = 100,
            IDataSaver dataSaver = null)
        {
            var random = new Random();
            var points = new HashSet<Point>();
            while (points.Count < count)
            {
                int x = random.Next(1, maxX + 1);
                int y = random.Next(1, maxY + 1);
                points.Add(new Point(x, y));
            }

            var pointList = points.ToList();

            if (dataSaver != null)
            {
                string filePath = "generated_points.json";
                dataSaver.SavePoints(pointList, filePath);
            }

            return pointList;
        }
    }
}
