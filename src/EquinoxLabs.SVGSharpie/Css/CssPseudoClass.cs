namespace EquinoxLabs.SVGSharpie.Css
{
    public abstract class CssPseudoClass
    {
        public abstract CssPseudoClassType PseudoClassType { get; }

        internal CssPseudoClass()
        {
        }
    }
}