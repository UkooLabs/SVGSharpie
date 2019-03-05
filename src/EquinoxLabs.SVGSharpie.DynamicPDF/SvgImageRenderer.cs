using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ceTe.DynamicPDF;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    public static class SvgImageRenderer
    {
        public static void RenderFromString(Page page, string content)
        {
            var document = SvgDocument.Parse(content);
            RenderInner(page, document, null, null);
        }

        public static void RenderFromString(Page page, string content, int width, int height)
        {
            var document = SvgDocument.Parse(content);
            RenderInner(page, document, width, height);
        }

        public static void RenderFromDocument(Page page, SvgDocument document)
        {
            RenderInner(page, document, null, null);
        }

        public static void RenderFromDocument(Page page, SvgDocument document, int width, int height)
        {
            RenderInner(page, document, width, height);
        }

        private static void RenderInner(Page page, SvgDocument document, int? targetWidth, int? targetHeight)
        {
            //float? width = targetWidth ?? document.RootElement.ViewWidth;
            //float? height = targetHeight ?? document.RootElement.ViewHeight;

            //if (!width.HasValue || !height.HasValue)
            //{
            //    throw new Exception("Svg does not specify a size set one.");
            //}

            //var image = new Image<TPixel>((int)Math.Ceiling(width.Value), (int)Math.Ceiling(height.Value));

            //image.Mutate(x =>
            //{
            //    var renderer = new SvgDocumentRenderer<TPixel>(image.Size(), x);
            //    document.RootElement.Accept(renderer);
            //});
            //return image;
        }
    }
}
