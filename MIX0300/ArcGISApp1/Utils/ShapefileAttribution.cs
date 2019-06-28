using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISApp1.Utils
{
    public class ShapefileAttribution
    {
        public string attrKey;
        public string attrValue;

        public ShapefileAttribution(string attrKey, string attrValue)
        {
            this.attrKey = attrKey;
            this.attrValue = attrValue;
        }

        public string AttrKey
        {
            get
            {
                return attrKey;
            }
            set
            {
                attrKey = value;
            }
        }

        public string AttrValue
        {
            get
            {
                return attrValue;
            }
            set
            {
                attrValue = value;
            }
        }
    }
}
