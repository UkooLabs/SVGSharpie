using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    /// <summary>
    /// Represents a color in multiple color spaces
    /// </summary>
    [DataContract]
    public struct Color : IEquatable<Color>
    {
        /// <summary>
        /// Gets the system defined No color
        /// </summary>
        public static readonly Color NoColor = new Color();

        /// <summary>
        /// Gets the system defined Black color
        /// </summary>
        public static readonly Color Black = new Color(EquinoxLabs.SVGSharpie.DynamicPDF.RgbColor.Black, EquinoxLabs.SVGSharpie.DynamicPDF.CmykColor.Black);

        /// <summary>
        /// Gets the system defined White color
        /// </summary>
        public static readonly Color White = new Color(EquinoxLabs.SVGSharpie.DynamicPDF.RgbColor.White, EquinoxLabs.SVGSharpie.DynamicPDF.CmykColor.White);

        /// <summary>
        /// Gets the color in the RGB color space
        /// </summary>
        [DataMember]
        public RgbColor? RgbColor { get; private set; }

        /// <summary>
        /// Gets the color in the CMYK color space
        /// </summary>
        [DataMember]
        public CmykColor? CmykColor { get; private set; }

        /// <summary>
        /// Gets the color in the Spot color space
        /// </summary>
        [DataMember]
        public SpotColor? SpotColor { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current color is empty (no values specified)
        /// </summary>
        [IgnoreDataMember]
        public bool IsNone => !RgbColor.HasValue && !CmykColor.HasValue && !SpotColor.HasValue;

        /// <summary>
        /// Creates a new instance color with just an RGB representation
        /// </summary>
        public Color(RgbColor rgb)
        {
            RgbColor = rgb;
            CmykColor = null;
            SpotColor = null;
        }

        /// <summary>
        /// Creates a new instance color with just a CMYK representation
        /// </summary>
        public Color(CmykColor cmyk)
        {
            RgbColor = null;
            CmykColor = cmyk;
            SpotColor = null;
        }

        /// <summary>
        /// Creates a new instance color with RGB, CMYK and Spot representations
        /// </summary>
        public Color(RgbColor? rgb, CmykColor? cmyk = null, SpotColor? spot = null)
        {
            RgbColor = rgb;
            CmykColor = cmyk;
            SpotColor = spot;
        }

        /// <summary>
        /// Creates a new instance color from the specified string representation
        /// </summary>
        public Color(string color)
        {
            if (color == null) throw new ArgumentNullException(nameof(color));
            RgbColor = null;
            CmykColor = null;
            SpotColor = null;

            if (color.StartsWith("#"))
            {
                RgbColor = new RgbColor(color);
                return;
            }

            void ParseColorSpace(ref Color self, string space)
            {
                var pair = space.Split(ColorSpaceValueSplitChars, StringSplitOptions.RemoveEmptyEntries);
                var colorSpace = pair[0];
                var colorValue = pair.Length == 2 ? pair[1] : string.Empty;
                if (pair.Length != 2)
                {
                    var index = space.IndexOf("(", StringComparison.OrdinalIgnoreCase);
                    if (index < 0)
                    {
                        throw new Exception($"Invalid color space value '{space}'");
                    }
                    colorSpace = space.Substring(0, index);
                    colorValue = space.Substring(index);
                }
                if (ColorSpaceInitializers.TryGetValue(colorSpace.ToLowerInvariant(), out var initializer))
                {
                    initializer(ref self, colorValue);
                }
                else
                {
                    throw new ArgumentException($"Invalid color space prefix '{colorSpace}', expected one of [{RgbSpacePrefix}, {CmykSpacePrefix}, {SpotSpacePrefix}]");
                }
            }

            var builder = new StringBuilder();
            var scope = 0;
            for (var i = 0; i < color.Length; i++)
            {
                var c = color[i];
                if (c == ColorSpaceSplitChar && scope == 0)
                {
                    var space = builder.ToString();
                    ParseColorSpace(ref this, space);
                    builder.Clear();
                }
                else if (!char.IsWhiteSpace(c))
                {
                    if (c == '(') scope++;
                    else if (c == ')') scope--;
                    builder.Append(c);
                }
            }
            if (builder.Length > 0)
            {
                ParseColorSpace(ref this, builder.ToString());
            }
        }

        /// <summary>
        /// Returns the string representation of the current color
        /// </summary>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (RgbColor.HasValue && !CmykColor.HasValue && !SpotColor.HasValue)
            {
                return RgbColor.Value.ToString();
            }
            if (RgbColor.HasValue)
            {
                builder.Append(RgbSpacePrefix).Append(ColorSpaceValueSplitChar).Append(RgbColor.Value);
            }
            if (CmykColor.HasValue)
            {
                if (builder.Length > 0) builder.Append(ColorSpaceSplitChar);
                builder.Append(CmykSpacePrefix).Append(ColorSpaceValueSplitChar).Append(CmykColor.Value);
            }
            if (SpotColor.HasValue)
            {
                if (builder.Length > 0) builder.Append(ColorSpaceSplitChar);
                builder.Append(SpotSpacePrefix).Append(ColorSpaceValueSplitChar).Append(SpotColor.Value);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Indicates whether the specified color is equal to the current color.
        /// </summary>
        public bool Equals(Color other) => RgbColor.Equals(other.RgbColor) && CmykColor.Equals(other.CmykColor) && SpotColor.Equals(other.SpotColor);

        /// <summary>
        /// Indicates whether the specified object is equal to the current color.
        /// </summary>
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && obj is Color color && Equals(color);

        /// <summary>
        /// Compares two colors for exact equality
        /// </summary>
        public static bool operator ==(Color left, Color right) => left.Equals(right);

        /// <summary>
        /// Compares two colors for inequality
        /// </summary>
        public static bool operator !=(Color left, Color right) => !left.Equals(right);

        /// <summary>
        /// Returns the hash code for the color structure
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                var hashCode = RgbColor.GetHashCode();
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hashCode = (hashCode * 397) ^ CmykColor.GetHashCode();
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hashCode = (hashCode * 397) ^ SpotColor.GetHashCode();
                return hashCode;
            }
        }

        private const string RgbSpacePrefix = "rgb";
        private const string CmykSpacePrefix = "cmyk";
        private const string SpotSpacePrefix = "spot";

        private const char ColorSpaceSplitChar = ',';
        private const char ColorSpaceValueSplitChar = ':';
        private static readonly char[] ColorSpaceValueSplitChars = { ColorSpaceValueSplitChar };

        private delegate void ColorSpaceInitializerDelegate(ref Color self, string value);

        private static readonly Dictionary<string, ColorSpaceInitializerDelegate> ColorSpaceInitializers = new Dictionary<string, ColorSpaceInitializerDelegate>
        {
            { RgbSpacePrefix, (ref Color self, string value) => self.RgbColor = new RgbColor(value) },
            { CmykSpacePrefix, (ref Color self, string value) => self.CmykColor = new CmykColor(value) },
            { SpotSpacePrefix, (ref Color self, string value) => self.SpotColor = new SpotColor(value) }
        };
    }
}
