using System;
using System.Collections.Generic;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

using SixLabors.Primitives;

namespace SixLabors.ImageSharp.Tests.TestUtilities.ImageComparison
{
    public class ExactImageComparer : ImageComparer
    {
        public static ExactImageComparer Instance { get; } = new ExactImageComparer();

        public override ImageSimilarityReport<TPixelA, TPixelB> CompareImagesOrFrames<TPixelA, TPixelB>(
            ImageFrame<TPixelA> expected,
            ImageFrame<TPixelB> actual)
        {
            if (expected.Size() != actual.Size())
            {
                throw new InvalidOperationException("Calling ImageComparer is invalid when dimensions mismatch!");
            }

            int width = actual.Width;

            // TODO: Comparing through Rgba64 may not be robust enough because of the existance of super high precision pixel types.

            var aBuffer = new Rgba32[width];
            var bBuffer = new Rgba32[width];

            var differences = new List<PixelDifference>();

            for (int y = 0; y < actual.Height; y++)
            {
                Span<TPixelA> aSpan = expected.GetPixelRowSpan(y);
                Span<TPixelB> bSpan = actual.GetPixelRowSpan(y);
                for(var i = 0; i<aSpan.Length; i++) { aSpan[i].ToRgba32(ref aBuffer[i]); }
                for(var i = 0; i<bSpan.Length; i++) { bSpan[i].ToRgba32(ref bBuffer[i]); }

                for (int x = 0; x < width; x++)
                {
                    Rgba32 aPixel = aBuffer[x];
                    Rgba32 bPixel = bBuffer[x];

                    if (aPixel != bPixel)
                    {
                        var diff = new PixelDifference(new Point(x, y), aPixel, bPixel);
                        differences.Add(diff);
                    }
                }
            }

            return new ImageSimilarityReport<TPixelA, TPixelB>(expected, actual, differences);
        }
    }
}