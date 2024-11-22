using Nineteen.Rectangle.Core.Models;
using Nineteen.Rectangle.Core.Processors;

namespace Nineteen.Rectangle.ConsoleApp.Processing
{
    public static class DataProcessor
    {
        public static List<IRectangle> ProcessData(List<Point> points)
        {
            var rectangleProcessor = new RectangleProcessorParallel(points);
            //var rectangleProcessor = new RectangleProcessor(points);

            return rectangleProcessor.Process();
        }
    }
}
