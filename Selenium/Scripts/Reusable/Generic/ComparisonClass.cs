using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.Scripts.Reusable.Generic
{
    public class ComparisonClass : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return String.CompareOrdinal(a, b);
        }
    }
}
