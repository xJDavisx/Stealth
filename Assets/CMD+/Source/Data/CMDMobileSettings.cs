using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class CMDMobileSettings : ScriptableObject
{
    public static CMDMobileSettings Loaded
    {
        get
        {
            if (m_loaded == null)
            {
                m_loaded = Resources.Load<CMDMobileSettings>("Mobile/CMDMobileSettings");
            }
            return m_loaded;
        }
    }
    private static CMDMobileSettings m_loaded;
#if UNITY_EDITOR
    [MenuItem("Assets/Create/CMD+ MobileSettings")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<CMDMobileSettings>();
    }
#endif
    public int TextSize;
    public float UIScale;
}