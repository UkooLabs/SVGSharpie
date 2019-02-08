namespace PNI.Graphics.Svg
{
    public struct SvgColor
    {
        public static readonly SvgColor Black = new SvgColor(0, 0, 0);

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public SvgColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public override string ToString() => $"#{R:x2}{G:x2}{B:x2}";
    }
}