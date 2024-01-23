using Nineteen.Rectangle.ConsoleApp.UI;
using Nineteen.Rectangle.Core;
using System.Text.Json;

namespace Nineteen.Rectangle.ConsoleApp.IO
{
    public static class FileOperations
    {
        public static List<Point> GetPointsFromFile(string filePath)
        {
            UserInterface.DataReadingStartMessage(filePath);

            try
            {
                var points = ReadPointsFromJson(filePath);
                UserInterface.DataReadingCompletedMessage();

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

        static List<Point> ReadPointsFromJson(string filePath)
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<Point>>(json) ?? new List<Point>();
        }

        public static void SaveResultsToFile(List<IRectangle> rectangles, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var rectangle in rectangles)
                {
                    writer.WriteLine(rectangle.ToString());
                }
            }
        }
    }
}
