using UnityEngine;
using System.Collections;

namespace HCFeng.ABManage.ABDownLoad
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public class DebugManager
    {
        public static void Log(object message) 
        {
            Debug.Log(message);
        }
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

    }
}