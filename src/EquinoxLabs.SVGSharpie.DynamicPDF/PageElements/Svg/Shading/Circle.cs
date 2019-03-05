using System;

namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements.Shading
{
    internal struct Circle : IEquatable<Circle>
    {
        public float X { get; }

        public float Y { get; }

        public float R { get; }

        public Circle(float x, float y, float r)
        {
            X = x;
            Y = y;
            R = r;
        }

        public bool Equals(Circle other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && R.Equals(other.R);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Circle circle && Equals(circle);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ R.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Circle left, Circle right) => left.Equals(right);

        public static bool operator !=(Circle left, Circle right) => !left.Equals(right);
    }
}