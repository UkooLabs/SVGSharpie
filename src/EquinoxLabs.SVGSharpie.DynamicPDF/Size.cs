using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    /// <summary>
    /// Describes the size of an object
    /// </summary>
    [DataContract]
    public struct Size
    {
        private double _width;
        private double _height;

        /// <summary>
        /// Gets a special size that represents a size with no area
        /// </summary>
        public static Size Empty { get; }

        /// <summary>
        /// Gets a value indicating whether the current size is the <see cref="Empty"/> size
        /// </summary>
        public bool IsEmpty => double.IsNegativeInfinity(_width) && double.IsNegativeInfinity(_height);

        /// <summary>
        /// Gets or sets the width of the current size
        /// </summary>
        [DataMember]
        public double Width
        {
            get => _width;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the current size
        /// </summary>
        [DataMember]
        public double Height
        {
            get => _height;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _height = value;
            }
        }

        static Size()
        {
            Empty = new Size
            {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };
        }

        public Size(double width, double height)
        {
            if (width < 0) {
                throw new ArgumentOutOfRangeException(nameof(width));
            }
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }
            _width = width;
            _height = height;
        }
    }
}
