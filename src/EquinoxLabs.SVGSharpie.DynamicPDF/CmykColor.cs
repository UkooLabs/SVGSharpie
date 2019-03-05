using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    /// <summary>
    /// Represents a color in the CMYK color space
    /// </summary>
    public struct CmykColor : IEquatable<CmykColor>
    {
        /// <summary>
        /// Gets a color with CMYK values of (0, 0, 0, 100)
        /// </summary>
        public static readonly CmykColor Black = new CmykColor(0, 0, 0, 100);

        /// <summary>
        /// Gets a color with CMYK values of (0, 0, 0, 0)
        /// </summary>
        public static readonly CmykColor White = new CmykColor(0, 0, 0, 0);

        /// <summary>
        /// Gets a color with CMYK values of (100, 0, 0, 0)
        /// </summary>
        public static readonly CmykColor Cyan = new CmykColor(100, 0, 0, 0);

        /// <summary>
        /// Gets a color with CMYK values of (0, 100, 0, 0)
        /// </summary>
        public static readonly CmykColor Magenta = new CmykColor(0, 100, 0, 0);

        /// <summary>
        /// Gets a color with CMYK values of (0, 0, 100, 0)
        /// </summary>
        public static readonly CmykColor Yellow = new CmykColor(0, 0, 100, 0);

        /// <summary>
        /// Gets the Cyan component (0-100)
        /// </summary>
        public readonly byte C;

        /// <summary>
        /// Gets the Magenta component (0-100)
        /// </summary>
        public readonly byte M;

        /// <summary>
        /// Gets the Yellow component (0-100)
        /// </summary>
        public readonly byte Y;

        /// <summary>
        /// Gets the Black component (0-100)
        /// </summary>
        public readonly byte K;

        /// <summary>
        /// Creates a new instance of <see cref="CmykColor"/>
        /// </summary>
        /// <param name="c">cyan channel intensity between 0..100</param>
        /// <param name="m">magenta channel intensity between 0..100</param>
        /// <param name="y">yellow channel intensity between 0..100</param>
        /// <param name="k">black channel intensity between 0..100</param>
        public CmykColor(byte c, byte m, byte y, byte k)
        {
            C = EnsureValidIntensity(c);
            M = EnsureValidIntensity(m);
            Y = EnsureValidIntensity(y);
            K = EnsureValidIntensity(k);
        }

        /// <summary>
        /// Creates a new instance of <see cref="CmykColor"/> from the values in the specified string
        /// </summary>
        public CmykColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) throw new ArgumentException(nameof(color));
            if (color.StartsWith("#"))
            {
                C = byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber);
                M = byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber);
                Y = byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber);
                K = byte.Parse(color.Substring(7, 2), NumberStyles.HexNumber);
            }
            else if (color.StartsWith("(") && color.EndsWith(")"))
            {
                var values = color.Substring(1, color.Length - 2).Split(',');
                if (values.Length != 4)
                {
                    throw new ArgumentException($"Expected (C,M,Y,K) but got '{color}'", nameof(color));
                }
                C = byte.Parse(values[0]);
                M = byte.Parse(values[1]);
                Y = byte.Parse(values[2]);
                K = byte.Parse(values[3]);
            }
            else
            {
                throw new ArgumentException("Invalid color, expected hex or value encoded (e.g. '#B8BDB900', '(72, 74, 73, 0)')", nameof(color));
            }
        }

        /// <summary>
        /// Returns the hash code for the color structure
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = C.GetHashCode();
                hashCode = (hashCode * 397) ^ M.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ K.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Indicates whether the specified color is equal to the current color.
        /// </summary>
        public bool Equals(CmykColor other) => C == other.C && M == other.M && Y == other.Y && K == other.K;

        /// <summary>
        /// Indicates whether the specified object is equal to the current color.
        /// </summary>
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && obj is CmykColor cmyk && Equals(cmyk);

        /// <summary>
        /// Compares two colors for exact equality
        /// </summary>
        public static bool operator ==(CmykColor left, CmykColor right) => left.Equals(right);

        /// <summary>
        /// Compares two colors for inequality
        /// </summary>
        public static bool operator !=(CmykColor left, CmykColor right) => !left.Equals(right);

        /// <summary>
        /// Returns the string representation of the color
        /// </summary>
        public override string ToString() => $"#{C:x2}{M:x2}{Y:x2}{K:x2}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte EnsureValidIntensity(byte value) => value <= 100 ? value : throw new ArgumentOutOfRangeException(nameof(value), $"channel intensity should be between 0..100 but got {value}");
    }
}
