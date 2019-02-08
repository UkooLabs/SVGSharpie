using System.Collections.Generic;

namespace PNI.Graphics.Svg.Css
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a comma separated list of <see cref="T:PNI.Graphics.Svg.Css.CssSelector" />s
    /// </summary>
    public sealed class CssSelectorList : List<CssSelector>
    {
        public override string ToString() => string.Join(", ", this);
    }
}