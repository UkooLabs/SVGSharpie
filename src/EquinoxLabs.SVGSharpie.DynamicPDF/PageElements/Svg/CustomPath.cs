using ceTe.DynamicPDF;
using ceTe.DynamicPDF.IO;
using ceTe.DynamicPDF.PageElements;

namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements
{
    /// <summary>
    /// Customized PDF path renderer, the base <see cref="Path"/> doesn't support fill rules other than non-zero
    /// </summary>
    internal sealed class CustomPath : Path
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use the even-odd file rule, if false non-zero will be used
        /// </summary>
        public bool FillRuleEvenOdd { get; set; }

        public CustomPath(float x, float y, Color lineColor, Color fillColor, float lineWidth, LineStyle lineStyle, bool closePath)
            : base(x, y, lineColor, fillColor, lineWidth, lineStyle, closePath)
        {
        }

        public override void Draw(PageWriter writer)
        {
            PrepareDrawState(writer, out var hasFill, out var hasStroke);
            var lastCommandIsClose = DrawSubPathsUntilLastClose(writer);
            Paint(writer, ClosePath || lastCommandIsClose, FillRuleEvenOdd, hasFill, hasStroke);
        }

        private void PrepareDrawState(PageWriter writer, out bool fill, out bool stroke)
        {
            if (FillColor != null || LineColor != null)
            {
                writer.SetGraphicsMode();
            }
            if (LineWidth > 0 && FillColor != null)
            {
                writer.SetLineWidth(LineWidth);
                writer.SetStrokeColor(LineColor);
                writer.SetLineStyle(LineStyle);
                writer.SetFillColor(FillColor);
                fill = stroke = true;
            }
            else if (LineWidth > 0)
            {
                writer.SetLineWidth(LineWidth);
                writer.SetStrokeColor(LineColor);
                writer.SetLineStyle(LineStyle);
                stroke = true;
                fill = false;
            }
            else if (FillColor != null)
            {
                writer.SetFillColor(FillColor);
                fill = true;
                stroke = false;
            }
            else
            {
                fill = stroke = false;
                return;
            }
            if (LineStyle == LineStyle.None)
            {
                stroke = false;
            }
            writer.SetLineCap(LineCap);
            writer.SetLineJoin(LineJoin);
            writer.SetMiterLimit(MiterLimit);
        }

        private static void Paint(PageWriter writer, bool close, bool ruleEvenOdd, bool fill, bool stroke)
        {
            if (ruleEvenOdd)
            {
                PaintEvenOdd(writer, close, fill, stroke);
            }
            else
            {
                PaintNonZero(writer, close, fill, stroke);
            }
        }

        private static void PaintNonZero(PageWriter writer, bool close, bool fill, bool stroke)
        {
            if (close)
            {
                if (fill)
                {
                    if (stroke)
                    {
                        writer.Write_b_();
                    }
                    else
                    {
                        writer.Write_f();
                    }
                }
                else if (stroke)
                {
                    writer.Write_s_();
                }
            }
            else if (fill)
            {
                if (stroke)
                {
                    writer.Write_B();
                }
                else
                {
                    writer.Write_f();
                }
            }
            else if (stroke)
            {
                writer.Write_S();
            }
        }

        private static void PaintEvenOdd(PageWriter writer, bool close, bool fill, bool stroke)
        {
            if (close)
            {
                if (fill)
                {
                    if (stroke)
                    {
                        writer.Write_bx_();
                    }
                    else
                    {
                        writer.Write_fx();
                    }
                }
                else if (stroke)
                {
                    writer.Write_s_();
                }
            }
            else if (fill)
            {
                if (stroke)
                {
                    writer.Write_Bx();
                }
                else
                {
                    writer.Write_fx();
                }
            }
            else if (stroke)
            {
                writer.Write_S();
            }
        }

        /// <summary>
        /// Draws subpaths until the last close command, returns true if the last sub path was a close command.
        /// </summary>
        private bool DrawSubPathsUntilLastClose(PageWriter writer)
        {
            writer.Write_m_(X, Y);
            for (var i = 0; i < SubPaths.Count; i++)
            {
                var subPath = SubPaths[i];
                if (subPath is CloseSubPath && i == SubPaths.Count - 1)
                {
                    return true;
                }
                subPath.Draw(writer);
            }
            return false;
        }
    }
}
