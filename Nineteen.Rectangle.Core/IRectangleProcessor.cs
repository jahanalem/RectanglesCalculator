namespace Nineteen.Rectangle.Core
{
    public interface IRectangleProcessor
    {
        List<ILine> CreateLines(Dictionary<int, List<IPoint>> pointsGroupedByY);
        List<IRectangle> FindPotentialRectangles(List<ILine> lines);
        Dictionary<int, List<IPoint>> GroupPointsByY(List<IPoint> points);
        List<IRectangle> Process();
        List<KeyValuePair<int, List<ILine>>> GetMatchingXGroups(Dictionary<int, List<ILine>> linesGroupedByY, int baseLineX1, int baseLineX2);
    }
}