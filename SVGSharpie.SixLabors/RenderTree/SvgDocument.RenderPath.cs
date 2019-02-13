using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using SixLabors.Svg.Shapes;
using SVGSharpie;

namespace SixLabors.Svg.Dom
{
    internal sealed partial class SvgDocumentRenderer<TPixel> : SvgElementWalker
        where TPixel : struct, IPixel<TPixel>
    {

        private void RenderPathSegList(SvgGraphicsElement element, SvgPathSegList segs)
        {
            var reenderer = new PathRenderer();
            foreach (var seq in segs)
            {
                seq.Accept(reenderer);
            }
            var path = reenderer.Path();
            this.RenderShapeToCanvas(element, path);
        }


        internal class PathRenderer : SvgPathSegVisitor
        {

            public override void VisitClosePath(SvgPathSegClosePath segment)
                => this.Close();

            public override void VisitMovetoAbs(SvgPathSegMovetoAbs segment)
                => this.MoveTo(new PointF(segment.X, segment.Y));

            public override void VisitMovetoRel(SvgPathSegMovetoRel segment)
                => this.MoveTo(new PointF(segment.X, segment.Y) + this.currentPoint);

            public override void VisitLinetoAbs(SvgPathSegLinetoAbs segment)
                => this.LineTo(new PointF(segment.X, segment.Y));

            public override void VisitLinetoRel(SvgPathSegLinetoRel segment)
                => this.LineTo(new PointF(segment.X, segment.Y) + this.currentPoint);

            public override void VisitCurvetoCubicAbs(SvgPathSegCurvetoCubicAbs segment)
                => this.CubicBezierTo(new PointF(segment.X1, segment.Y1), new PointF(segment.X2, segment.Y2), new PointF(segment.X, segment.Y));

            public override void VisitCurvetoCubicRel(SvgPathSegCurvetoCubicRel segment)
                => this.CubicBezierTo(new PointF(segment.X1, segment.Y1) + this.currentPoint, new PointF(segment.X2, segment.Y2) + this.currentPoint, new PointF(segment.X, segment.Y) + this.currentPoint);

            public override void VisitCurvetoCubicSmoothAbs(SvgPathSegCurvetoCubicSmoothAbs segment)
                => this.SmoothCubicBezierTo(new PointF(segment.X2, segment.Y2), new PointF(segment.X, segment.Y));

            public override void VisitCurvetoCubicSmoothRel(SvgPathSegCurvetoCubicSmoothRel segment)
                => this.SmoothCubicBezierTo(new PointF(segment.X2, segment.Y2) + currentPoint, new PointF(segment.X, segment.Y) + currentPoint);


            public override void VisitCurvetoQuadraticAbs(SvgPathSegCurvetoQuadraticAbs segment)
                => this.QuadraticBezierTo(new PointF(segment.X1, segment.Y1), new PointF(segment.X, segment.Y));

            public override void VisitCurvetoQuadraticRel(SvgPathSegCurvetoQuadraticRel segment)
                => this.QuadraticBezierTo(new PointF(segment.X1, segment.Y1) + this.currentPoint, new PointF(segment.X, segment.Y) + this.currentPoint);
            public override void VisitCurvetoQuadraticSmoothAbs(SvgPathSegCurvetoQuadraticSmoothAbs segment)
                => this.SmoothQuadraticBezierTo(new PointF(segment.X, segment.Y));

            public override void VisitCurvetoQuadraticSmoothRel(SvgPathSegCurvetoQuadraticSmoothRel segment)
                => this.SmoothQuadraticBezierTo(new PointF(segment.X, segment.Y) + currentPoint);

            public override void VisitArcAbs(SvgPathSegArcAbs segment)
                => this.ArcTo(new PointF(segment.X, segment.Y), new SizeF(segment.RadiusX, segment.RadiusY), segment.Angle, segment.LargeArcFlag, segment.SweepFlag);

            public override void VisitArcRel(SvgPathSegArcRel segment)
                => this.ArcTo(new PointF(segment.X, segment.Y) + currentPoint, new SizeF(segment.RadiusX, segment.RadiusY), segment.Angle, segment.LargeArcFlag, segment.SweepFlag);

            public override void VisitLinetoHorizontalAbs(SvgPathSegLinetoHorizontalAbs segment)
                => this.LineTo(new PointF(segment.X, this.currentPoint.Y));

            public override void VisitLinetoHorizontalRel(SvgPathSegLinetoHorizontalRel segment)
                => this.LineTo(this.currentPoint + new PointF(segment.X, 0));

            public override void VisitLinetoVerticalAbs(SvgPathSegLinetoVerticalAbs segment)
                => this.LineTo(new PointF(this.currentPoint.X, segment.Y));

            public override void VisitLinetoVerticalRel(SvgPathSegLinetoVerticalRel segment)
                => this.LineTo(this.currentPoint + new PointF(0, segment.Y));

            /// <summary>
            /// The builder. TODO: Should this be a property?
            /// </summary>
            // ReSharper disable once InconsistentNaming
            private readonly PathBuilder builder;
            private readonly List<IPath> paths = new List<IPath>();
            private PointF currentPoint = default(PointF);
            private PointF initalPoint = default;
            private PointF lastCubicControlPoint = default;
            private PointF lastQuadriticControlPoint = default;

            public PointF CurrentPoint => currentPoint;
            public PointF InitalPoint => initalPoint;

            /// <summary>
            /// Initializes a new instance of the <see cref="BaseGlyphBuilder"/> class.
            /// </summary>
            public PathRenderer()
            {
                // glyphs are renderd realative to bottom left so invert the Y axis to allow it to render on top left origin surface
                this.builder = new PathBuilder();
            }

            /// <summary>
            /// Gets the paths that have been rendered by this.
            /// </summary>
            public IPath Path()
            {
                return this.builder.Build();
            }

            /// <summary>
            /// Draws a cubic bezier from the current point  to the <paramref name="point"/>
            /// </summary>
            /// <param name="secondControlPoint">The second control point.</param>
            /// <param name="thirdControlPoint">The third control point.</param>
            /// <param name="point">The point.</param>
            public void CubicBezierTo(PointF secondControlPoint, PointF thirdControlPoint, PointF point)
            {
                this.builder.AddBezier(this.currentPoint, secondControlPoint, thirdControlPoint, point);
                this.currentPoint = point;
                lastCubicControlPoint = thirdControlPoint;
                lastQuadriticControlPoint = currentPoint;
            }

            /// <summary>
            /// Draws a quadratics bezier from the current point  to the <paramref name="point"/>
            /// </summary>
            /// <param name="secondControlPoint">The second control point.</param>
            /// <param name="point">The point.</param>
            public void SmoothCubicBezierTo(PointF thirdControlPoint, PointF point)
            {
                var cp = currentPoint + (currentPoint - lastCubicControlPoint);
                this.CubicBezierTo(cp, thirdControlPoint, point);
                lastCubicControlPoint = thirdControlPoint;
                lastQuadriticControlPoint = currentPoint;
            }

            /// <summary>
            /// Draws a line from the current point  to the <paramref name="point"/>.
            /// </summary>
            /// <param name="point">The point.</param>
            public void LineTo(PointF point)
            {
                this.builder.AddLine(this.currentPoint, point);
                this.currentPoint = point;
                lastCubicControlPoint = currentPoint;
                lastQuadriticControlPoint = currentPoint;
            }

            /// <summary>
            /// Moves to current point to the supplied vector.
            /// </summary>
            /// <param name="point">The point.</param>
            public void MoveTo(PointF point)
            {
                this.builder.StartFigure();
                this.currentPoint = point;
                this.initalPoint = point;
                lastCubicControlPoint = currentPoint;
                lastQuadriticControlPoint = currentPoint;
            }

            /// <summary>
            /// Draws a quadratics bezier from the current point  to the <paramref name="point"/>
            /// </summary>
            /// <param name="secondControlPoint">The second control point.</param>
            /// <param name="point">The point.</param>
            public void QuadraticBezierTo(PointF secondControlPoint, PointF point)
            {
                this.builder.AddBezier(this.currentPoint, secondControlPoint, point);
                this.currentPoint = point;
                lastCubicControlPoint = currentPoint;
                lastQuadriticControlPoint = secondControlPoint;
            }
            /// <summary>
            /// Draws a quadratics bezier from the current point  to the <paramref name="point"/>
            /// </summary>
            /// <param name="secondControlPoint">The second control point.</param>
            /// <param name="point">The point.</param>
            public void SmoothQuadraticBezierTo(PointF point)
            {
                var secondControlPoint = currentPoint + (currentPoint - lastQuadriticControlPoint);
                this.QuadraticBezierTo(secondControlPoint, point);

                lastCubicControlPoint = currentPoint;
                lastQuadriticControlPoint = secondControlPoint;
            }

            /// <summary>
            /// Draws a arc from the current point  to the <paramref name="point"/>
            /// </summary>
            public void ArcTo(PointF point, SizeF radius, float angle, bool largeArcFlag, bool sweepFlag)
            {
                this.builder.AddSegment(new ArcLineSegemnt(this.currentPoint, point, radius, angle, largeArcFlag, sweepFlag));
                this.currentPoint = point;
                lastCubicControlPoint = point;
                lastQuadriticControlPoint = point;
            }

            internal void Close()
            {
                this.LineTo(initalPoint);
                this.builder.CloseFigure();
                this.currentPoint = this.initalPoint;
                lastCubicControlPoint = initalPoint;
                lastQuadriticControlPoint = initalPoint;
            }
        }


        public void VisitSvgGeometryElement(SvgGeometryElement element)
        {
            var pathSegList = element.ConvertToPathSegList();
            RenderPathSegList(element, pathSegList);
        }

        public override void VisitLineElement(SvgLineElement element)
        {
            base.VisitLineElement(element);
            VisitSvgGeometryElement(element);
        }

        public override void VisitPolylineElement(SvgPolylineElement element)
        {
            base.VisitPolylineElement(element);
            VisitSvgGeometryElement(element);
        }

        public override void VisitPolygonElement(SvgPolygonElement element)
        {
            base.VisitPolygonElement(element);
            VisitSvgGeometryElement(element);
        }

        public override void VisitPathElement(SvgPathElement element)
        {
            base.VisitPathElement(element);
            VisitSvgGeometryElement(element);
        }

    }
}
