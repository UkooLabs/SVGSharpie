using System.Linq;
using System.Numerics;
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
        private void RenderShapeToCanvas(SvgGraphicsElement svgGraphicsElement, IPath path)
        {
            var matrix = CalulateUpdatedMatrix(svgGraphicsElement);

            var brush = svgGraphicsElement.CreateFillPaintServer()?.Accept(BrushGenerator<TPixel>.Instance);

            IBrush<TPixel> strokFill = null;
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

            if (brush != null)
            {
                image.Fill(brush, path.Transform(matrix));
            }

            if (outline != null && strokFill != null)
            {
                image.Fill( strokFill, outline.Transform(matrix));
            }
        }
    }
}
