using ceTe.DynamicPDF;
using Color = EquinoxLabs.SVGSharpie.DynamicPDF.Color;
using PdfColor = ceTe.DynamicPDF.Color;
using PdfCmykColor = ceTe.DynamicPDF.CmykColor;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;
using PdfSpotkColorInk = ceTe.DynamicPDF.SpotColorInk;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    /// <summary>
    /// Provides functionality to convert from the render langue color to the DynamicPdf color structure
    /// </summary>
    internal static class DynamicPdfColorConverter
    {
        public static PdfColor ToPdfColor(Color color, PdfSpotColor spotColorOverride)
        {
            if (spotColorOverride != null)
            {
                return spotColorOverride;
            }
            if (color.SpotColor != null)
            {
                var spotColorName = color.SpotColor.Value.InkName;
                var cmyk = color.CmykColor.GetValueOrDefault();
                var cmykAlternate = new PdfCmykColor(cmyk.C / 100f, cmyk.M / 100f, cmyk.Y / 100f, cmyk.K / 100f);
                return new PdfSpotColor(100, new PdfSpotkColorInk(spotColorName, cmykAlternate));
            }
            if (color.CmykColor != null)
            {
                var cmyk = color.CmykColor.GetValueOrDefault();
                return new PdfCmykColor(cmyk.C / 100f, cmyk.M / 100f, cmyk.Y / 100f, cmyk.K / 100f);
            }
            if (color.RgbColor != null)
            {
                var rgb = color.RgbColor.Value;
                return new ceTe.DynamicPDF.RgbColor(rgb.R, rgb.G, rgb.B);
            }
            return null;
        }
    }
}
