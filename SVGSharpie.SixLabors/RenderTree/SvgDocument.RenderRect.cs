using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Shapes;
using SixLabors.Svg.Shapes;
using SVGSharpie;

namespace SixLabors.Svg.Dom
{
    internal sealed partial class SvgDocumentRenderer<TPixel> : SvgElementWalker
        where TPixel : struct, IPixel<TPixel>
    {
        public override void VisitRectElement(SvgRectElement element)
        {
            // rect render visitor here???
            base.VisitRectElement(element);
            var x = element.X?.Value ?? 0;
            var y = element.Y?.Value ?? 0;
            var width = element.Width?.Value ?? 0;
            var height = element.Height?.Value ?? 0;
            var rx = element.RadiusX;
            var ry = element.RadiusY;

            var rect = new RoundedRect(x, y, width, height, rx, ry);
            this.RenderShapeToCanvas(element, rect);
        }
    }
}
