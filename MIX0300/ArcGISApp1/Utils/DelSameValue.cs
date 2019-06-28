using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISApp1.Utils
{
    class DelSameValue : IEqualityComparer<Object>
    {
        public new bool Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
