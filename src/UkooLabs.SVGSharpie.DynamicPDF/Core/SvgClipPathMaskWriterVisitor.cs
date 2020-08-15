using System;
using System.Collections.Generic;
using System.Linq;
using ceTe.DynamicPDF.IO;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;

namespace UkooLabs.SVGSharpie.DynamicPDF.Core
{
    internal sealed class SvgClipPathMaskWriterVisitor : SvgElementVisitor
    {
        private readonly PdfSpotColor _spotColorOverride;

        public bool ClippingAreaPainted { get; private set; }

        public SvgClipPathMaskWriterVisitor(PageWriter writer, SvgMatrix rootTransform, PdfSpotColor spotColorOverride)
        {
            _spotColorOverride = spotColorOverride;
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _matrixStack.Push(SvgMatrix.Identity);
            PushMatrix(rootTransform);
        }

        public override void VisitClipPathElement(SvgClipPathElement element)
        {
            VisitStructuralElement(element);
            if (!ClippingAreaPainted)
            {
                return;
            }

            // 'W' operator intersects the current clipping path by the new path (above) using non-zero winding
            // 'n' operator is a no-op path-painting operator, no marks will be rendered

            _writer.Write_W();
            _writer.Write_n();
        }

        public override void VisitGElement(SvgGElement element) 
            => VisitStructuralElement(element);

        public override void VisitRectElement(SvgRectElement element) 
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override void VisitCircleElement(SvgCircleElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override void VisitEllipseElement(SvgEllipseElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override void VisitLineElement(SvgLineElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override void VisitPolylineElement(SvgPolylineElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override void VisitPolygonElement(SvgPolygonElement element)
            => VisitPathSegList(element, element.ConvertToPathSegList());

        public override void VisitPathElement(SvgPathElement element)
            => VisitPathSegList(element, element.Segments);

        private void VisitPathSegList(SvgGraphicsElement graphicsElement, SvgPathSegList paths)
        {
            if (!ShouldDisplay(graphicsElement))
            {
                return;
            }

            PushTransformsMatrix(graphicsElement.Transform);
            {
                var pathsToDraw = paths
                    .ConvertToAbsolute()
                    .ConvertAllLinesAndCurvesToCubicCurves()
                    .MultiplyByMatrix(CurrentMatrix);

                var svgToPdfPathConverter = new SvgPathSegToDynamicPdfPathsConverter(graphicsElement, null, 0, _spotColorOverride);
                pathsToDraw.ForEach(i => i.Accept(svgToPdfPathConverter));
                
                foreach (var path in svgToPdfPathConverter.Paths)
                {
                    path.Draw(_writer);
                    ClippingAreaPainted = true;
                }
            }
            PopTransformsMatrix(graphicsElement.Transform);
        }

        private void VisitStructuralElement(SvgStructuralGraphicsElement element)
            => TransformAndVisitElements(element.Transform, element.Children);

        private void TransformAndVisitElements(SvgTransformList transforms, IEnumerable<SvgElement> elements)
        {
            PushTransformsMatrix(transforms);
            {
                foreach (var child in elements)
                {
                    child.Accept(this);
                }
            }
            PopTransformsMatrix(transforms);
        }
        
        private static bool ShouldDisplay(SvgGraphicsElement element) 
            => element.Style.Display != CssDisplayType.None && element.Style.Visibility == CssVisibilityType.Visible;

        private SvgMatrix CurrentMatrix => _matrixStack.Peek();

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

        private readonly Stack<SvgMatrix> _matrixStack = new Stack<SvgMatrix>();
        private readonly PageWriter _writer;
    }
}