using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UkooLabs.SVGSharpie;
using SixLabors.ImageSharp.Drawing.Processing;

namespace UkooLabs.SVGSharpie.ImageSharp.Dom
{
    internal sealed class BrushGenerator<TPixel> : SvgPaintServerVisitor<IBrush>
            where TPixel : unmanaged, IPixel<TPixel>
    {
        public static BrushGenerator<TPixel> Instance { get; } = new BrushGenerator<TPixel>();
        public override IBrush VisitSolidColorPaintServer(SvgSolidColorPaintServer paintServer)
        {
            // lets asume the brush is a color for now
            // TODO update ColourBuilder to expose named colors
            var color = paintServer.Color.As<Rgba32>(paintServer.Opacity);
            return new SolidBrush(color);
        }

    }
}
