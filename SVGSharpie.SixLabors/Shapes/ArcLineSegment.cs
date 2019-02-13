using SixLabors.Primitives;
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SixLabors.Svg.Shapes
{
    public class ArcLineSegemnt : SixLabors.Shapes.ILineSegment
    {
        private ILineSegment[] innerSegments;

        public ArcLineSegemnt(PointF from, PointF to, SizeF radius, float angle, bool largeArcFlag, bool sweepFlag)
        {
            innerSegments = EllipticArcToBezierCurve(from, to, radius, angle, largeArcFlag, sweepFlag).ToArray();
        }

        private ArcLineSegemnt()
        { }
        public PointF EndPoint => this.innerSegments.Last().EndPoint;

        public IReadOnlyList<PointF> Flatten()
        {
            return this.innerSegments.SelectMany(x => x.Flatten()).ToList();
        }

        public ILineSegment Transform(Matrix3x2 matrix)
        {
            return new ArcLineSegemnt()
            {
                innerSegments = this.innerSegments.Select(x => x.Transform(matrix)).ToArray()
            };
        }

        const float _zeroTolerance = 1e-05f;
        static bool EllipticArcOutOfRange(Vector2 from, Vector2 to, Vector2 absRadius)
        {
            //F.6.2 Out-of-range parameters
            var len = (to - from).LengthSquared();
            if (len < _zeroTolerance)
                return true;

            if (absRadius.X < _zeroTolerance || absRadius.Y < _zeroTolerance)
                return true;

            return false;
        }

        static IEnumerable<ILineSegment> EllipticArcToBezierCurve(PointF from, PointF to, SizeF radius, float angle, bool largeArcFlag, bool sweepFlag)
        {
            Vector2 radiusVecotr = Vector2.Abs(radius);

            if (EllipticArcOutOfRange(from, to, radius))
            {
                return new[] { new LinearLineSegment(from, to) };
            }
            else
            {

                var xAngle = angle;

                EndpointToCenterArcParams(from, to, ref radiusVecotr, xAngle,
                    largeArcFlag, sweepFlag,
                    out var center, out var angles);

                return EllipticArcToBezierCurveInner(from, center, radius, xAngle, angles.X, angles.Y);
            }
        }

        static Vector2 EllipticArcPoint(Vector2 c, Vector2 r, float xAngle, float t)
        {
            return new Vector2(
                (float)(c.X + r.X * Math.Cos(xAngle) * Math.Cos(t) - r.Y * Math.Sin(xAngle) * Math.Sin(t)),
                (float)(c.Y + r.X * Math.Sin(xAngle) * Math.Cos(t) + r.Y * Math.Cos(xAngle) * Math.Sin(t)));
        }

        static Vector2 EllipticArcDerivative(Vector2 c, Vector2 r, float xAngle, float t)
        {
            return new Vector2(
                (float)(-r.X * Math.Cos(xAngle) * Math.Sin(t) - r.Y * Math.Sin(xAngle) * Math.Cos(t)),
                (float)(-r.X * Math.Sin(xAngle) * Math.Sin(t) + r.Y * Math.Cos(xAngle) * Math.Cos(t)));
        }

        static IEnumerable<ILineSegment> EllipticArcToBezierCurveInner(Vector2 from, Vector2 center, Vector2 radius, float xAngle, float startAngle, float deltaAngle)
        {
            var s = startAngle;
            var e = s + deltaAngle;
            bool neg = e < s;
            float sign = neg ? -1 : 1;
            var remain = Math.Abs(e - s);

            var prev = EllipticArcPoint(center, radius, xAngle, s);

            while (remain > _zeroTolerance)
            {
                float step = (float)Math.Min(remain, Math.PI / 4);
                var signStep = step * sign;

                var p1 = prev;
                var p2 = EllipticArcPoint(center, radius, xAngle, s + signStep);

                float alphaT = (float)Math.Tan(signStep / 2);
                float alpha = (float)(Math.Sin(signStep) * (Math.Sqrt(4 + 3 * alphaT * alphaT) - 1) / 3);
                var q1 = p1 + alpha * EllipticArcDerivative(center, radius, xAngle, s);
                var q2 = p2 - alpha * EllipticArcDerivative(center, radius, xAngle, s + signStep);

                yield return new CubicBezierLineSegment(from, q1, q2, p2);
                from = p2;

                s += signStep;
                remain -= step;
                prev = p2;
            }
        }

        internal static void EndpointToCenterArcParams(Vector2 p1, Vector2 p2, ref Vector2 r_, float xAngle,
            bool flagA, bool flagS, out Vector2 c, out Vector2 angles)
        {
            double rX = Math.Abs(r_.X);
            double rY = Math.Abs(r_.Y);

            //(F.6.5.1)
            double dx2 = (p1.X - p2.X) / 2.0;
            double dy2 = (p1.Y - p2.Y) / 2.0;
            double x1p = Math.Cos(xAngle) * dx2 + Math.Sin(xAngle) * dy2;
            double y1p = -Math.Sin(xAngle) * dx2 + Math.Cos(xAngle) * dy2;

            //(F.6.5.2)
            double rxs = rX * rX;
            double rys = rY * rY;
            double x1ps = x1p * x1p;
            double y1ps = y1p * y1p;
            // check if the radius is too small `pq < 0`, when `dq > rxs * rys` (see below)
            // cr is the ratio (dq : rxs * rys) 
            double cr = x1ps / rxs + y1ps / rys;
            if (cr > 1)
            {
                //scale up rX,rY equally so cr == 1
                var s = Math.Sqrt(cr);
                rX = s * rX;
                rY = s * rY;
                rxs = rX * rX;
                rys = rY * rY;
            }
            double dq = (rxs * y1ps + rys * x1ps);
            double pq = (rxs * rys - dq) / dq;
            double q = Math.Sqrt(Math.Max(0, pq)); //use Max to account for float precision
            if (flagA == flagS)
                q = -q;
            double cxp = q * rX * y1p / rY;
            double cyp = -q * rY * x1p / rX;

            //(F.6.5.3)
            double cx = Math.Cos(xAngle) * cxp - Math.Sin(xAngle) * cyp + (p1.X + p2.X) / 2;
            double cy = Math.Sin(xAngle) * cxp + Math.Cos(xAngle) * cyp + (p1.Y + p2.Y) / 2;

            //(F.6.5.5)
            double theta = svgAngle(1, 0, (x1p - cxp) / rX, (y1p - cyp) / rY);
            //(F.6.5.6)
            double delta = svgAngle(
                (x1p - cxp) / rX, (y1p - cyp) / rY,
                (-x1p - cxp) / rX, (-y1p - cyp) / rY);
            delta = delta % (Math.PI * 2);
            if (!flagS)
            {
                delta -= 2 * Math.PI;
            }

            r_ = new Vector2((float)rX, (float)rY);
            c = new Vector2((float)cx, (float)cy);
            angles = new Vector2((float)theta, (float)delta);
        }

        static float Clamp(float val, float min, float max)
        {
            if (val < min) return min;
            else if (val > max) return max;
            else return val;
        }

        static float svgAngle(double ux, double uy, double vx, double vy)
        {
            var u = new Vector2((float)ux, (float)uy);
            var v = new Vector2((float)vx, (float)vy);
            //(F.6.5.4)
            var dot = Vector2.Dot(u, v);
            var len = u.Length() * v.Length();
            var ang = (float)Math.Acos(Clamp(dot / len, -1, 1)); //floating point precision, slightly over values appear
            if ((u.X * v.Y - u.Y * v.X) < 0)
            {
                ang = -ang;
            }

            return ang;
        }
    }
}
