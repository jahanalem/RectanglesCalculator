using System.Text.Json;
using Nineteen.Rectangle.Core.Models;

namespace Nineteen.Rectangle.Core.IO
{
    public class CoreFileOperations : IBaseFileOperations
    {
        public virtual List<Point> GetPointsFromFile(string filePath)
        {
            try
            {
                var points = ReadPointsFromJson(filePath);
                return points;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON data: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
#if DEBUG
            Console.WriteLine(ex.StackTrace);
#endif
            }

            Console.WriteLine("Returning default values (empty list).");
            return new List<Point>();
        }

        public virtual List<Point> ReadPointsFromJson(string filePath)
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<Point>>(json) ?? new List<Point>();
        }

        public virtual void SaveResultsToFile(List<IRectangle> rectangles, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var rectangle in rectangles)
                {
                    writer.WriteLine(rectangle.ToString());
                }
            }
        }

        public virtual void SavePointsToFile(List<Point> points, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(points, options);
            File.WriteAllText(filePath, json);
        }

        public virtual void SaveRectanglesToJson(List<IRectangle> rectangles, string filePath)
        {
            var rectangleModels = rectangles.Select(rect => new RectangleJsonModel(rect)).ToList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(rectangleModels, options);
            File.WriteAllText(filePath, json);
        }
    }
}
