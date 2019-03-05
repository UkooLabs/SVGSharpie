using ceTe.DynamicPDF.IO;
using ceTe.DynamicPDF.PageElements;

namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements
{
    /// <summary>
    /// Represents a 'moveto' sub path command, when added to a <see cref="T:ceTe.DynamicPDF.PageElements.Path" />, is used to 
    /// move the pen to the specified location.
    /// </summary>
    internal sealed class MoveSubPath : SubPath
    {
        public float X { get; set; }

        public float Y { get; set; }

        public MoveSubPath(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override void Draw(PageWriter writer)
        {
            writer.Write_m_(X, Y);
        }
    }
}
