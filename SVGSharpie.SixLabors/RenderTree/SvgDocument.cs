using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using SixLabors.Svg.Shapes;
using SVGSharpie;

namespace SixLabors.Svg.Dom
{
    internal sealed partial class SvgDocumentRenderer<TPixel> : SvgElementWalker
        where TPixel : struct, IPixel<TPixel>
    {
        private Matrix3x2 activeMatrix = Matrix3x2.Identity;
        private readonly Vector2 size;
        private readonly IImageProcessingContext<TPixel> image;

        public SvgDocumentRenderer(SizeF size, IImageProcessingContext<TPixel> image)
        {
            this.size = size;
            this.image = image;
        }

        public override void VisitSvgElement(SvgSvgElement element)
        {
            activeMatrix = element.CalculateViewboxFit(size.X, size.Y).AsMatrix3x2();

            base.VisitSvgElement(element);
        }

        private Matrix3x2 CalulateUpdatedMatrix(SvgGraphicsElement elm)
        {
            var matrix = activeMatrix;
            foreach (var t in elm.Transform)
            {
                matrix = matrix * t.Matrix.AsMatrix3x2();
            }
            return matrix;
        }

        public override void VisitGElement(SvgGElement element)
        {
            var oldMatrix = activeMatrix;
            activeMatrix = CalulateUpdatedMatrix(element);
            base.VisitGElement(element);

            activeMatrix = oldMatrix;
        }
    }
}
