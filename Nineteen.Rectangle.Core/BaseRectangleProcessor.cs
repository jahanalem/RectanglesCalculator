namespace Nineteen.Rectangle.Core
{
    public abstract class BaseRectangleProcessor
    {
        public List<Point> Points { get; set; }

        public BaseRectangleProcessor(List<Point> points)
        {
            Points = points;
        }

        public List<IRectangle> DeduplicateRectangles(List<IRectangle> potentialRectangles)
        {
            return potentialRectangles.Distinct().ToList();
        }
    }
}
