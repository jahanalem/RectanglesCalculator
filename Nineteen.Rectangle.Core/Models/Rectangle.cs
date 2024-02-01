namespace Nineteen.Rectangle.Core.Models
{
    public class Rectangle : IRectangle, IEquatable<IRectangle>
    {
        private const int HashSeed = 19;
        private const int HashFactor = 31;

        public Rectangle(Line line1, Line line2)
        {
            Line1 = line1;
            Line2 = line2;
        }

        public Line Line1 { get; set; }
        public Line Line2 { get; set; }

        public IEnumerable<Point> GetOrderedPoints()
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
        public override bool Equals(object? obj)
        {
            return Equals(obj as IRectangle);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = HashSeed;
                foreach (var point in GetOrderedPoints())
                {
                    hash = hash * HashFactor + point.GetHashCode();
                }
                return hash;
            }
        }

        public override string ToString()
        {
            return $"[{Line1}, {Line2}]";
        }
    }
}
