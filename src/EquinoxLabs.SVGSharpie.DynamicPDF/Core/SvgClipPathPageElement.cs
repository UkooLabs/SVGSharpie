using System;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.IO;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;

namespace EquinoxLabs.SVGSharpie.DynamicPDF.Core
{
    internal sealed class SvgClipPathPageElement : PageElement
    {
        private readonly PdfSpotColor _spotColorInk;

        public SvgClipPathPageElement(PageElement child, SvgClipPathElement clipPath, SvgMatrix clipPathParentTransform, PdfSpotColor spotColorInk)
        {
            _spotColorInk = spotColorInk;
            _child = child ?? throw new ArgumentNullException(nameof(child));
            _clipPath = clipPath ?? throw new ArgumentNullException(nameof(clipPath));
            _clipPathParentTransform = clipPathParentTransform ?? throw new ArgumentNullException(nameof(clipPathParentTransform));
        }

        public override void Draw(PageWriter writer)
        {
            if (_clipPath.Children.Count == 0)
            {
                // clipPath defines the area to be rendered, no children, nothing to render
                return;
            }

            writer.Write_q_();
            {
                var clippingAreaWriter = new SvgClipPathMaskWriterVisitor(writer, _clipPathParentTransform, _spotColorInk);
                _clipPath.Accept(clippingAreaWriter);
                if (clippingAreaWriter.ClippingAreaPainted)
                {
                    _child.Draw(writer);
                }
            }
            writer.Write_Q();
        }

        private readonly PageElement _child;
        private readonly SvgClipPathElement _clipPath;
        private readonly SvgMatrix _clipPathParentTransform;
    }
}
