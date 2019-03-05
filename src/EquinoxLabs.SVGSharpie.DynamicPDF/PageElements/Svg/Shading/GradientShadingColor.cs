using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.IO;

namespace PNI.Apollo.Render.Services.DynamicPdf.PageElements.Shading
{
    internal abstract class GradientShadingColor<T> : GradientShadingColor, IEquatable<T>
        where T : GradientShadingColor<T>
    {
        protected GradientShadingColor(IEnumerable<GradientColorStop> stops) : base(stops)
        {
        }

        public abstract bool Equals(T other);
    }

    internal abstract class GradientShadingColor : Pattern, IEquatable<GradientShadingColor>
    {
        public IReadOnlyList<GradientColorStop> Stops { get; }

        /// <summary>
        /// Gets the PDF ShadingType
        /// </summary>
        protected abstract ShadingType ShadingType { get; }

        /// <summary>
        /// Emits the array of numbers specifying the starting and ending coordinates expressed in the targets coordinate
        /// space.  These will then be mapped to the parent coordinate space by the matrix specified emitted by the
        /// <see cref="DrawMatrix"/> method.
        /// </summary>
        protected abstract void DrawCoordinates(Action<float> writeNumber);

        /// <summary>
        /// Emits the 6-values of the matrix to map the patterns <see cref="DrawCoordinates">internal coordinate system</see>
        /// to the default coordinate system of the pattern's parent content stream.
        /// </summary>
        protected abstract void DrawMatrix(Action<float> writeNumber);

        protected GradientShadingColor(IEnumerable<GradientColorStop> stops)
        {
            Stops = stops?.ToArray() ?? throw new ArgumentNullException(nameof(stops));
            if (Stops.Count < 2)
            {
                throw new ArgumentException($"Expected at least 2 color stop, got {Stops.Count}", nameof(stops));
            }
            var colorSpace = Stops[0].Color.ColorSpace;
            if (Stops.Any(i => i.Color.ColorSpace != colorSpace))
            {
                throw new ArgumentException($"Mixed colorspace stops are not supported, expected all to be in the {colorSpace} space", nameof(stops));
            }
            _resource = new ShadingColorResource(this);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = ShadingType.GetHashCode();
                for (var i = 0; i < Stops.Count; i++)
                {
                    result *= 397;
                    result ^= Stops[i].GetHashCode();
                }
                return result;
            }
        }

        public override void DrawStroke(PageWriter writer)
        {
            writer.SetStrokeColorSpace(ColorSpace.Pattern);
            writer.Resources.Patterns.Add(_resource, writer);       // stealthily writes the /P operator :(
            writer.Write(OperatorSCNBytes);
        }

        public override void DrawFill(PageWriter writer)
        {
            writer.SetFillColorSpace(ColorSpace.Pattern);
            writer.Resources.Patterns.Add(_resource, writer);       // stealthily writes the /P operator :(
            writer.Write(Operator_scnBytes);
        }

        public bool Equals(GradientShadingColor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ShadingType != other.ShadingType) return false;
            if (Stops.Count != other.Stops.Count)
            {
                return false;
            }

            for (var i = 0; i < Stops.Count; i++)
            {
                if (Stops[i] != other.Stops[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GradientShadingColor)obj);
        }

        private readonly ShadingColorResource _resource;

        private static readonly byte[] OperatorSCNBytes = Encoding.ASCII.GetBytes(" SCN\n");
        private static readonly byte[] Operator_scnBytes = Encoding.ASCII.GetBytes(" scn\n");

        private static readonly byte[] NameFunctionTypeBytes = Encoding.ASCII.GetBytes("FunctionType");
        private static readonly byte[] NameFunctionsBytes = Encoding.ASCII.GetBytes("Functions");
        private static readonly byte[] NameBoundsBytes = Encoding.ASCII.GetBytes("Bounds");
        private static readonly byte[] NameMatrixBytes = Encoding.ASCII.GetBytes("Matrix");
        private static readonly byte[] NameFunctionBytes = Encoding.ASCII.GetBytes("Function");
        private static readonly byte[] NameDomainBytes = Encoding.ASCII.GetBytes("Domain");
        private static readonly byte[] NameExtendBytes = Encoding.ASCII.GetBytes("Extend");
        private static readonly byte[] NameEncodeBytes = Encoding.ASCII.GetBytes("Encode");
        private static readonly byte[] NameNBytes = Encoding.ASCII.GetBytes("N");
        private static readonly byte[] NameC0Bytes = Encoding.ASCII.GetBytes("C0");
        private static readonly byte[] NameC1Bytes = Encoding.ASCII.GetBytes("C1");

        private static readonly byte[] NamePatternTypeBytes = Encoding.ASCII.GetBytes("PatternType");
        private static readonly byte[] NameShadingBytes = Encoding.ASCII.GetBytes("Shading");
        private static readonly byte[] NameShadingTypeBytes = Encoding.ASCII.GetBytes("ShadingType");
        private static readonly byte[] NameColorSpaceBytes = Encoding.ASCII.GetBytes("ColorSpace");
        private static readonly byte[] NameCoordsBytes = Encoding.ASCII.GetBytes("Coords");
        
        private sealed class ShadingColorResource : Resource
        {
            private readonly GradientShadingColor _shadingColor;

            public ShadingColorResource(GradientShadingColor shadingColor)
            {
                _shadingColor = shadingColor ?? throw new ArgumentNullException(nameof(shadingColor));
            }

