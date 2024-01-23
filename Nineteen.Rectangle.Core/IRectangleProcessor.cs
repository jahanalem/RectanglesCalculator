namespace Nineteen.Rectangle.Core
{
    public interface IRectangleProcessor
    {
        List<Line> CreateLines(Dictionary<int, List<Point>> pointsGroupedByY);
        List<IRectangle> FindPotentialRectangles(List<Line> lines);
        Dictionary<int, List<Point>> GroupPointsByY(List<Point> points);
        List<IRectangle> Process();
        List<KeyValuePair<int, List<Line>>> GetMatchingXGroups(Dictionary<int, List<Line>> linesGroupedByY, int baseLineX1, int baseLineX2);
    }
}