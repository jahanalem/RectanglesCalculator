using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle.Core
{
    public class Rectangle : IRectangle, IEquatable<IRectangle>
    {
        public Rectangle(ILine line1, ILine line2)
        {
            Line1 = line1;
            Line2 = line2;
        }

        public ILine Line1 { get; set; }
        public ILine Line2 { get; set; }

        public IEnumerable<IPoint> GetOrderedPoints()
        {
            return new[] { Line1.Point1, Line1.Point2, Line2.Point1, Line2.Point2 }
                .OrderBy(p => p.X)
                .ThenBy(p => p.Y);
        }

        public bool Equals(IRectangle? other)
        {
            if (other == null)
            {
                return false;
            }

            return GetOrderedPoints().SequenceEqual(other.GetOrderedPoints());
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as IRectangle);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (var point in GetOrderedPoints())
                {
                    hash = hash * 31 + point.GetHashCode();
                }
                return hash;
            }
        }

        public override string ToString()
        {
            return $"Rectangle: [{Line1}, {Line2}]";
        }
    }
}
