using System;
using EquinoxLabs.SVGSharpie.ImageSharp.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EquinoxLabs.SVGSharpie.ImageSharp
{
    /// <summary>
    /// SVG image renderer
    /// </summary>
    public static class SvgImageRenderer
    {
        /// <summary>
        /// Render SVG from xml string
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Image<TPixel> RenderFromString<TPixel>(string content) where TPixel : struct, IPixel<TPixel>
        {
            var document = SvgDocument.Parse(content);
            return RenderInner<TPixel>(document, null, null);
        }

        /// <summary>
        /// Render SVG from xml string
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="content"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image<TPixel> RenderFromString<TPixel>(string content, int width, int height) where TPixel : struct, IPixel<TPixel>
        {
            var document = SvgDocument.Parse(content);
            return RenderInner<TPixel>(document, width, height);
        }

        /// <summary>
        /// Render SVG from SvgDocument
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Image<TPixel> RenderFromDocument<TPixel>(SvgDocument document) where TPixel : struct, IPixel<TPixel>
        {
            return RenderInner<TPixel>(document, null, null);
        }

        /// <summary>
        /// Render SVG from SvgDocument
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="document"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image<TPixel> RenderFromDocument<TPixel>(SvgDocument document, int width, int height) where TPixel : struct, IPixel<TPixel>
        {
            return RenderInner<TPixel>(document, width, height);
        }

        private static Image<TPixel> RenderInner<TPixel>(SvgDocument document, int? targetWidth, int? targetHeight) where TPixel : struct, IPixel<TPixel>
        {
            float? width = targetWidth ?? document.RootElement.ViewWidth;
            float? height = targetHeight ?? document.RootElement.ViewWidth;

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
