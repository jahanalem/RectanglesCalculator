namespace Nineteen.Rectangle.Core.Models
{
    public class Rectangle : IRectangle, IEquatable<IRectangle>
    {
        private const int HashSeed = 19;
        private const int HashFactor = 31;

        private Point[]? _cachedOrderedPoints;
        private int? _cachedHashCode;
        private string? _cachedString;

        public Rectangle(Line line1, Line line2)
        {
            Line1 = line1;
            Line2 = line2;
        }

        public Line Line1 { get; set; }
        public Line Line2 { get; set; }

        public IEnumerable<Point> GetOrderedPoints()
        {
            if (_cachedOrderedPoints != null)
                return _cachedOrderedPoints;

            _cachedOrderedPoints = new[] { Line1.Point1, Line1.Point2, Line2.Point1, Line2.Point2 };
            Array.Sort(_cachedOrderedPoints, static (p1, p2) => (p1.X, p1.Y).CompareTo((p2.X, p2.Y)));

            return _cachedOrderedPoints;
        }


        public bool Equals(IRectangle? other)
        {
            if (other == null)
            {
                return false;
            }

            var thisPoints = GetOrderedPoints();
            var otherPoints = other.GetOrderedPoints();

            return thisPoints.SequenceEqual(otherPoints);
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as IRectangle);
        }

        public override int GetHashCode()
        {
            if (_cachedHashCode.HasValue)
                return _cachedHashCode.Value;

            unchecked
            {
                int hash = HashSeed;
                foreach (var point in GetOrderedPoints())
                {
                    hash = hash * HashFactor + point.GetHashCode();
                }
                _cachedHashCode = hash;

                return hash;
            }
        }

        public override string ToString()
        {
            return _cachedString ??= $"[{Line1}, {Line2}]";
        }
    }
}
