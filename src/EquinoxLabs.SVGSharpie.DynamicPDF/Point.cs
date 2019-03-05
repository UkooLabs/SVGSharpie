using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    /// <summary>
    /// Defines a point in a two-dimensional plane using a pair of x- and y- coordinates.
    /// </summary>
    [DataContract]
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// Represents a Point that has <see cref="X"/> and <see cref="Y"/> values set to zero.
        /// </summary>
        public static readonly Point Empty = new Point();

        /// <summary>
        /// Gets or sets the x-coordinate of this Point.
        /// </summary>
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of this Point.
        /// </summary>
        [DataMember]
        public double Y { get; set; }

        /// <summary>
        /// Gets a value indicating whether this Point is empty (both X and Y are 0, otherwise false)
        /// </summary>
        public bool IsEmpty =>
            Math.Abs(X) < double.Epsilon &&
            Math.Abs(Y) < double.Epsilon;

        /// <summary>
        /// Gets a value indicating whether both the x- and y- axis coordinates are the same
        /// </summary>
        public bool IsUniform =>
            Math.Abs(X - Y) < double.Epsilon;

        /// <summary>
        /// Initializes a new instance of the Point class with the specified coordinates.
        /// </summary>
        /// <param name="x">The horizontal position of the point.</param>
        /// <param name="y">The vertical position of the point.</param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and the specified object represent the same value.
        /// </summary>
        public bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);

        /// <summary>
        /// Returns a value indicating whether this instance and the specified object represent the same value.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point && Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a value indicating whether the two specified points represent the same value.
        /// </summary>
        public static bool operator ==(Point left, Point right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating whether the two specified points do not represent the same value.
        /// </summary>
        public static bool operator !=(Point left, Point right) => !left.Equals(right);
    }
}
