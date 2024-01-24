namespace Nineteen.Rectangle.Core.Models
{
    public class RectangleJsonModel
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public Point Point3 { get; set; }
        public Point Point4 { get; set; }

        public RectangleJsonModel(IRectangle rect)
        {
            var points = rect.GetOrderedPoints().ToList();
            Point1 = points[0];
            Point2 = points[1];
            Point3 = points[2];
            Point4 = points[3];
        }
    }
}
