using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nineteen.Rectangle
{
    public class Point : IPoint, IEquatable<IPoint>
    {
        public Point(int v1, int v2)
        {
            this.X = v1;
            this.Y = v2;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public bool Equals(IPoint? other)
        {
            if (other == null)
            {
                return false;
            }

            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is IPoint otherPoint)
            {
                return Equals(otherPoint);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return $"point({this.X},{this.Y})";
        }
    }
}
