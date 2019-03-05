using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ceTe.DynamicPDF;
using PNI.Apollo.Render.Services.DynamicPdf.PageElements;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    public static class SvgImageRenderer
    {

        public static PageElement CreateSvgImagePageElement(SvgDocument document, Rectangle bounds, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, PdfSpotColor spotColorInk)
        {
            var svg = document.RootElement;
            var boundsWidth = (float)bounds.Width;
            var boundsHeight = (float)bounds.Height;
            if (svg.WidthAsLength == null || svg.HeightAsLength == null)
            {
                svg.Width = boundsWidth;
                svg.Height = boundsHeight;
            }

            if (svg.WidthAsLength?.LengthType == SvgLengthType.Percentage)
            {
                svg.Width = boundsWidth * (svg.WidthAsLength?.ValueInSpecifiedUnits / 100);
            }

            if (svg.HeightAsLength?.LengthType == SvgLengthType.Percentage)
            {
                svg.Height = boundsHeight * (svg.HeightAsLength?.ValueInSpecifiedUnits / 100);
            }

            var svgElement = new SvgPageElement(document, bounds, horizontalAlignment, verticalAlignment);
            svgElement.SpotColorOveride = spotColorInk;
            var shouldClip = horizontalAlignment == HorizontalAlignment.Stretch || verticalAlignment == VerticalAlignment.Stretch;
            return shouldClip ? (PageElement)new ClippingGroup(bounds, svgElement) : svgElement;
        }
    }
}
