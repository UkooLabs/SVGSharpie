using System;
using System.Collections.Generic;
using System.Linq;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using EquinoxLabs.SVGSharpie;
using EquinoxLabs.SVGSharpie.DynamicPDF;
using EquinoxLabs.SVGSharpie.DynamicPDF.Extensions;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;

namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements
{
    /// <inheritdoc />
    /// <summary>
    /// Converts <see cref="SvgElement"/> into their <see cref="PageElement"/> equivalents.  All SVG Shape elements 
    /// are first converted into paths which are then converted into dynamic pdf elements.  This path conversion is done 
    /// due to the different coordinate systems of SVG documents, where the origin is at top left, and PDF documents where 
    /// the origin is at bottom left and increasing upwards.  Since we need to support matrix transformations in SVG documents 
    /// we pretransform all SVG path coordinates to their final positions which we then supply to the DynamicPdf library which 
    /// internally performs the y-axis flip.
    /// </summary>
    internal sealed class SvgElementToDynamicPdfElementConverter : SvgElementVisitor<PageElement>
    {    
        private readonly SvgSvgElement _rootSvgElement;
        private readonly VectorElementPdfPageViewport _pageViewport;
        private readonly float _pageHeight;
        private readonly Stack<SvgMatrix> _matrixStack = new Stack<SvgMatrix>();
        private float _groupAlpha = 1;
        private PdfSpotColor _spotColorInk;

        private SvgMatrix CurrentMatrix => _matrixStack.Peek();

        public SvgElementToDynamicPdfElementConverter(SvgSvgElement rootSvgElement, VectorElementPdfPageViewport pageViewport, float pageHeight, PdfSpotColor spotColorInk)
        {
            _spotColorInk = spotColorInk;
            _rootSvgElement = rootSvgElement ?? throw new ArgumentNullException(nameof(rootSvgElement));
            _pageViewport = pageViewport ?? throw new ArgumentNullException(nameof(pageViewport));
            _pageHeight = pageHeight;
            _matrixStack.Push(SvgMatrix.Identity);
        }

        public override PageElement DefaultVisit(SvgElement element)
            => throw new NotSupportedException($"SVG Element of type '{element?.GetType()}' not supported by DynamicPdf");

        public override PageElement VisitTitleElement(SvgTitleElement element) => null;

        public override PageElement VisitDescElement(SvgDescElement element) => null;

        public override PageElement VisitDefsElement(SvgDefsElement element) => null;

        public override PageElement VisitSymbolElement(SvgSymbolElement element) => null;

        public override PageElement VisitStyleElement(SvgStyleElement element) => null;

        public override PageElement VisitClipPathElement(SvgClipPathElement element) => null;

        public override PageElement VisitGElement(SvgGElement element)
        {
            if (!IsVisible(element))
            {
                return null;
            }

            var ownOpacity = element.PresentationStyleData.FillOpacity;
            if (ownOpacity != null)
            {
                var oldGAlpha = _groupAlpha;
                _groupAlpha *= ownOpacity.Value;
                var result = new TransparencyGroup(_groupAlpha)
                {
                    VisitStructuralElement(element)
                };
                _groupAlpha = oldGAlpha;
                return result;
            }

            return VisitStructuralElement(element);
        }

        public override PageElement VisitLinearGradientElement(SvgLinearGradientElement element) => null;

        public override PageElement VisitRadialGradientElement(SvgRadialGradientElement element) => null;

        public override PageElement VisitSvgElement(SvgSvgElement element)
        {
            if (element.IsOutermost)
            {
                return EstablishViewportThen(element, element.ViewWidth, element.ViewHeight, VisitStructuralElement, element);
            }
            var tx = element.X?.Value ?? 0;
            var ty = element.Y?.Value ?? 0;
            PushMatrix(SvgMatrix.CreateTranslate(tx, ty));
            var viewportWidth = element.Width ?? 0;
            var viewportHeight = element.Height ?? 0;
            var result = EstablishViewportThen(element, viewportWidth, viewportHeight, VisitStructuralElement, element);
            PopMatrix();
            return result;
        }

        public override PageElement VisitRectElement(SvgRectElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override PageElement VisitCircleElement(SvgCircleElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override PageElement VisitEllipseElement(SvgEllipseElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override PageElement VisitLineElement(SvgLineElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override PageElement VisitPolylineElement(SvgPolylineElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override PageElement VisitPolygonElement(SvgPolygonElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override PageElement VisitPathElement(SvgPathElement element)
            => VisitPathSegList(element, element.Segments);

        public override PageElement VisitUseElement(SvgUseElement element)
        {
            if (element.IsRenderingDisabled || string.IsNullOrEmpty(element.Href))
            {
                return null;
            }
            var tx = element.X?.Value ?? 0;
            var ty = element.Y?.Value ?? 0;
            var transform = SvgMatrix.CreateTranslate(tx, ty);

            PushMatrix(transform);
            PushTransformsMatrix(element.Transform);
            PageElement result;
            var generated = element.GeneratedElement;
            if (generated is SvgSymbolElement symbol)
            {
                var viewportWidth = element.Width?.Value ?? 0;
                var viewportHeight = element.Height?.Value ?? 0;
                result = EstablishViewportThen(symbol, viewportWidth, viewportHeight,
                    s => TransformAndVisitElements(s.Transform, s.Children), symbol);
            }
            else
            {
                result = generated.Accept(this);
            }
            PopTransformsMatrix(element.Transform);
            PopMatrix();
            return result;
        }

        private PageElement VisitPathSegList(SvgGraphicsElement graphicsElement, SvgPathSegList paths)
        {
            if (!IsVisible(graphicsElement))
            {
                return null;
            }

            var result = new Group();

            PushTransformsMatrix(graphicsElement.Transform);
            var innerMatrix = CurrentMatrix;

            var pathsToDraw = paths
                .ConvertToAbsolute()
                .ConvertAllLinesAndCurvesToCubicCurves()
                .MultiplyByMatrix(CurrentMatrix);

            var fillOpacity = Math.Max(0, Math.Min(1, graphicsElement.Style.FillOpacity));
            var strokeOpacity = Math.Max(0, Math.Min(1, graphicsElement.Style.StrokeOpacity));
            var svgToPdfPathConverter = new SvgPathSegToDynamicPdfPathsConverter(graphicsElement, _pageViewport, _pageHeight, _spotColorInk);

            bool IsPaintedPath(Path path) => path.FillColor != null || path.LineColor != null;

            if (Math.Abs(fillOpacity - 1) > float.Epsilon || Math.Abs(strokeOpacity - 1) > float.Epsilon)
            {
                var fillGroup = new TransparencyGroup(fillOpacity * _groupAlpha);
                svgToPdfPathConverter.SetStrokeStyle(null);
                pathsToDraw.ForEach(i => i.Accept(svgToPdfPathConverter));
                fillGroup.AddRange(svgToPdfPathConverter.Paths.Where(IsPaintedPath));

                svgToPdfPathConverter.Reset();
                svgToPdfPathConverter.SetFillStyle(null);
                svgToPdfPathConverter.SetStrokeStyle(graphicsElement);
                var strokeGroup = new TransparencyGroup(strokeOpacity * _groupAlpha);
                pathsToDraw.ForEach(i => i.Accept(svgToPdfPathConverter));
                strokeGroup.AddRange(svgToPdfPathConverter.Paths.Where(IsPaintedPath));

                result.Add(fillGroup);
                result.Add(strokeGroup);
            }
            else
            {
                pathsToDraw.ForEach(i => i.Accept(svgToPdfPathConverter));
                result.AddRange(svgToPdfPathConverter.Paths.Where(IsPaintedPath));
            }
            PopTransformsMatrix(graphicsElement.Transform);

            return result.Count == 0 ? null : CheckAndApplyClipPath(graphicsElement, result, innerMatrix);
        }

        private PageElement CheckAndApplyClipPath(SvgGraphicsElement element, PageElement result, SvgMatrix matrix)
        {
            if (result == null)
            {
                return null;
            }

            var clipPathRef = element.PresentationStyleData.ClipPath.Value?.FragmentIdentifier;
            var clipPath = _rootSvgElement.GetElementById(clipPathRef) as SvgClipPathElement;
            if (clipPath != null)
            {
                if (clipPath.Children.Count == 0)
                {
                    // clipPath defines the area to be rendered, no children, nothing to render
                    return null;
                }
                return new SvgClipPathPageElement(result, clipPath, matrix, _spotColorInk);
            }
            return result;
        }

        private PageElement VisitStructuralElement(SvgStructuralGraphicsElement element)
            => CheckAndApplyClipPath(element, TransformAndVisitElements(element.Transform, element.Children), CurrentMatrix);

        private PageElement TransformAndVisitElements(SvgTransformList transforms, IEnumerable<SvgElement> elements)
        {
            var result = new Group();
            PushTransformsMatrix(transforms);
            {
                foreach (var child in elements)
                {
                    var transformed = child.Accept(this);
                    if (transformed != null)
                    {
                        result.Add(transformed);
                    }
                }
            }
            PopTransformsMatrix(transforms);
            return result;
        }

        private static bool IsVisible(SvgGraphicsElement element)
        {
            if (element.Style.Display == CssDisplayType.None)
            {
                return false;
            }

            if (element.Style.Visibility != CssVisibilityType.Visible)
            {
                return false;
            }

            return true;
        }

        private PageElement EstablishViewportThen<TArg>(ISvgFitToViewbox fitToViewbox, float viewportWidth, float viewportHeight, Func<TArg, PageElement> transform, TArg arg)
        {
            ClippingGroup result = null;
            var matrix = CurrentMatrix;
            if (PushViewportMatrixIfValid(fitToViewbox, viewportWidth, viewportHeight))
            {
                SvgMatrix.Multiply(matrix, 0, 0, out var xMin, out var yMin);
                SvgMatrix.Multiply(matrix, viewportWidth, viewportHeight, out var xMax, out var yMax);
                result = new ClippingGroup(xMin, yMin, xMax, yMax, transform(arg));
                PopMatrix();
            }
            return result;
        }

        private bool PushViewportMatrixIfValid(ISvgFitToViewbox fitToViewbox, float viewportWidth, float viewportHeight)
        {
            if (viewportWidth <= 0 || viewportHeight <= 0)
            {
                return false;
            }
            var matrix = fitToViewbox.CalculateViewboxFit(viewportWidth, viewportHeight);
            PushMatrix(matrix);
            return true;
        }

        private void PushMatrix(SvgMatrix matrix) => _matrixStack.Push(SvgMatrix.Multiply(_matrixStack.Peek(), matrix));

        private void PopMatrix() => _matrixStack.Pop();

        private void PushTransformsMatrix(SvgTransformList transforms)
        {
            if (transforms.Count == 0)
            {
                return;
            }
            var matrix = transforms.Aggregate(SvgMatrix.Identity, (current, transform) => current * transform.Matrix);
            PushMatrix(matrix);
        }

        private void PopTransformsMatrix(SvgTransformList transforms)
        {
            if (transforms.Count > 0)
            {
                PopMatrix();
            }
        }
    }
}