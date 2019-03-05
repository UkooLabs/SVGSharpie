using System;
using ceTe.DynamicPDF;

namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements.Shading
{
    internal struct GradientColorStop : IEquatable<GradientColorStop>
    {
        public DeviceColor Color { get; }

        public float Offset { get; }

        public GradientColorStop(DeviceColor color, float offset)
        {
            Color = color ?? throw new ArgumentNullException(nameof(color));
            Offset = offset;
        }

        public bool Equals(GradientColorStop other) => Color.Equals(other.Color) && Offset.Equals(other.Offset);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GradientColorStop stop && Equals(stop);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Color.GetHashCode() * 397) ^ Offset.GetHashCode();
            }
        }
        
        public static bool operator ==(GradientColorStop left, GradientColorStop right) => left.Equals(right);

        public static bool operator !=(GradientColorStop left, GradientColorStop right) => !left.Equals(right);
    }
}