using System;
using System.Drawing;

namespace UkooLabs.SVGSharpie.DynamicPDF.Core
{
    internal sealed class VectorElementPdfPageViewport
    {
        /// <summary>
        /// Gets the horizontal scale factor to apply to the Svg element
        /// </summary>
        public float ScaleX { get; }

        /// <summary>
        /// Gets the vertical scale factor to apply to the Svg element
        /// </summary>
        public float ScaleY { get; }

        /// <summary>
        /// Gets the absolute rectangle into which the svg element will be placed on the page
        /// </summary>
        public RectangleF PagePlacement { get; }

        /// <summary>
        /// Gets the additional x- axis offset to apply to the SVG, in page coordinates
        /// </summary>
        public float OffsetX { get; }

        /// <summary>
        /// Gets the additional y- axis offset to apply to the SVG, in page coordinates
        /// </summary>
        public float OffsetY { get; }

        /// <summary>
        /// Gets a value indicating whether a clipping group is required around the element
        /// </summary>
        public bool ClippingGroupRequired { get; }

        public VectorElementPdfPageViewport(float contentWidth, float contentHeight, float x, float y, float width, float height, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            ScaleX = width / contentWidth;
            ScaleY = height / contentHeight;

            var minScale = Math.Min(ScaleX, ScaleY);

            var stretchHorz = horizontalAlignment == HorizontalAlignment.Stretch;
            var stretchVert = verticalAlignment == VerticalAlignment.Stretch;
            if (!stretchHorz && !stretchVert)
            {
                // scale to fill the container but not overflow it
                ScaleX = ScaleY = minScale;
            }
            else if (stretchHorz && !stretchVert)
            {
                // scaling to fill horizontally, keep aspect so Y factor = X factor
                ScaleY = ScaleX;
            }
            else if (!stretchHorz)
            {
                // scaling to fill vertically, keep aspect so X factor = Y factor
                ScaleX = ScaleY;
            }

            ClippingGroupRequired = !(!stretchVert && !stretchHorz);

            var scaledWidth = ScaleX * contentWidth;
            var scaledHeight = ScaleY * contentHeight;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    OffsetX = (width - scaledWidth) * 0.5f;
                    break;
                case HorizontalAlignment.Right:
                    OffsetX = width - scaledWidth;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    OffsetY = (height - scaledHeight) * 0.5f;
                    break;
                case VerticalAlignment.Bottom:
                    OffsetY = height - scaledHeight;
                    break;
            }

            PagePlacement = new RectangleF(x + OffsetX, y + OffsetY, scaledWidth, scaledHeight);
        }
    }
}