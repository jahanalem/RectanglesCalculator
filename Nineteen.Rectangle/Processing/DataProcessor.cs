using Nineteen.Rectangle.Core;

namespace Nineteen.Rectangle.ConsoleApp.Processing
{
    public static class DataProcessor
    {
        public static List<IRectangle> ProcessData(List<Point> points)
        {
            var rectangleProcessor = new RectangleProcessorParallel(points);

            return rectangleProcessor.Process();
        }
    }
}
