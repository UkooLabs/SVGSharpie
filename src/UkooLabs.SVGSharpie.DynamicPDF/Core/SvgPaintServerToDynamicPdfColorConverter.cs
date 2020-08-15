using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UkooLabs.SVGSharpie.DynamicPDF.Extensions;
using Color = ceTe.DynamicPDF.Color;
using UkooLabs.SVGSharpie.DynamicPDF.Core.Shading;
using PdfSpotColor = ceTe.DynamicPDF.SpotColor;

namespace UkooLabs.SVGSharpie.DynamicPDF.Core
{
    internal sealed class SvgPaintServerToDynamicPdfColorConverter
    {
        public static Color ConvertToColor(SvgPaintServer paintServer, VectorElementPdfPageViewport pageViewport, float pageHeight, SvgGraphicsElement element, PdfSpotColor spotColorInk)
            => paintServer?.Accept(new Visitor(pageViewport, element, pageHeight, spotColorInk));

        private sealed class Visitor : SvgPaintServerVisitor<Color>
        {
            private readonly PdfSpotColor _spotColorOverride;

            public Visitor(VectorElementPdfPageViewport pageViewport, SvgGraphicsElement element, float pageHeight, PdfSpotColor spotColorOverride)
            {
                _spotColorOverride = spotColorOverride;
                _pageViewport = pageViewport ?? throw new ArgumentNullException(nameof(pageViewport));
                _element = element ?? throw new ArgumentNullException(nameof(element));
                _pageHeight = pageHeight;
            }

            public override Color DefaultVisit(SvgPaintServer paintServer)
                => throw new NotSupportedException($"SVG paint server '{paintServer?.GetType().Name}' is not supported");

            public override Color VisitSolidColorPaintServer(SvgSolidColorPaintServer paintServer)
                => paintServer.Color.ToPdfColor(_spotColorOverride);

            public override Color VisitRadialGradientPaintServer(SvgRadialGradientPaintServer paintServer)
            {
                if (CheckInvalidGradientPaintServer(paintServer, _spotColorOverride, out var color))
                {
                    return color;
                }

                var stops = ConvertStops(paintServer.Stops, _spotColorOverride);

                var fx = paintServer.FocalX;
                var fy = paintServer.FocalY;
                var cx = paintServer.CircleX;
                var cy = paintServer.CircleY;
                var cr = paintServer.CircleRadius;

                RectangleF pdfPagePlacementRect;

                if (paintServer.Units == SvgUnitTypes.ObjectBoundingBox)
                {
                    var bbox = GetBBoxOrThrow();
                    pdfPagePlacementRect = CalculatePagePlacementRect(bbox);

                    fx /= bbox.Width;
                    fy /= bbox.Height;
                    cx /= bbox.Width;
                    cy /= bbox.Height;
                    cr /= bbox.Width;
                }
                else
                {
                    // units = userSpaceOnUse
                    pdfPagePlacementRect = new RectangleF(0, _pageHeight, _pageViewport.ScaleX, _pageViewport.ScaleY);
                }

                var circle0 = new Circle(fx, fy, 0);
                var circle1 = new Circle(cx, cy, cr);

                return new RadialGradientShadingColor(pdfPagePlacementRect, circle0, circle1, stops);
            }

            public override Color VisitLinearGradientPaintServer(SvgLinearGradientPaintServer paintServer)
            {
                if (CheckInvalidGradientPaintServer(paintServer, _spotColorOverride, out var color))
                {
                    return color;
                }

                var x1 = paintServer.X1;
                var y1 = paintServer.Y1;
                var y2 = paintServer.Y2;
                var x2 = paintServer.X2;

                RectangleF pdfPagePlacementRect;

                if (paintServer.Units == SvgUnitTypes.ObjectBoundingBox)
                {
                    var bbox = GetBBoxOrThrow();
                    pdfPagePlacementRect = CalculatePagePlacementRect(bbox);

                    x1 /= bbox.Width;
                    x2 /= bbox.Width;
                    y1 /= bbox.Height;
                    y2 /= bbox.Height;
                }
                else
                {
                    // units = userSpaceOnUse
                    pdfPagePlacementRect = new RectangleF(0, _pageHeight, _pageViewport.ScaleX, _pageViewport.ScaleY);
                }

                var domainRect = new RectangleF(x1, y1, width: x2 - x1, height: y2 - y1);

                var stops = ConvertStops(paintServer.Stops, _spotColorOverride);
                return new LinearGradientShadingColor(pdfPagePlacementRect, domainRect, stops);
            }

            private static bool CheckInvalidGradientPaintServer(SvgGradientPaintServer paintServer, PdfSpotColor spotColorInk, out Color color)
            {
                color = null;

                switch (paintServer.Stops.Count)
                {
                    case 0:
                        return true;
                    case 1:
                        color = paintServer.Stops[0].Color.ToPdfColor(spotColorInk);
                        return true;
                }

                return false;
            }

            private RectangleF CalculatePagePlacementRect(SvgRect bbox)
            {
                var x1 = (bbox.X * _pageViewport.ScaleX) + _pageViewport.OffsetX;
                var y1 = (bbox.Y * _pageViewport.ScaleY) + _pageViewport.OffsetY;
                var x2 = (bbox.Right * _pageViewport.ScaleX) + _pageViewport.OffsetX;
                var y2 = (bbox.Bottom * _pageViewport.ScaleY) + _pageViewport.OffsetY;

                var pageX = x1 + _pageViewport.PagePlacement.X;
                var pageY = _pageHeight - (y1 + _pageViewport.PagePlacement.Y); // invert the Y coordinate here for PDF space

                var pdfPagePlacementRect = new RectangleF(pageX, pageY, x2 - x1, y2 - y1);

                return pdfPagePlacementRect;
            }

            private SvgRect GetBBoxOrThrow()
            {
                var bboxOrNull = _element.GetBBox();
                if (bboxOrNull == null)
                {
                    throw new Exception("Element has no bounding box");
                }

                return bboxOrNull.Value;
            }

            private static IReadOnlyCollection<GradientColorStop> ConvertStops(IEnumerable<SvgGradientPaintServerColorStop> stops, PdfSpotColor spotColorInk)
            {
                var result = stops.Select(s => ConvertStop(s, spotColorInk)).ToList();
                if (result.Count > 1)
                {
                    var lastStop = result[result.Count - 1];
                    if (lastStop.Offset < 1)
                    {
                        result.Add(new GradientColorStop(lastStop.Color, lastStop.Offset + 0.0001001f));
                    }
                }
                return result;
            }

            private static GradientColorStop ConvertStop(SvgGradientPaintServerColorStop stop, PdfSpotColor spotColorOverride)
            {
                if (Math.Abs(stop.Opacity - 1) > float.Epsilon)
                {
                    throw new NotSupportedException($"Gradient stop opacity is not supported, only supports value of 1 but got {stop.Opacity}");
                }
                return new GradientColorStop(stop.Color.ToPdfColor(spotColorOverride), stop.Offset);
            }

            private readonly VectorElementPdfPageViewport _pageViewport;
            private readonly SvgGraphicsElement _element;
            private readonly float _pageHeight;
        }
    }
}