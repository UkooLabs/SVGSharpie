using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    /// <summary>
    /// Describes the type of spot color
    /// </summary>
    public enum SpotColorType
    {
        /// <summary>
        /// Pantone proprietary color space
        /// </summary>
        Pantone,

        /// <summary>
        /// 
        /// </summary>
        Generic
    }

    /// <summary>
    /// Represents a spot color, composed of a type and a descriptor specific to that type
    /// </summary>
    public struct SpotColor : IEquatable<SpotColor>
    {
        /// <summary>
        /// Gets the type of the spot color
        /// </summary>
        public readonly SpotColorType SpotColorType;

        /// <summary>
        /// Gets the ink name of the spot color
        /// </summary>
        public readonly string InkName;

        /// <summary>
        /// Creates a new instance of spot color of a specific type and descriptor
        /// </summary>
        public SpotColor(SpotColorType spotColorType, string inkName)
        {
            SpotColorType = spotColorType;
            InkName = inkName ?? throw new ArgumentNullException(nameof(inkName));
        }

        /// <summary>
        /// Creates a new instance of spot color by parsing the specified color string
        /// </summary>
        /// <param name="color">string representation to parse the type and descriptor from</param>
        public SpotColor(string color)
        {
            if (color == null) throw new ArgumentNullException(nameof(color));
            if (color.StartsWith("(") && color.EndsWith(")"))
            {
                color = color.Substring(1, color.Length - 2);
            }
            var splitIndex = color.IndexOf('-');
            if (splitIndex < 0)
            {
                throw new ArgumentException(nameof(color));
            }
            var spotColorTypeStr = color.Substring(0, splitIndex);
            if (!Enum.TryParse(spotColorTypeStr, true, out SpotColorType))
            {
                throw new ArgumentException($"Invalid spot color type '{spotColorTypeStr}'", nameof(color));
            }
            InkName = color.Substring(splitIndex + 1);
            if (string.IsNullOrWhiteSpace(InkName))
            {
                throw new ArgumentException("Missing spot color value", nameof(color));
            }
        }

        /// <summary>
        /// Returns the string representation of the color
        /// </summary>
        public override string ToString() => $"{SpotColorType}-{InkName}";

        /// <summary>
        /// Indicates whether the specified color is equal to the current color.
        /// </summary>
        public bool Equals(SpotColor other) => SpotColorType == other.SpotColorType && string.Equals(InkName, other.InkName);

        /// <summary>
        /// Indicates whether the specified object is equal to the current color.
        /// </summary>
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && obj is SpotColor spot && Equals(spot);

        /// <summary>
        /// Returns the hash code for the color structure
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)SpotColorType * 397) ^ (InkName != null ? InkName.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Compares two colors for exact equality
        /// </summary>
        public static bool operator ==(SpotColor left, SpotColor right) => left.Equals(right);

        /// <summary>
        /// Compares two colors for inequality
        /// </summary>
        public static bool operator !=(SpotColor left, SpotColor right) => !left.Equals(right);
    }
}
