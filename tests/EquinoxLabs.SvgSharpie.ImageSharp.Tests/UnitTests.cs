using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Tests.TestUtilities.ImageComparison;
using Xunit;
using SixLabors.Primitives;
using System.Diagnostics;

namespace EquinoxLabs.SVGSharpie.ImageSharp.Tests
{
    public class UnitTests
    {
        public static TheoryData<string, string, string> SVG11Tests => Utils.GetTestImages(Path.Combine(Utils.TestFolder, "SVG1.1"));
        public static TheoryData<string, string, string> SVG12Tests => Utils.GetTestImages(Path.Combine(Utils.TestFolder, "SVG1.2"));

        [Theory]
        [MemberData(nameof(SVG11Tests))]
        public void SvgTest11(string svgFilePath, string pngFilePath, string resultFilePath)
        {
            Debug.WriteLine($"Testing SVG11: {Path.GetFileName(svgFilePath)} in {Path.GetDirectoryName(svgFilePath)}");
            using (Image<Rgba32> svgImg = SvgImageRenderer.RenderFromString<Rgba32>(File.ReadAllText(svgFilePath)))
            using (var pngImg = Image.Load(pngFilePath))
            using (var result = new Image<Rgba32>(pngImg.Width * 2, pngImg.Height))
            {
                result.Mutate(x => x.DrawImage(pngImg, new Point(0, 0), 1.0f));
                result.Mutate(x => x.DrawImage(svgImg, new Point(pngImg.Width, 0), 1.0f));
                result.Save(resultFilePath);
                //ImageComparer.Tolerant(perPixelManhattanThreshold: 500).VerifySimilarity(svgImg, pngImg);
            }
        }

        [Theory]
        [MemberData(nameof(SVG12Tests))]
        public void SvgTest12(string svgFilePath, string pngFilePath, string resultFilePath)
        {
            Debug.WriteLine($"Testing SVG12: {Path.GetFileName(svgFilePath)} in {Path.GetDirectoryName(svgFilePath)}");
            using (Image<Rgba32> svgImg = SvgImageRenderer.RenderFromString<Rgba32>(File.ReadAllText(svgFilePath)))
            using (var pngImg = Image.Load(pngFilePath))
            using (var result = new Image<Rgba32>(pngImg.Width * 2, pngImg.Height))
            {
                result.Mutate(x => x.DrawImage(pngImg, new Point(0, 0), 1.0f));
                result.Mutate(x => x.DrawImage(svgImg, new Point(pngImg.Width, 0), 1.0f));
                result.Save(resultFilePath);
                //ImageComparer.Tolerant(perPixelManhattanThreshold: 500).VerifySimilarity(svgImg, pngImg);
            }
        }

        [Fact]
        public void QuickTest()
        {
            var svgFilePath = Path.Combine(Utils.TestFolder, "Test", "Test.svg");
            Debug.WriteLine($"Testing QuickTest: {Path.GetFileName(svgFilePath)} in {Path.GetDirectoryName(svgFilePath)}");
            using (Image<Rgba32> svgImg = SvgImageRenderer.RenderFromString<Rgba32>(File.ReadAllText(svgFilePath)))
            {
                svgImg.Save(Path.Combine(Path.GetDirectoryName(svgFilePath), Path.GetFileNameWithoutExtension(svgFilePath) + ".png"));
            }
        }
    }
}
