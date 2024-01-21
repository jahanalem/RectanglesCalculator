using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle
{
    public class Line : ILine, IEquatable<ILine>
    {
        public Line(IPoint point1, IPoint point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public IPoint Point1 { get; set; }
        public IPoint Point2 { get; set; }

        public bool Equals(ILine? other)
        {
            if (other == null)
            {
                return false;
            }

            return (Point1.Equals(other.Point1) && Point2.Equals(other.Point2)) || (Point1.Equals(other.Point2) && Point2.Equals(other.Point1));
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is ILine otherLine)
            {
                return Equals(otherLine);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hashPoint1 = Point1.GetHashCode();
            int hashPoint2 = Point2.GetHashCode();

            return hashPoint1 ^ hashPoint2;
        }

        public override string ToString()
        {
            return $"line({Point1}, {Point2})";
        }
    }
}
