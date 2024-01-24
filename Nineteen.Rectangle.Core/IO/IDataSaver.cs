using Nineteen.Rectangle.Core.Models;

namespace Nineteen.Rectangle.Core.IO
{
    public interface IDataSaver
    {
        void SavePoints(List<Point> points, string filePath);
    }
}
