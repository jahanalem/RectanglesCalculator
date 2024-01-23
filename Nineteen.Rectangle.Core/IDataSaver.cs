using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle.Core
{
    public interface IDataSaver
    {
        void SavePoints(List<Point> points, string filePath);
    }
}
