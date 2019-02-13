using System;
using EquinoxLabs.SVGSharpie.ImageSharp.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EquinoxLabs.SVGSharpie.ImageSharp
{
    public static class SvgImageRenderer
    {

        public static Image<TPixel> RenderFromString<TPixel>(string content) where TPixel : struct, IPixel<TPixel>
        {
            var document = SvgDocument.Parse(content);
            return RenderInner<TPixel>(document, null, null);
        }

        public static Image<TPixel> RenderFromString<TPixel>(string content, int width, int height) where TPixel : struct, IPixel<TPixel>
        {
            var document = SvgDocument.Parse(content);
            return RenderInner<TPixel>(document, width, height);
        }

        public static Image<TPixel> RenderFromDocument<TPixel>(SvgDocument document) where TPixel : struct, IPixel<TPixel>
        {
            return RenderInner<TPixel>(document, null, null);
        }

        public static Image<TPixel> RenderFromDocument<TPixel>(SvgDocument document, int width, int height) where TPixel : struct, IPixel<TPixel>
        {
            return RenderInner<TPixel>(document, width, height);
        }

        private static Image<TPixel> RenderInner<TPixel>(SvgDocument document, int? targetWidth, int? targetHeight) where TPixel : struct, IPixel<TPixel>
        {
            float? width = targetWidth ?? document.RootElement.Width ?? document.RootElement.ViewWidth;
            float? height = targetHeight ?? document.RootElement.Height ?? document.RootElement.ViewWidth;

            if (!width.HasValue || !height.HasValue)
            {
                throw new Exception("Svg does not specify a size set one.");
            }

            var image = new Image<TPixel>((int)Math.Ceiling(width.Value), (int)Math.Ceiling(height.Value));

            image.Mutate(x =>
            {
                var renderer = new SvgDocumentRenderer<TPixel>(image.Size(), x);
                document.RootElement.Accept(renderer);
            });
            return image;
        }
    }
}
