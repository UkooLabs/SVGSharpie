using System;
using System.IO;
using Xunit;

namespace EquinoxLabs.SVGSharpie.ImageSharp.Tests
{
    public class Tests
    {
        [Fact]
        public void FileTest()
        {
            string path = Path.GetTempFileName();
            string testData = Guid.NewGuid().ToString();
            File.WriteAllText(path, testData);
            string data = File.ReadAllText(path);
            Assert.Equal(testData, data);            
            File.Delete(path);
        }
    }
}
