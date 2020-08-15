using ceTe.DynamicPDF.IO;
using ceTe.DynamicPDF.PageElements;

namespace UkooLabs.SVGSharpie.DynamicPDF.Core
{
    /// <summary>
    /// Represents a 'closepath' sub path command and writes the PDF 'h' operator
    /// </summary>
    internal class CloseSubPath : SubPath
    {
        public override void Draw(PageWriter writer) => writer.Write_h();
    }
}
