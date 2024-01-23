namespace Nineteen.Rectangle.Core
{
    public class Point : IPoint, IEquatable<IPoint>
    {
        public Point() { }

        public Point(int v1, int v2)
        {
            X = v1;
            Y = v2;
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

            if (obj is Point otherPoint)
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
            return $"({X},{Y})";
        }
    }
}
