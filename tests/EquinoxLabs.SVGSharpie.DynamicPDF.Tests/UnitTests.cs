using System;
using System.Diagnostics;
using System.IO;
using ceTe.DynamicPDF;
using Xunit;

namespace EquinoxLabs.SVGSharpie.DynamicPDF.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void QuickTest()
        {
            var svgFilePath = Path.Combine(Utils.TestFolder, "Test", "Test.svg");
            Debug.WriteLine($"Testing QuickTest: {Path.GetFileName(svgFilePath)} in {Path.GetDirectoryName(svgFilePath)}");
            var page = new Page();
            SvgImageRenderer.RenderFromString(page, File.ReadAllText(svgFilePath));            
            //Path.Combine(Path.GetDirectoryName(svgFilePath), Path.GetFileNameWithoutExtension(svgFilePath) + ".pdf");            
        }
    }
}
