using System;
using System.Xml.Serialization;

namespace PNI.Graphics.Svg
{
    /// <inheritdoc />
    /// <summary>
    /// The SVGTextElement interface corresponds to the ‘text’ element.
    /// </summary>
    [XmlType("text", Namespace = SvgDocument.SvgNs)]
    public sealed class SvgTextElement : SvgGraphicsElement
    {
        public override SvgRect? GetBBox() => throw new NotImplementedException();

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override void Accept(SvgElementVisitor visitor) => visitor.VisitTextElement(this);

        /// <inheritdoc cref="SvgElement.Accept"/>
        public override TResult Accept<TResult>(SvgElementVisitor<TResult> visitor) => visitor.VisitTextElement(this);

        protected override SvgElement CreateClone() => new SvgTextElement();
    }
}