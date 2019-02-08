using System;
using System.Collections.Generic;
using PNI.Graphics.Svg.Css;

namespace PNI.Graphics.Svg
{
    public sealed class SvgStyleRule
    {
        /// <summary>
        /// Gets the selectors of the current rule
        /// </summary>
        public IReadOnlyList<CssSelector> Selectors { get; }

        /// <summary>
        /// Gets the body of the current rule
        /// </summary>
        public SvgElementStyleData Rules { get; } = new SvgElementStyleData();

        public SvgStyleRule(CssStyleRule cssStyleRule)
        {
            Selectors = cssStyleRule?.Selectors ?? throw new ArgumentNullException(nameof(cssStyleRule));
            foreach (var p in cssStyleRule.Properties)
            {
                if (!Rules.TryPopulateProperty(p.Key, p.Value))
                {
                    throw new Exception($"Unknown style property '{p.Key}:{p.Value}'");
                }
            }
        }

        public override string ToString() => string.Join(", ", Selectors);
    }
}