#if UNITY_5 || UNITY_2017

using UnityEngine;

namespace DaanRuiter.CMDPlus
{
    public static class CMDSelfInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnLoad()
        {
            CMD.Init();
        }
    }
}

#endif