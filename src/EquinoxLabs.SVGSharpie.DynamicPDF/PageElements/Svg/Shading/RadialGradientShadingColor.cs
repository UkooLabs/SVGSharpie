using System;
using System.Collections.Generic;
using System.Drawing;

namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements.Shading
{
    internal sealed class RadialGradientShadingColor : GradientShadingColor<RadialGradientShadingColor>
    {
        private readonly RectangleF _placementRect;
        private readonly Circle _circle0;
        private readonly Circle _circle1;
        
        public RadialGradientShadingColor(RectangleF placementRect, Circle circle0, Circle circle1, IEnumerable<GradientColorStop> stops) : base(stops)
        {
            _placementRect = placementRect;
            _circle0 = circle0;
            _circle1 = circle1;
        }

        public override bool Equals(RadialGradientShadingColor other)
        {
            if (ReferenceEquals(other, null)) return false;
            return _placementRect.Equals(other._placementRect) && 
                   _circle0 == other._circle0 &&
                   _circle1 == other._circle1;
        }

        protected override ShadingType ShadingType => ShadingType.RadialType3;
        
        /// <remarks>
        /// Writes an array of six numbers specifying the radii of the starting and ending circles
        /// </remarks>
        protected override void DrawCoordinates(Action<float> writeNumber)
        {
            writeNumber(_circle0.X);
            writeNumber(_circle0.Y);
            writeNumber(_circle0.R);
            writeNumber(_circle1.X);
            writeNumber(_circle1.Y);
            writeNumber(_circle1.R);
        }

        protected override void DrawMatrix(Action<float> writeNumber)
        {
            writeNumber(_placementRect.Width);
            writeNumber(0);

            writeNumber(0);
            writeNumber(-_placementRect.Height);

            writeNumber(_placementRect.X);
            writeNumber(_placementRect.Y);
        }
    }
}