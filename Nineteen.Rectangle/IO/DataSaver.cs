using Nineteen.Rectangle.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle.ConsoleApp.IO
{
    public class DataSaver : IDataSaver
    {
        public void SavePoints(List<Point> points, string filePath)
        {
            FileOperations.SavePointsToFile(points, filePath);
        }
    }
}
