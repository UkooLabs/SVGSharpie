using SixLabors.Primitives;
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SixLabors.Svg.Shapes
{
    public class RoundedRect : IPath
    {
        private IPath innerPath;
        public RoundedRect(float x, float y, float width, float height, float rx, float ry)
        {
            IPath rect = new SixLabors.Shapes.RectangularPolygon(x, y, width, height);

            if (rx > 0 && ry > 0)
            {
                rect = MakeRounded(rect, rx, ry);
            }

            innerPath = rect;
        }

        private static IPath MakeRounded(IPath path, float rx, float ry)
        {
            return path.Clip(BuildCorners(path.Bounds.Width, path.Bounds.Height, rx, ry).Translate(path.Bounds.Location));
        }

        private static IPathCollection BuildCorners(float imageWidth, float imageHeight, float cornerRadiusX, float cornerRadiusY)
        {
            // first create a square
            var rect = new RectangularPolygon(0, 0, cornerRadiusX, cornerRadiusY);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerToptLeft = rect.Clip(new EllipsePolygon(cornerRadiusX, cornerRadiusY, cornerRadiusX * 2, cornerRadiusY * 2));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the orgional artound the center of the image
            var center = new Vector2(imageWidth / 2F, imageHeight / 2F);

            float rightPos = imageWidth - cornerToptLeft.Bounds.Width;
            float bottomPos = imageHeight - cornerToptLeft.Bounds.Height;

            // move it across the widthof the image - the width of the shape
            IPath cornerTopRight = cornerToptLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerToptLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerToptLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerToptLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }

        public PathTypes PathType => this.innerPath.PathType;

        public RectangleF Bounds => this.innerPath.Bounds;

        public int MaxIntersections => this.innerPath.MaxIntersections;

        public float Length => this.innerPath.Length;

        public IPath AsClosedPath()
        {
            return this.innerPath.AsClosedPath();
        }

        public bool Contains(PointF point)
        {
            return this.innerPath.Contains(point);
        }

        public PointInfo Distance(PointF point)
        {
            return this.innerPath.Distance(point);
        }

        public int FindIntersections(PointF start, PointF end, PointF[] buffer, int offset)
        {
            return this.innerPath.FindIntersections(start, end, buffer, offset);
        }

        public int FindIntersections(PointF start, PointF end, Span<PointF> buffer)
        {
            return this.innerPath.FindIntersections(start, end, buffer);
        }

        public IEnumerable<ISimplePath> Flatten()
        {
            return this.innerPath.Flatten();
        }

        public SegmentInfo PointAlongPath(float distanceAlongPath)
        {
            return this.innerPath.PointAlongPath(distanceAlongPath);
        }

        public IPath Transform(Matrix3x2 matrix)
        {
            return this.innerPath.Transform(matrix);
        }
    }
}
