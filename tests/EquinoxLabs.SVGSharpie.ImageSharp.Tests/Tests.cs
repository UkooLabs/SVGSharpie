using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
