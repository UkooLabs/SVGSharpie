using System;
using System.IO;
using Xunit;

namespace EQL.SVGSharpie.ImageSharp.Tests
{
    public class UnitTests
    {
        [Fact]
        public void Test1()
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
