using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using SixLabors.Shapes.Text;
using SixLabors.Svg.Shapes;
using SVGSharpie;

namespace SixLabors.Svg.Dom
{
    internal sealed partial class SvgDocumentRenderer<TPixel> : SvgElementWalker
        where TPixel : struct, IPixel<TPixel>
    {
        public static string DefaultFont { get; set; } = "Times New Roman";
        public static string DefaultSansSerifFont { get; set; } = "Arial";
        public static string DefaultSerifFont { get; set; } = "Times New Roman";
        public override void VisitTextElement(SvgTextElement element)
        {
            base.VisitTextElement(element);

            var fonts = SystemFonts.Collection;
            FontFamily family = null;

            foreach (var f in element.Style.FontFamily.Value)
            {
                var fontName = f;
                if (fontName.Equals("sans-serif"))
                {
                    fontName = DefaultSansSerifFont;
                }
                else if (fontName.Equals("serif"))
                {
                    fontName = DefaultSerifFont;
                }

                if (fonts.TryFind(fontName, out family))
                {
                    break;
                }
            }

            if (family == null)
            {
                family = fonts.Find(DefaultFont);
            }

            var fontSize = element.Style.FontSize.Value.Value;
            var origin = new PointF(element.X?.Value ?? 0, element.Y?.Value ?? 0);
            var font = family.CreateFont(fontSize);

            var visitor = new SvgTextSpanTextVisitor();
            element.Accept(visitor);
            var text = visitor.Text;

            // offset by the ascender to account for fonts render origin of top left
            var ascender = ((font.Ascender * font.Size) / (font.EmSize * 72)) * 72;

            var render = new RendererOptions(font, 72, origin - new PointF(0, ascender))
            {
                HorizontalAlignment = element.Style.TextAnchor.Value.AsHorizontalAlignment()
            };

            var glyphs = TextBuilder.GenerateGlyphs(text, render);
            foreach (var p in glyphs)
            {
                this.RenderShapeToCanvas(element, p);
            }
        }

    }


    internal class SvgTextSpanTextVisitor : SvgElementWalker
    {
        private StringBuilder sb = new StringBuilder();

        public string Text => sb.ToString();

        public override void VisitInlineTextSpanElement(SvgInlineTextSpanElement element)
        {
            sb.Append(element.Text);
        }
    }
}
