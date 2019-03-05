using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;

namespace EquinoxLabs.SVGSharpie.DynamicPDF
{
    internal static class DynamicPdfPageElementExtensions
    {
        public static void AddRange(this Group group, IEnumerable<PageElement> pageElements)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            if (pageElements == null) throw new ArgumentNullException(nameof(pageElements));
            foreach (var element in pageElements)
            {
                group.Add(element);
            }
        }
    }
}
