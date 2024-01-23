namespace Nineteen.Rectangle.Core
{
    public interface IRectangle
    {
        ILine Line1 { get; set; }
        ILine Line2 { get; set; }
        IEnumerable<IPoint> GetOrderedPoints();
    }
}