using ceTe.DynamicPDF;
using ceTe.DynamicPDF.IO;
using ceTe.DynamicPDF.PageElements;

namespace UkooLabs.SVGSharpie.DynamicPDF.Core
{
    /// <summary>
    /// Draws a rectangular clipping group around the child elements.  All children will be clipped to the 
    /// <see cref="Rectangle"/> specified.
    /// </summary>
    internal sealed class ClippingGroup : Group
    {
        /// <summary>
        /// Initializes a new clipping group at the specified <see cref="clippingRectangle"/> with the specified
        /// <see cref="children"/> elements
        /// </summary>
        /// <param name="clippingRectangle">the clipping rectangle to which the child elements will be clipped</param>
        /// <param name="children">the child elements to add to the clipping group</param>
        public ClippingGroup(Rectangle clippingRectangle, params PageElement[] children)
            : this(clippingRectangle.X, clippingRectangle.Y, clippingRectangle.X + clippingRectangle.Width, clippingRectangle.Y + clippingRectangle.Height, children)
        {

        }

        /// <summary>
        /// Initializes a new clipping group with a rectangle defined by the four points and containing 
        /// the specified <see cref="children"/> elements
        /// </summary>
        /// <param name="xMin">top left x-axis coordinate of the clipping rectangle</param>
        /// <param name="yMin">top left y-axis coordinate of the clipping rectangle</param>
        /// <param name="xMax">bottom right x-axis coordinate of the clipping rectangle</param>
        /// <param name="yMax">bottom right y-axis coordinate of the clipping rectangle</param>
        /// <param name="children">the child elements to add to the clipping group</param>
        public ClippingGroup(double xMin, double yMin, double xMax, double yMax, params PageElement[] children)
        {
            _xMin = xMin;
            _yMin = yMin;
            _xMax = xMax;
            _yMax = yMax;
            foreach (var child in children)
            {
                Add(child);
            }
        }

        public override void Draw(PageWriter writer)
        {
            //
            // See "4.4.3 Clipping Path Operators" (pg. 234, PDF Reference 1.7)
            //

            writer.Write_q_();
            
            writer.Write_m_((float)_xMin, (float)_yMin);
            writer.Write_l_((float)_xMax, (float)_yMin);
            writer.Write_l_((float)_xMax, (float)_yMax);
            writer.Write_l_((float)_xMin, (float)_yMax);
            
            writer.Write_W();
            writer.Write_n();
            base.Draw(writer);
            writer.Write_Q();
        }

        private readonly double _xMin;
        private readonly double _yMin;
        private readonly double _xMax;
        private readonly double _yMax;
    }
}
