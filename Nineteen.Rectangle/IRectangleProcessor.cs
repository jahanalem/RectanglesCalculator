
namespace Nineteen.Rectangle
{
    public interface IRectangleProcessor
    {
        List<ILine> CreateLines(Dictionary<int, List<IPoint>> pointsGroupedByY);
        List<IRectangle> FindPotentialRectangles(List<ILine> lines);
        Dictionary<int, List<IPoint>> GroupPointsByY(List<IPoint> points);
        List<IRectangle> Process();
    }
}