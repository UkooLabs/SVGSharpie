using PdfColor = ceTe.DynamicPDF.Color;
using PdfRgbColor = ceTe.DynamicPDF.RgbColor;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;

namespace EquinoxLabs.SVGSharpie.DynamicPDF.Extensions
{
    internal static class ColorExtensions
    {
        public static PdfColor ToPdfColor(this SvgColor color, PdfSpotColor spotColorOverride)
        {
            if (spotColorOverride != null)
            {
                return spotColorOverride;
            }
            return new PdfRgbColor(color.R, color.G, color.B);
        }
    }
}
