namespace Nineteen.Rectangle.Core
{
    public class Line : ILine, IEquatable<Line>
    {
        public Line(Point point1, Point point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public Point Point1 { get; set; }
        public Point Point2 { get; set; }

        public bool Equals(Line? other)
        {
            if (other == null)
            {
                return false;
            }

            return Point1.Equals(other.Point1) && Point2.Equals(other.Point2) || Point1.Equals(other.Point2) && Point2.Equals(other.Point1);
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

            if (obj is Line otherLine)
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
            return $"{Point1}, {Point2}";
        }
    }
}
