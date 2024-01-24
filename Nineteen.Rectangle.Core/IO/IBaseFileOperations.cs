
using Nineteen.Rectangle.Core.Models;

namespace Nineteen.Rectangle.Core.IO
{
    public interface IBaseFileOperations
    {
        List<Point> GetPointsFromFile(string filePath);
        List<Point> ReadPointsFromJson(string filePath);
        void SavePointsToFile(List<Point> points, string filePath);
        void SaveRectanglesToJson(List<IRectangle> rectangles, string filePath);
        void SaveResultsToFile(List<IRectangle> rectangles, string filePath);
    }
}