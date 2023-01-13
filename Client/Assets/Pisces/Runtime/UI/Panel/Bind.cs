/****************
 *@class name:		Bind
 *@description:		绑定界面属性
 *@author:			selik0
 *@date:			2022-06-05 11:12:52
 *@version: 		V1.0.0
*************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Pisces
{
    public class Bind : MonoBehaviour
    {
        public enum BindObjectType
        {
            Object,
            Integer,
            Float,
            Boolean,
            String
        }

        [System.Serializable]
        public class BindObject
        {
            public string key;
            public BindObjectType type;
            public int i;
            public float f;
            public bool b;
            public string str;
            public UnityEngine.Object obj;
        }
        public BindObject[] objects;

        public void LuaBind(XLua.LuaTable lua)
        {
            if (lua == null) return;
            foreach (BindObject obj in objects)
            {
                if (string.IsNullOrEmpty(obj.key))
                    continue;
                switch (obj.type)
                {
                    case BindObjectType.Object:
                        lua.Set(obj.key, obj.obj);
                        break;
                    case BindObjectType.Integer:
                        lua.Set(obj.key, obj.i);
                        break;
                    case BindObjectType.Float:
                        lua.Set(obj.key, obj.f);
                        break;
                    case BindObjectType.Boolean:
                        lua.Set(obj.key, obj.b);
                        break;
                    case BindObjectType.String:
                        lua.Set(obj.key, obj.str);
                        break;
                }
            }
        }
    }
}