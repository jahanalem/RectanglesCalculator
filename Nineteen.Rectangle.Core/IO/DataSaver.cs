using Nineteen.Rectangle.Core.Models;

namespace Nineteen.Rectangle.Core.IO
{
    public class DataSaver : IDataSaver
    {
        public virtual void SavePoints(List<Point> points, string filePath)
        {
            var fileOperation = new CoreFileOperations();
            fileOperation.SavePointsToFile(points, filePath);
        }
    }
}
