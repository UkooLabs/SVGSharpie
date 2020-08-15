using System;

namespace UkooLabs.SVGSharpie
{
    internal abstract class SvgLengthContext
    {
        public static SvgLengthContext Null { get; } = new NullContext();

        /// <summary>
        /// Get elements size of font
        /// </summary>
        public abstract float GetFontSize();

        /// <summary>
        /// Calculates the total length in the direction of an <see cref="SvgLength"/>
        /// </summary>
        public abstract float ComputeTotalLength();
        
        private sealed class NullContext : SvgLengthContext
        {
            public override float GetFontSize() => throw new Exception("Unable to get font size");
            public override float ComputeTotalLength() => throw new Exception("Unable to compute total length");
        }
    }
}