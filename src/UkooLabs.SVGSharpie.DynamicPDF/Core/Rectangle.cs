using System.Runtime.Serialization;

namespace UkooLabs.SVGSharpie.DynamicPDF.Core
{
    /// <summary>
    /// Describes the width, height and location of a rectangle
    /// </summary>
    [DataContract]
    public struct Rectangle
    {
        /// <summary>
        /// Gets or sets the x-axis value of the left side of the rectangle.
        /// </summary>
        [DataMember(Name = "X")]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y-axis value of the top side of the rectangle.
        /// </summary>
        [DataMember(Name = "Y")]
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
        /// Initializes a new instance of the Rectangle structure that has the specified x-coordinate, y-coordinate, width, and height.
        /// </summary>
        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
