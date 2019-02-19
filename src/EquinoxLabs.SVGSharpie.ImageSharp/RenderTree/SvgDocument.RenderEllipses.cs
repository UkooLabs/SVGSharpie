using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Shapes;
using EquinoxLabs.SVGSharpie.ImageSharp.Shapes;
using EquinoxLabs.SVGSharpie;

namespace EquinoxLabs.SVGSharpie.ImageSharp.Dom
{
    internal sealed partial class SvgDocumentRenderer<TPixel> : SvgElementWalker
        where TPixel : struct, IPixel<TPixel>
    {
        public override void VisitCircleElement(SvgCircleElement element)
        {
            base.VisitCircleElement(element);

            this.RenderShapeToCanvas(element, new EllipsePolygon(element.Cx?.Value ?? 0, element.Cy?.Value ?? 0, element.R.Value.Value));
        }

        public override void VisitEllipseElement(SvgEllipseElement element)
        {
            this.RenderShapeToCanvas(element, new EllipsePolygon(element.Cx?.Value ?? 0, element.Cy?.Value ?? 0, element.Rx.Value.Value * 2, element.Ry.Value.Value * 2));
        }
    }
}
