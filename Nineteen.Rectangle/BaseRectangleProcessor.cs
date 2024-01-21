using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle
{
    public abstract class BaseRectangleProcessor
    {
        public List<IPoint> Points { get; set; }

        public BaseRectangleProcessor(List<IPoint> points)
        {
            Points = points;
        }

        public List<IRectangle> DeduplicateRectangles(List<IRectangle> potentialRectangles)
        {
            return potentialRectangles.Distinct().ToList();
        }
    }
}
