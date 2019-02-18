using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace EquinoxLabs.SVGSharpie.ImageSharp.Tests
{
    public static class Utils
    {
        public static readonly string TestFolder;

        static Utils()
        {
            var rootpath = Path.GetDirectoryName(new Uri(typeof(Utils).GetTypeInfo().Assembly.CodeBase).LocalPath);
            TestFolder = Path.GetFullPath(Path.Combine(rootpath, "..", "..", "..", "..", "SVGTests"));
        }

        public static TheoryData<string, string, string> GetTestImages(string folder)
        {
            var result = new TheoryData<string, string, string>();

            var svgFolder = Path.Combine(folder, "svg");
            var pngFolder = Path.Combine(folder, "png");
            var resultFolder = Path.Combine(folder, "result");

            IEnumerable<string> svgFiles = Directory.EnumerateFiles(svgFolder, "*.svg");
            foreach (string svgFile in svgFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(svgFile);
                var pngFile = Path.Combine(pngFolder, $"{filename}.png");
                var resultFile = Path.Combine(resultFolder, $"{filename}.png");
                result.Add(svgFile, pngFile, resultFile);
            }
            return result;
        }

        //private static string FindParentFolderContaining(string item, string path = ".")
        //{
        //    IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path).Select(Path.GetFileName);
        //    if (items.Any(x => x.Equals(item, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        return Path.GetFullPath(path);
        //    }
        //    string parent = Path.GetDirectoryName(Path.GetFullPath(path));
        //    if (parent != null)
        //    {
        //        return FindParentFolderContaining(item, parent);
        //    }
        //    return null;
        //}
    }
}
