using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    /// <summary>
    /// Represents a color in the RGB color space
    /// </summary>
    public struct RgbColor : IEquatable<RgbColor>
    {
        /// <summary>
        /// Gets a color with the RGB value of #000000
        /// </summary>
        public static readonly RgbColor Black = new RgbColor(0, 0, 0);

        /// <summary>
        /// Gets a color with the RGB value of #FFFFFF
        /// </summary>
        public static readonly RgbColor White = new RgbColor(255, 255, 255);

        /// <summary>
        /// Gets a color with the RGB value of #FF0000
        /// </summary>
        public static readonly RgbColor Red = new RgbColor(255, 0, 0);

        /// <summary>
        /// Gets a color with the RGB value of #00FF00
        /// </summary>
        public static readonly RgbColor Green = new RgbColor(0, 255, 0);

        /// <summary>
        /// Gets a color with the RGB value of #0000FF
        /// </summary>
        public static readonly RgbColor Blue = new RgbColor(0, 0, 255);

        /// <summary>
        /// Gets the Red component
        /// </summary>
        public readonly byte R;

        /// <summary>
        /// Gets the Green component
        /// </summary>
        public readonly byte G;

        /// <summary>
        /// Gets the Blue component
        /// </summary>
        public readonly byte B;

        /// <summary>
        /// Creates a new instance of <see cref="RgbColor"/> with the specified channel intensities
        /// </summary>
        public RgbColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Creates a new instance of <see cref="RgbColor"/> from the specified string value
        /// </summary>
        public RgbColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) throw new ArgumentException(nameof(color));
            if (color.StartsWith("#"))
            {
                int HexCharToNumber(char c)
                {
                    if (c >= '0' && c <= '9') return c - '0';
                    if (c >= 'a' && c <= 'f') return c - 'a' + 10;
                    if (c >= 'A' && c <= 'F') return c - 'A' + 10;
                    throw new Exception($"Invalid character '{c}' in color '{color}'");
                }

                byte HexStrToByte(string str, int index) => (byte)(HexCharToNumber(str[index]) * 16 + HexCharToNumber(str[index + 1]));

                R = HexStrToByte(color, 1);
                G = HexStrToByte(color, 3);
                B = HexStrToByte(color, 5);
            }
            else if (color.StartsWith("(") && color.EndsWith(")"))
            {
                var values = color.Substring(1, color.Length - 2).Split(',');
                if (values.Length != 3)
                {
                    throw new ArgumentException($"Expected (R,G,B) but got '{color}'", nameof(color));
                }
                R = byte.Parse(values[0]);
                G = byte.Parse(values[1]);
                B = byte.Parse(values[2]);
            }
            else
            {
                throw new ArgumentException($"Invalid color, expected hex or value encoded (e.g. '#7248BD', '(114, 72,189)') got '{color}'", nameof(color));
            }
        }

        /// <summary>
        /// Returns the hash code for the color structure
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Indicates whether the specified color is equal to the current color.
        /// </summary>
        public bool Equals(RgbColor other) => R == other.R && G == other.G && B == other.B;

        /// <summary>
        /// Indicates whether the specified object is equal to the current color.
        /// </summary>
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && obj is RgbColor rgb && Equals(rgb);

        /// <summary>
        /// Compares two colors for exact equality
        /// </summary>
        public static bool operator ==(RgbColor left, RgbColor right) => left.Equals(right);

        /// <summary>
        /// Compares two colors for inequality
        /// </summary>
        public static bool operator !=(RgbColor left, RgbColor right) => !left.Equals(right);

        /// <summary>
        /// Returns the color encoded as a hex string (e.g. '#ccffa0')
        /// </summary>
        public override string ToString() => $"#{R:x2}{G:x2}{B:x2}";
    }
}
