using System.Xml.Serialization;

namespace PNI.Graphics.Svg
{
    /// <summary>
    /// Specifies the shape to be used at the corners of paths or basic shapes when they are stroked
    /// </summary>
    public enum SvgStrokeLineJoin
    {
        [XmlEnum("miter")]
        Miter,
        [XmlEnum("round")]
        Round,
        [XmlEnum("bevel")]
        Bevel,
        [XmlEnum("inherit")]
        Inherit
    }
}