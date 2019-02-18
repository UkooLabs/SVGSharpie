using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Tests.TestUtilities.ImageComparison;
using Xunit;

namespace EquinoxLabs.SVGSharpie.ImageSharp.Tests
{
    public class UnitTests
    {
        public static TheoryData<string, string, string> SVG11Tests => Utils.GetTestImages(Path.Combine(Utils.TestFolder, "SVG1.1"));
        public static TheoryData<string, string, string> SVG12Tests => Utils.GetTestImages(Path.Combine(Utils.TestFolder, "SVG1.2"));

        [Theory]
        [MemberData(nameof(SVG11Tests))]
        public void SvgTest(string svgFilePath, string pngFilePath, string resultFilePath)
        {
            using (Image<Rgba32> svgImg = SvgImageRenderer.RenderFromString<Rgba32>(File.ReadAllText(svgFilePath)))
            using (var pngImg = Image.Load(pngFilePath))
            {
                svgImg.Save(resultFilePath);
                ImageComparer.Tolerant(perPixelManhattanThreshold: 500).VerifySimilarity(svgImg, pngImg);
            }
        }
    }
}
