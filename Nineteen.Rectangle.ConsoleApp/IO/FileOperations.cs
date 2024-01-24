using Nineteen.Rectangle.ConsoleApp.UI;
using Nineteen.Rectangle.Core.IO;
using Nineteen.Rectangle.Core.Models;

namespace Nineteen.Rectangle.ConsoleApp.IO
{
    public class FileOperations : CoreFileOperations
    {
        public override List<Point> GetPointsFromFile(string filePath)
        {
            UserInterface.DataReadingStartMessage(filePath);
            var points = base.GetPointsFromFile(filePath);
            UserInterface.DataReadingCompletedMessage();
            return points;
        }
    }
}
