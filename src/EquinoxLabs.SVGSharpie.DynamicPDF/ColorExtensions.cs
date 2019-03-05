using ceTe.DynamicPDF;
using Color = EquinoxLabs.SVGSharpie.DynamicPDF.Color;
using PdfColor = ceTe.DynamicPDF.Color;
using PdfCmykColor = ceTe.DynamicPDF.CmykColor;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;
using PdfSpotkColorInk = ceTe.DynamicPDF.SpotColorInk;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    internal static class ColorExtensions
    {
        public static PdfColor ToPdfColor(this Color color, PdfSpotColor spotColorInk)
        {
           return  DynamicPdfColorConverter.ToPdfColor(color, spotColorInk);
        }

        public static PdfColor ToPdfColor(this SvgColor color, PdfSpotColor spotColorOverride)
        {
            if (spotColorOverride != null)
            {
                return spotColorOverride;
            }
            return new ceTe.DynamicPDF.RgbColor(color.R, color.G, color.B);
        }
    }
}
