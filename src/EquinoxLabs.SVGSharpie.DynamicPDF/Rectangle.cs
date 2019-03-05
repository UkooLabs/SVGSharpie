using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    /// <summary>
    /// Describes the width, height and location of a rectangle
    /// </summary>
    [DataContract]
    public struct Rectangle
    {
        /// <summary>
        /// Gets a special value that represents a rectangle with no position or area.
        /// </summary>
        public static Rectangle Empty { get; }

        /// <summary>
        /// Gets or sets the x-axis value of the left side of the rectangle.
        /// </summary>
        [DataMember(Name = "Left")]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y-axis value of the top side of the rectangle.
        /// </summary>
        [DataMember(Name = "Top")]
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the rectangle
        /// </summary>
        [DataMember]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the rectangle
        /// </summary>
        [DataMember]
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the size of the rectangle
        /// </summary>
        [IgnoreDataMember]
        public Size Size
        {
            get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current rectangle is the <see cref="Empty"/> rectangle
        /// </summary>
        public bool IsEmpty =>
            double.IsPositiveInfinity(X) &&
            double.IsPositiveInfinity(Y) &&
            double.IsNegativeInfinity(Width) &&
            double.IsNegativeInfinity(Height);

        /// <summary>
        /// Gets the x-axis value of the left side of the rectangle.
        /// </summary>
        [IgnoreDataMember]
        public double Left => X;

        /// <summary>
        /// Gets the y-axis value of the top side of the rectangle.
        /// </summary>
        [IgnoreDataMember]
        public double Top => Y;

        /// <summary>
        /// Gets the x-axis value of the right side of the rectangle.
        /// </summary>
        [IgnoreDataMember]
        public double Right => X + Width;

        /// <summary>
        /// Gets the y-axis value of the bottom of the rectangle.
        /// </summary>
        [IgnoreDataMember]
        public double Bottom => Y + Height;

        /// <summary>
        /// Gets the top left point of the rectangle.
        /// </summary>
        [IgnoreDataMember]
        public Point TopLeft => new Point(Left, Top);

        /// <summary>
        /// Gets the top right point of the rectangle.
        /// </summary>
        [IgnoreDataMember]
        public Point TopRight => new Point(Right, Top);

        /// <summary>
        /// Gets the bottom left point of the rectangle.
        /// </summary>
        [IgnoreDataMember]
        public Point BottomLeft => new Point(Left, Bottom);

        /// <summary>
        /// Gets the bottom right point of the rectangle.
        /// </summary>
        [IgnoreDataMember]
        public Point BottomRight => new Point(Right, Bottom);

        static Rectangle()
        {
            Empty = new Rectangle
            {
                X = double.PositiveInfinity,
                Y = double.PositiveInfinity,
                Width = double.NegativeInfinity,
                Height = double.NegativeInfinity
            };
        }

        /// <summary>
        /// Initializes a new instance of the Rectangle structure that has the specified x-coordinate, y-coordinate, width, and height.
        /// </summary>
        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the Rectangle structure that has the specified width, and height and is located at (0,0).
        /// </summary>
        public Rectangle(double width, double height)
            : this(0, 0, width, height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Rectangle structure that has the specified size and is located at (0,0).
        /// </summary>
        public Rectangle(Size size)
            : this(size.Width, size.Height)
        {
        }
    }
}
