using System;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.IO;
using ceTe.DynamicPDF.PageElements;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;

namespace EquinoxLabs.SVGSharpie.DynamicPDF.Core
{
    /// <summary>
    /// Represents a DynamicPdf graphical <see cref="PageElement"/> where the 
    /// content is an <see cref="SvgDocument"/>.
    /// </summary>
    internal sealed class SvgPageElement : PageElement
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public HorizontalAlignment ContentHorizontalAlignment { get; set; }

        public VerticalAlignment ContentVerticalAlignment { get; set; }

        public SvgDocument SvgDocument { get; set; }

        public PdfSpotColor SpotColorOveride { get; set; }

        public SvgPageElement(SvgDocument svgDocument, Rectangle bounds, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, VerticalAlignment verticalAlignment = VerticalAlignment.Center)
            : this(svgDocument, (float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height, horizontalAlignment, verticalAlignment)
        {
        }

        public SvgPageElement(SvgDocument svgDocument, float x, float y, float width, float height, HorizontalAlignment contentHorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment contentVerticalAlignment = VerticalAlignment.Center)
        {
            SvgDocument = svgDocument ?? throw new ArgumentNullException(nameof(svgDocument));
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ContentHorizontalAlignment = contentHorizontalAlignment;
            ContentVerticalAlignment = contentVerticalAlignment;
        }

        public override void Draw(PageWriter writer)
        {
            var svg = SvgDocument.RootElement;
            if (svg == null)
            {
                return;
            }

            var svgWidth = svg.ViewBox?.Width ?? (svg.Width ?? 0);
            var svgHeight = svg.ViewBox?.Height ?? (svg.Height ?? Height);   // as per spec height defaults to 100%
            if (svgWidth <= 0 || svgHeight <= 0)
            {
                return;
            }

            var viewport = new VectorElementPdfPageViewport(svgWidth, svgHeight, X, Y, Width, Height, ContentHorizontalAlignment, ContentVerticalAlignment);
            var pagePlacement = viewport.PagePlacement;
            var container = new TransformationGroup(pagePlacement.X, pagePlacement.Y, Width, Height)
            {
                ScaleX = viewport.ScaleX,
                ScaleY = viewport.ScaleY
            };

            var pageHeight = writer.Page.Dimensions.Height;

            container.Add(svg.Accept(new SvgElementToDynamicPdfElementConverter(svg, viewport, pageHeight, SpotColorOveride)));
            container.Draw(writer);
        }
    }
}
