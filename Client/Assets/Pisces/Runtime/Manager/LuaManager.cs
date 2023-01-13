/****************
 *@class name:		LuaManager
 *@description:		
 *@author:			selik0
 *@date:			2023-01-09 11:43:13
 *@version: 		V1.0.0
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
namespace Pisces
{
    public class LuaManager : MonoSingleton<LuaManager>
    {
        LuaEnv luaEnv = null;
        protected override void Startup()
        {
            luaEnv = new LuaEnv();
            luaEnv.AddLoader(AssetLoad.LuaCustomLoader);
        }

        protected override void Dispose()
        {
            luaEnv = null;
        }

        public override void ReStart()
        {
            Dispose();
            Startup();
        }

        private void Update()
        {
            if (luaEnv != null)
            {
                luaEnv.Tick();
            }
        }

        public void SafeDoString(string script)
        {
            if (luaEnv != null)
            {
                try
                {
                    luaEnv.DoString(script);
                }
                catch (System.Exception ex)
                {
                    string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                    MyLogger.LogError(msg);
                }
            }
        }
    }
}