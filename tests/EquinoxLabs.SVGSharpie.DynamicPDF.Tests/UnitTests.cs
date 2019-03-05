using System;
using System.Diagnostics;
using System.IO;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using Xunit;
using PdfSpotColorInk = ceTe.DynamicPDF.SpotColorInk;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;

namespace EquinoxLabs.SVGSharpie.DynamicPDF.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void QuickTest()
        {
            var svgFilePath = System.IO.Path.Combine(Utils.TestFolder, "Test", "Tiger.svg");
            Debug.WriteLine($"Testing QuickTest: {System.IO.Path.GetFileName(svgFilePath)} in {System.IO.Path.GetDirectoryName(svgFilePath)}");          
            Document document = new Document
            {
                Creator = "PNI",
                Author = "PNI"
            };
            var spotColorInk = new PdfSpotColorInk("MyCol", new ceTe.DynamicPDF.CmykColor(0, 255, 0, 0));
            var spotColor = new PdfSpotColor(1, spotColorInk);
            Page page = new Page( PageSize.Letter, PageOrientation.Portrait, 54.0f );
            var svgDocument = SvgDocument.Parse(File.ReadAllText(svgFilePath));
            var pageElement = SvgImageRenderer.CreateSvgImagePageElement(svgDocument, new Rectangle(0, 0, 300, 300), PNI.Apollo.Render.Services.DynamicPdf.PageElements.HorizontalAlignment.Center, PNI.Apollo.Render.Services.DynamicPdf.PageElements.VerticalAlignment.Center, spotColor);
            page.Elements.Add(pageElement);
            document.Pages.Add(page);
            document.Draw(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(svgFilePath), System.IO.Path.GetFileNameWithoutExtension(svgFilePath) + ".pdf"));
        }
    }
}
