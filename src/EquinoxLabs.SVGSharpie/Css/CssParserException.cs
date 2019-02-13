using System;

namespace EquinoxLabs.SVGSharpie.Css
{
    public class CssParserException : Exception
    {
        internal CssParserException(CssStringStreamReader reader, string message)
            : base($"'{reader.Stream.Substring(Math.Max(0, reader.Position - 10))}', {message}")
        {
        }
    }
}