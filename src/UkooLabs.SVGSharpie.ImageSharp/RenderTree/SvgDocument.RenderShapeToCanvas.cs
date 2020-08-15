using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UkooLabs.SVGSharpie.ImageSharp.Shapes;
using UkooLabs.SVGSharpie;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace UkooLabs.SVGSharpie.ImageSharp.Dom
{
    internal sealed partial class SvgDocumentRenderer<TPixel> : SvgElementWalker
            where TPixel : unmanaged, IPixel<TPixel>
    {
        private void RenderShapeToCanvas(SvgGraphicsElement svgGraphicsElement, IPath path)
        {
            var matrix = CalulateUpdatedMatrix(svgGraphicsElement);

            var brush = svgGraphicsElement.CreateFillPaintServer()?.Accept(BrushGenerator<TPixel>.Instance);

            IBrush strokFill = null;
            IPath outline = null;
            if (svgGraphicsElement.StrokeWidth > 0)
            {
                strokFill = svgGraphicsElement.CreateStrokePaintServer()?.Accept(BrushGenerator<TPixel>.Instance);
                if (strokFill != null)
                {
                    var pattern = svgGraphicsElement.Style.StrokeDashArray.Value?.Select(X => X.Value).ToArray();
                    var joint = svgGraphicsElement.Style.StrokeLineJoin.AsJointStyle();
                    var end = svgGraphicsElement.Style.StrokeLineCap.AsEndCapStyle();
                    if (pattern == null || pattern.Length == 0)
                    {
                        outline = path.GenerateOutline(svgGraphicsElement.StrokeWidth, joint, end);
                    }
                    else
                    {
                        outline = path.GenerateOutline(svgGraphicsElement.StrokeWidth,
                            pattern,
                            false,
                            joint, end);
                    }
                }
            }

            var shapeOptions = new ShapeOptions { IntersectionRule = IntersectionRule.Nonzero };
            var shapeGraphicsOptions = new ShapeGraphicsOptions(new GraphicsOptions(), shapeOptions);

            if (brush != null)
            {
                image.Fill(shapeGraphicsOptions, brush, path.Transform(matrix));
            }

            if (outline != null && strokFill != null)
            {
                image.Fill(shapeGraphicsOptions, strokFill, outline.Transform(matrix));
            }
        }
    }
}
