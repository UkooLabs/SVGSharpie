using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ceTe.DynamicPDF;
using EquinoxLabs.SVGSharpie;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    internal static class SvgEnumConversionExtensions
    {
        public static LineCap ConvertToDynamicPdf(this StyleProperty<SvgStrokeLineCap> strokeLineCap)
            => ConvertToDynamicPdf(strokeLineCap.Value);

        public static LineCap ConvertToDynamicPdf(this SvgStrokeLineCap strokeLineCap)
        {
            switch (strokeLineCap)
            {
                case SvgStrokeLineCap.Butt:
                    return LineCap.Butt;
                case SvgStrokeLineCap.Round:
                    return LineCap.Round;
                case SvgStrokeLineCap.Square:
                    return LineCap.ProjectedSquare;
                case SvgStrokeLineCap.Inherit:
                    throw new Exception("Inherited value can not be resolved");
                default:
                    throw new ArgumentOutOfRangeException(nameof(strokeLineCap), strokeLineCap, null);
            }
        }

        public static LineJoin ConvertToDynamicPdf(this StyleProperty<SvgStrokeLineJoin> strokeLineJoin)
            => ConvertToDynamicPdf(strokeLineJoin.Value);

        public static LineJoin ConvertToDynamicPdf(this SvgStrokeLineJoin strokeLineJoin)
        {
            switch (strokeLineJoin)
            {
                case SvgStrokeLineJoin.Miter:
                    return LineJoin.Miter;
                case SvgStrokeLineJoin.Round:
                    return LineJoin.Round;
                case SvgStrokeLineJoin.Bevel:
                    return LineJoin.Bevel;
                case SvgStrokeLineJoin.Inherit:
                    throw new Exception("Inherited value can not be resolved");
                default:
                    throw new ArgumentOutOfRangeException(nameof(strokeLineJoin), strokeLineJoin, null);
            }
        }
    }
}
