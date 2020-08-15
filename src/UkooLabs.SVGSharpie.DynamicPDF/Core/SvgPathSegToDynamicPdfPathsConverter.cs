using System;
using System.Collections.Generic;
using System.Linq;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;
using UkooLabs.SVGSharpie.DynamicPDF.Extensions;

namespace UkooLabs.SVGSharpie.DynamicPDF.Core
{
    /// <summary>
    /// Transforms path segments composed only of <see cref="SvgPathSegCurvetoCubicAbs"/>, <see cref="SvgPathSegLinetoAbs"/> 
    /// and <see cref="SvgPathSegMovetoAbs"/> commands into DynamicPdf <see cref="Path"/> and <see cref="SubPath"/>.  All other
    /// path segment commands will throw an exception.
    /// </summary>
    internal sealed class SvgPathSegToDynamicPdfPathsConverter : SvgPathSegVisitor
    {
        private readonly PdfSpotColor _spotColorOverride;

        public List<Path> Paths { get; } = new List<Path>();

        /// <summary>
        /// Initializes a new instance of <see cref="SvgPathSegToDynamicPdfPathsConverter"/> using the specified <see cref="graphicsElement"/> 
        /// for the styling properties of the resulting paths
        /// </summary>
        public SvgPathSegToDynamicPdfPathsConverter(SvgGraphicsElement graphicsElement, VectorElementPdfPageViewport pageViewport, float pageHeight, PdfSpotColor spotColorOverride)
        {
            _spotColorOverride = spotColorOverride;
            _pageViewport = pageViewport;
            _pageHeight = pageHeight;
            SetFillStyle(graphicsElement);
            SetStrokeStyle(graphicsElement);
        }

        public void Reset()
        {
            Paths.Clear();
            _currentPath = null;
        }

        public void SetFillStyle(SvgGraphicsElement graphicsElement)
        {
            var style = graphicsElement?.Style;
            _fillEvenOdd = style?.FillRule == SvgFillRule.EvenOdd;
            _fillColor = graphicsElement != null && _pageViewport != null
                ? SvgPaintServerToDynamicPdfColorConverter.ConvertToColor(graphicsElement.CreateFillPaintServer(), _pageViewport, _pageHeight, graphicsElement, _spotColorOverride)
                : null;
        }

        public void SetStrokeStyle(SvgGraphicsElement graphicsElement)
        {
            var style = graphicsElement?.Style;
            _strokeColor = graphicsElement != null && _pageViewport != null
                ? SvgPaintServerToDynamicPdfColorConverter.ConvertToColor(graphicsElement.CreateStrokePaintServer(), _pageViewport, _pageHeight, graphicsElement, _spotColorOverride)
                : null;
            _strokeWidth = _strokeColor != null ? graphicsElement?.StrokeWidth ?? 0 : 0;
            _lineJoin = style?.StrokeLineJoin.ConvertToDynamicPdf() ?? LineJoin.Miter;
            _miterLimit = style?.StrokeMiterLimit ?? 1f;
            _lineCap = style?.StrokeLineCap.ConvertToDynamicPdf() ?? LineCap.Butt;
            _strokeStyle = LineStyle.Solid;

            var dashArray = style?.StrokeDashArray.Value;
            if (dashArray?.Length > 0)
            {
                var dashValues = dashArray.Select(i => i.Value).ToArray();
                _strokeStyle = new LineStyle(dashValues, style.StrokeDashOffset.Value.Value);
            }
        }

        public override void DefaultVisit(SvgPathSeg segment)
            => throw new NotSupportedException($"SVG Path segment of type '{segment?.GetType()}' can't be converted to a DyanmicPdf SubPath");

        public override void VisitMovetoAbs(SvgPathSegMovetoAbs moveto)
        {
            if (!StartNewPathIfNull(moveto.X, moveto.Y))
            {
                AddNewSubPath(new MoveSubPath(moveto.X, moveto.Y));
            }
        }

        public override void VisitCurvetoCubicAbs(SvgPathSegCurvetoCubicAbs curve)
            => AddNewSubPath(new CurveSubPath(curve.X, curve.Y, curve.X1, curve.Y1, curve.X2, curve.Y2));

        public override void VisitLinetoAbs(SvgPathSegLinetoAbs line)
            => AddNewSubPath(new LineSubPath(line.X, line.Y));

        public override void VisitClosePath(SvgPathSegClosePath segment)
            => AddNewSubPath(new CloseSubPath());

        private bool StartNewPathIfNull(float x = 0, float y = 0)
        {
            if (_currentPath == null)
            {
                _currentPath = new CustomPath(x, y, _strokeColor, _fillColor, _strokeWidth, _strokeStyle, false)
                {
                    LineCap = _lineCap,
                    LineJoin = _lineJoin,
                    FillRuleEvenOdd = _fillEvenOdd,
                    MiterLimit = _miterLimit
                };
                Paths.Add(_currentPath);
                return true;
            }
            return false;
        }

        private void AddNewSubPath(SubPath subpath)
        {
            StartNewPathIfNull();
            _currentPath.SubPaths.Add(subpath);
        }

        private readonly VectorElementPdfPageViewport _pageViewport;
        private readonly float _pageHeight;

        private ceTe.DynamicPDF.Color _fillColor;
        private ceTe.DynamicPDF.Color _strokeColor;
        private float _strokeWidth;
        private LineStyle _strokeStyle;
        private LineCap _lineCap;
        private LineJoin _lineJoin;
        private bool _fillEvenOdd;
        private float _miterLimit;

        private Path _currentPath;
    }
}
