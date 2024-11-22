namespace Nineteen.Rectangle.Core.Models
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
            return ReferenceEquals(this, other) || (other is not null && X == other.X && Y == other.Y);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is Point otherPoint && Equals(otherPoint));
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
