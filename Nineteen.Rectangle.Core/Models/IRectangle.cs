namespace Nineteen.Rectangle.Core.Models
{
    public interface IRectangle
    {
        Line Line1 { get; set; }
        Line Line2 { get; set; }
        IEnumerable<Point> GetOrderedPoints();
    }
}