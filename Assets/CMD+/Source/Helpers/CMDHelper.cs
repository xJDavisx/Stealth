using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DaanRuiter.CMDPlus {
    public static class CMDHelper {
        public const float CMD_ALT_BACKGROUND_COLOR_MODIFIER = 1.55f;

        public static GUISkin UnityDefaultSkinRef { set {
                unityDefaultSkinRef = value;
                unityDefaultSkinRef.label.fontSize = 12;
                unityDefaultSkinRef.textField.fontSize = 12;

                unityDefaultSkinRef.button.normal.textColor = Color.black;
                unityDefaultSkinRef.button.normal.background = new Texture2D(1, 1);
                unityDefaultSkinRef.button.normal.background.SetPixel(0, 0, Color.white * 0.15f);
                unityDefaultSkinRef.button.normal.background.Apply();

                unityDefaultSkinRef.button.hover.background = new Texture2D(1, 1);
                unityDefaultSkinRef.button.hover.background.SetPixel(0, 0, Color.white * 1.25f);
                unityDefaultSkinRef.button.hover.background.Apply();
            }
            get {
                return unityDefaultSkinRef;
            }
        }
        private static GUISkin unityDefaultSkinRef;

        public static Texture2D DefaultTexture { get {
                if(defaultTexture == null) {
                    defaultTexture = new Texture2D(1, 1);
                    defaultTexture.SetPixel(0, 0, CMDWindow.DEFAULT_WINDOW_BACKGROUND_COLOR);
                    defaultTexture.Apply();
                }
                return defaultTexture;
            } }
        private static Texture2D defaultTexture;

        public static AttributeType[] FindMethodAttributes<AttributeType>(Assembly[] assemblies, bool staticOnly = false) where AttributeType : Attribute, ICMDAttribute {
            List<AttributeType> result = new List<AttributeType>();

            for (int current = 0; current < assemblies.Length; current++) {
                MethodInfo[] methods = assemblies[current].GetTypes()
                  .SelectMany(t => t.GetMethods())
                  .Where(m => m.GetCustomAttributes(typeof(AttributeType), true).Length > 0)
                  .ToArray();

                for (int i = 0; i < methods.Length; i++) {
                    if (staticOnly && !methods[i].IsStatic) {
                        continue;
                    }

                    AttributeType attribute = ExtractAttribute<AttributeType>(methods[i]);
                    attribute.MethodInfo = methods[i];
                    result.Add(attribute);
                }
            }

            return result.ToArray();
        }

        public static void OpenScriptInIDE(string filePath, int lineNumber)
        {
            UnityEngine.Object asset = null;

#if UNITY_EDITOR
            string relativePath = filePath.Substring(filePath.IndexOf("Assets\\"));
            asset = AssetDatabase.LoadAssetAtPath(relativePath, typeof(UnityEngine.Object)) as UnityEngine.Object;

            if (asset != null)
            {
                AssetDatabase.OpenAsset(asset);
            }
#endif
            if (asset == null)
            {
                if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) 
                {
                    Process.Start("devenv", string.Format("{0} /command \"Edit.Goto {1}\"", filePath, lineNumber));
                }
            }
        }
        
        public static bool CopyDefaultSettingsIfNeeded() {
            string pathToCMDRoot = Application.dataPath + "/../" + CMDStrings.CMD_DATA_FOLDER_PATH;
            string settingsFolderPath = pathToCMDRoot + "Settings/";

            if (Directory.Exists(settingsFolderPath) && Directory.GetFiles(settingsFolderPath).Length > 0) {
                return false;
            }

            string[] requiredFolders = { settingsFolderPath, pathToCMDRoot + "Logs" };

            for (int i = 0; i < requiredFolders.Length; i++) {
                if (!Directory.Exists(requiredFolders[i])) {
                    Directory.CreateDirectory(requiredFolders[i]);
                }
            }

            string[] defaultSettingFiles = Directory.GetFiles(Application.dataPath + "/" + CMDStrings.CMD_SETTINGS_PATH_DEFAULT_SETTING_FILES, "*.xml");
            for (int i = 0; i < defaultSettingFiles.Length; i++) {
                string[] split = defaultSettingFiles[i].Split('\\');
                File.Copy(defaultSettingFiles[i], settingsFolderPath + split[split.Length - 1]);
            }

            UnityEngine.Debug.Log("Copied default CMD settings to project folder. (This happens the first time only)");
            return true;
        }

        public static void SetBuildSymbol(bool newState) {
#if UNITY_EDITOR
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            string originalSymbols = currentSymbols;
            if (!newState) {
                if (!currentSymbols.Contains("CMDPLUS_EXCLUDED_IN_BUILD")) {
                    currentSymbols += ";CMDPLUS_EXCLUDED_IN_BUILD";
                }
            } else {
                if (currentSymbols.Contains("CMDPLUS_EXCLUDED_IN_BUILD")) {
                    currentSymbols = new string(currentSymbols.Where(c => !"CMDPLUS_EXCLUDED_IN_BUILD".Contains(c)).ToArray());
                }
            }
            if(currentSymbols != originalSymbols) {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentSymbols);
                string newStateDesc = "DISABLING";
                if (newState) {
                    newStateDesc = "ENABLING";
                }
                UnityEngine.Debug.Log(newStateDesc + " CMD+, Please wait while Unity recompiles");
            }
#endif
        }

        public static CMDUnityInjector GenerateUnityInjector () {
            GameObject CMDViewGameObject = GameObject.Find(CMDStrings.CMD_UNITY_INJECTOR_GAMEOBJECT_NAME);
            if(CMDViewGameObject == null) {
                CMDViewGameObject = new GameObject(CMDStrings.CMD_UNITY_INJECTOR_GAMEOBJECT_NAME);
                CMDViewGameObject.AddComponent<CMDUnityInjector>();
            }
            return CMDViewGameObject.GetComponent<CMDUnityInjector>();
        }

        public static CMDWindow SetupWindow(CMDWindow window) {
            window.Toggle(true);
            window.ToggleVisibility(true);

            CMD.Controller.UnityInjector.AddRenderable(window);
            CMD.Controller.UnityInjector.AddRenderable(window.ContextMenu);

            return window;
        }

        public static bool CastParameter(string parameter, out object result) {

            //Bool
            bool parsedBool;
            bool parsed = bool.TryParse(parameter, out parsedBool);
            if (parsed) {
                result = parsedBool;
                return true;
            }

            //Int
            int parsedInt;
            parsed = int.TryParse(parameter, out parsedInt);
            if (parsed) {
                result = parsedInt;
                return true;
            }

            //Float
            float parsedFloat;
            parsed = float.TryParse(parameter, out parsedFloat);
            if (parsed) {
                result = parsedFloat;
                return true;
            }

            //Color
            Color parsedColor;
            parsed = TryParseColor(parameter, out parsedColor);
            if (parsed) {
                result = parsedColor;
                return true;
            }

            //String
            result = parameter;
            return false;
        }

        public static object[] CastParameters(string[] parameters) {
            if(parameters.Length == 0) {
                return null;
            }

            object[] result = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++) {
                CastParameter(parameters[i], out result[i]);
            }
            return result;
        }

        public static bool CompareParameters(MethodInfo method, object[] parameters) {
            if (parameters == null && method.GetParameters().Length > 0) { return false; }
            if (parameters == null && method.GetParameters().Length == 0) { return true; }
            if (parameters.Length != method.GetParameters().Length) { return false; }

            for (int i = 0; i < method.GetParameters().Length; i++) {
                if(!parameters[i].GetType().IsSubclassOf(method.GetParameters()[i].ParameterType) && parameters[i].GetType() != method.GetParameters()[i].ParameterType) {
                    return false;
                }
            }
            return true;
        }

        public static CMDParameterMismatchException GenerateParamMismatchException(MethodInfo method, object[] parameters) {
            KeyValuePair<Type, Type>[] parameterTypes;
            if (parameters == null || method.GetParameters().Length > parameters.Length) {
                parameterTypes = new KeyValuePair<Type, Type>[method.GetParameters().Length];
            }else {
                parameterTypes = new KeyValuePair<Type, Type>[parameters.Length];
            }

            for (int i = 0; i < parameterTypes.Length; i++) {
                Type methodType = null;
                Type parameterType = null;
                
                if(i < method.GetParameters().Length) {
                    methodType = method.GetParameters()[i].ParameterType;
                }
                if(parameters != null) {
                    if(i < parameters.Length) {
                        parameterType = parameters[i].GetType();
                    }
                }

                parameterTypes[i] = new KeyValuePair<Type, Type>(methodType, parameterType);
            }
            return new CMDParameterMismatchException(method, parameterTypes);
        }

        public static T ExtractAttribute<T>(MethodInfo method) where T : Attribute{
            object[] attributes = method.GetCustomAttributes(typeof(T), true);
            for (int a = 0; a < attributes.Length; a++) {
                if (attributes[a].GetType() == typeof(T)) {
                    return (T)attributes[a];
                }                
            }
            return default(T);
        }
        
        public static bool TryParseColor(string s, out Color color) {
            if(s == null) {
                color = Color.white;
                return false;
            }
            string[] split = s.Split('/');
            if (split.Length == 4) {
                float r = float.Parse(split[0]);
                float g = float.Parse(split[1]);
                float b = float.Parse(split[2]);
                float a = float.Parse(split[3]);
                color = new Color(r, g, b, a);
                return true;
            }
            color = default(Color);
            return false;
        }

        public static Color ParseColor(string s) {
            Color result = Color.clear;
            string[] split = s.Split('/');
            if (split.Length == 4) {
                float r = float.Parse(split[0]);
                float g = float.Parse(split[1]);
                float b = float.Parse(split[2]);
                float a = float.Parse(split[3]);
                result = new Color(r, g, b, a);
            }
            return result;
        }

        public static string ColorToString(Color color) {
            return string.Format("{0}/{1}/{2}/{3}", color.r, color.g, color.b, color.a);
        }

        public static string ColorToHex(Color32 color) {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public static Color HexToColor(string hex) {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static Texture2D GenerateVerticalGradientTexture(Color[] colors, float[] alphas, int width, int height) {
            Texture2D tex = new Texture2D(width, height);
            Gradient gradient = new Gradient();

            //Alpha
            GradientAlphaKey[] aKeys = new GradientAlphaKey[alphas.Length];
            for (int i = 0; i < aKeys.Length; i++) {
                aKeys[i] = new GradientAlphaKey(alphas[i], 1f / aKeys.Length * i);
            }

            //Colors
            GradientColorKey[] cKeys = new GradientColorKey[colors.Length];
            for (int i = 0; i < cKeys.Length; i++) {
                cKeys[i] = new GradientColorKey(colors[i], 1f / cKeys.Length * i);
            }

            gradient.SetKeys(cKeys, aKeys);

            for (int x = 0; x < tex.width; x++) {
                for (int y = 0; y < tex.height; y++) {
                    tex.SetPixel(x, y, gradient.Evaluate(y / (tex.height / 100f) / 100f));
                }
            }
            tex.Apply();

            return tex;
        }
    }
}