using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SVGSharpie;

namespace SixLabors.Svg.Dom
{
    internal sealed class BrushGenerator<TPixel> : SvgPaintServerVisitor<IBrush<TPixel>>
            where TPixel : struct, IPixel<TPixel>
    {
        public static BrushGenerator<TPixel> Instance { get; } = new BrushGenerator<TPixel>();
        public override IBrush<TPixel> VisitSolidColorPaintServer(SvgSolidColorPaintServer paintServer)
        {
            // lets asume the brush is a color for now
            // TODO update ColourBuilder to expose named colors
            var color = paintServer.Color.As<TPixel>(paintServer.Opacity);
            return new SolidBrush<TPixel>(color);
        }

    }
}
