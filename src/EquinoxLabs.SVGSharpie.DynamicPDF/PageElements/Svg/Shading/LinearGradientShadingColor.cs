using System;
using System.Collections.Generic;
using System.Drawing;

namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements.Shading
{
    internal sealed class LinearGradientShadingColor : GradientShadingColor<LinearGradientShadingColor>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LinearGradientShadingColor"/>
        /// </summary>
        /// <param name="placementRect">the final placement rectangle of the gradient element in PDF page coordinates (inverted Y-axis)</param>
        /// <param name="domainRect">the gradient vector encoded into a rectangle, vector from (top, left) to (bottom, right)</param>
        /// <param name="stops">the color stops</param>
        public LinearGradientShadingColor(RectangleF placementRect, RectangleF domainRect, IEnumerable<GradientColorStop> stops)
            : base(stops)
        {
            _placementRect = placementRect;
            _domainRect = domainRect;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is LinearGradientShadingColor color && Equals(color);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ _placementRect.GetHashCode();
                hashCode = (hashCode * 397) ^ _domainRect.GetHashCode();
                return hashCode;
            }
        }

        protected override ShadingType ShadingType => ShadingType.AxialType2;

        protected override void DrawMatrix(Action<float> writeNumber)
        {
            writeNumber(_placementRect.Width);
            writeNumber(0);

            writeNumber(0);
            writeNumber(-_placementRect.Height);

            writeNumber(_placementRect.X);
            writeNumber(_placementRect.Y);
        }

        protected override void DrawCoordinates(Action<float> writeNumber)
        {
            writeNumber(_domainRect.Left);
            writeNumber(_domainRect.Top);
            writeNumber(_domainRect.Right);
            writeNumber(_domainRect.Bottom);
        }

        public override bool Equals(LinearGradientShadingColor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals((GradientShadingColor)other) && 
                   _placementRect.Equals(other._placementRect) &&
                   _domainRect.Equals(other._domainRect);
        }

        private readonly RectangleF _placementRect;
        private readonly RectangleF _domainRect;
    }
}