using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISApp1.Utils
{
    class RemoveAllEvent
    {
        // 移除事件 - 遗弃
        public static Delegate[] GetObjectEventList(object p_Object, string p_EventName)
        {
            FieldInfo _Field = p_Object.GetType().GetField(p_EventName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            if (_Field == null)
            {
                return null;
            }
            object _FieldValue = _Field.GetValue(p_Object);
            if (_FieldValue != null && _FieldValue is Delegate)
            {
                Delegate _ObjectDelegate = (Delegate)_FieldValue;
                return _ObjectDelegate.GetInvocationList();
            }
            return null;
        }
    }
}