            public override void Draw(DocumentWriter writer)
            {
                const int shadingPatternType = 2;           // tiling pattern = 1

                void WriteNumberHighPrecision(float v)
                {
                    // WriteNumber isn't high precision enough, only supports 2 decimal places :(

                    var bytes = Encoding.ASCII.GetBytes($"{v} ");
                    writer.Write(bytes);
                }

                writer.WriteBeginObject();
                writer.WriteDictionaryOpen();
                {
                    writer.WriteName(NamePatternTypeBytes);
                    writer.WriteNumber(shadingPatternType);

                    writer.WriteName(NameMatrixBytes);
                    writer.WriteArrayOpen();
                    {
                        _shadingColor.DrawMatrix(WriteNumberHighPrecision);
                    }
                    writer.WriteArrayClose();

                    writer.WriteName(NameShadingBytes);
                    writer.WriteDictionaryOpen();
                    {
                        writer.WriteName(NameShadingTypeBytes);
                        writer.WriteNumber((int)_shadingColor.ShadingType);

                        writer.WriteName(NameColorSpaceBytes);
                        _shadingColor.Stops[0].Color.ColorSpace.DrawColorSpace(writer);

                        writer.WriteName(NameCoordsBytes);
                        writer.WriteArrayOpen();
                        {
                            _shadingColor.DrawCoordinates(WriteNumberHighPrecision);
                        }
                        writer.WriteArrayClose();

                        writer.WriteName(NameExtendBytes);
                        writer.WriteArrayOpen();
                        {
                            writer.WriteBoolean(true);
                            writer.WriteBoolean(true);
                        }
                        writer.WriteArrayClose();

                        writer.WriteName(NameFunctionBytes);

                        if (_shadingColor.Stops.Count > 2)
                        {
                            WriteStitchingFunction(writer);
                        }
                        else if (_shadingColor.Stops.Count == 2)
                        {
                            var stop1 = _shadingColor.Stops[0];
                            var stop2 = _shadingColor.Stops[1];
                            WriteLinearColorInterpolationFunction(writer, stop1.Color, stop2.Color);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Expected two or more color stops");
                        }
                    }
                    writer.WriteDictionaryClose();
                }
                writer.WriteDictionaryClose();
                writer.WriteEndObject();
            }

            private void WriteStitchingFunction(DocumentWriter writer)
            {
                const int stitchingFunctionType = 3;

                var stops = _shadingColor.Stops;

                writer.WriteDictionaryOpen();
                {
                    writer.WriteName(NameFunctionTypeBytes);
                    writer.WriteNumber(stitchingFunctionType);

                    WriteDomain0To1NameAndArray(writer);

                    writer.WriteName(NameFunctionsBytes);
                    writer.WriteArrayOpen();
                    {
                        for (var i = 0; i < stops.Count - 1; i++)
                        {
                            var stop = stops[i];
                            var next = stops[i + 1];
                            WriteLinearColorInterpolationFunction(writer, stop.Color, next.Color);
                        }
                    }
                    writer.WriteArrayClose();

                    writer.WriteName(NameBoundsBytes);
                    writer.WriteArrayOpen();
                    {
                        for (var i = 1; i < stops.Count - 1; i++)
                        {
                            writer.WriteNumber(stops[i].Offset);
                        }
                    }
                    writer.WriteArrayClose();

                    writer.WriteName(NameEncodeBytes);
                    writer.WriteArrayOpen();
                    {
                        for (var i = 0; i < stops.Count - 1; i++)
                        {
                            writer.WriteNumber(0);
                            writer.WriteNumber(1);
                        }
                    }
                    writer.WriteArrayClose();
                }
                writer.WriteDictionaryClose();
            }

            private static void WriteLinearColorInterpolationFunction(DocumentWriter writer, DeviceColor color1, DeviceColor color2)
            {
                const int interpolationFunctionType = 2;

                void WriteColorArray(DeviceColor color)
                {
                    writer.WriteArrayOpen();
                    switch (color)
                    {
                        case RgbColor rgb:
                            writer.WriteNumberColor(rgb.R);
                            writer.WriteNumberColor(rgb.G);
                            writer.WriteNumberColor(rgb.B);
                            break;
                        case CmykColor cmyk:
                            writer.WriteNumberColor(cmyk.C);
                            writer.WriteNumberColor(cmyk.M);
                            writer.WriteNumberColor(cmyk.Y);
                            writer.WriteNumberColor(cmyk.K);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    writer.WriteArrayClose();
                }

                writer.WriteDictionaryOpen();
                {
                    writer.WriteName(NameFunctionTypeBytes);
                    writer.WriteNumber(interpolationFunctionType);
                    writer.WriteName(NameNBytes);
                    writer.WriteNumber1();
                    WriteDomain0To1NameAndArray(writer);
                    writer.WriteName(NameC0Bytes);
                    WriteColorArray(color1);
                    writer.WriteName(NameC1Bytes);
                    WriteColorArray(color2);
                }
                writer.WriteDictionaryClose();
            }

            private static void WriteDomain0To1NameAndArray(DocumentWriter writer)
            {
                writer.WriteName(NameDomainBytes);
                writer.WriteArrayOpen();
                {
                    writer.WriteNumber0();
                    writer.WriteNumber1();
                }
                writer.WriteArrayClose();
            }
        }
    }
}