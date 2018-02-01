using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public static class CMDSettings
    {
        private const string INIT_TAG = "[INIT]";
        
        private static List<CMDSetting<object>> settings = new List<CMDSetting<object>>();
        private static string[] loadedFiles;

        /// <summary>
        /// Loads all CMDSettings in all XML files in the given folder
        /// </summary>
        /// <param name="pathToFolder">The (full) path to the target folder</param>
        public static void LoadSettings(string pathToFolder = CMDStrings.CMD_SETTINGS_PATH_DEFAULT)
        {
            if(Application.isMobilePlatform)
            {
                return;
            }

            if(pathToFolder == CMDStrings.CMD_SETTINGS_PATH_DEFAULT)
            {
                pathToFolder = Application.dataPath + "/../" + CMDStrings.CMD_SETTINGS_PATH;
            }
            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
                CMD.Warning("No settings were found, generating empty settings folder..");
            }

            string[] settingFiles = Directory.GetFiles(pathToFolder, "*.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<CMDSetting<object>>));
            settings = new List<CMDSetting<object>>();
            loadedFiles = new string[settingFiles.Length];
            for (int i = 0; i < loadedFiles.Length; i++) {
                loadedFiles[i] = Path.GetFileName(settingFiles[i]);
            }

            for (int i = 0; i < settingFiles.Length; i++)
            {
                using (FileStream stream = new FileStream(settingFiles[i], FileMode.Open))
                {
                    try
                    {
                        List<CMDSetting<object>> loaded = (serializer.Deserialize(stream) as List<CMDSetting<object>>);
                        for (int l = loaded.Count -1; l >= 0; l--)
                        {
                            if(loaded[l].Identifier.Length >= INIT_TAG.Length) {
                                if(loaded[l].Identifier.Substring(0, INIT_TAG.Length) == INIT_TAG) {
                                    loaded.RemoveAt(l);
                                    continue;
                                }
                            }
                            loaded[l].SettingsFile = Path.GetFileName(settingFiles[i]);
                            settings.Add(loaded[l]);
                        }
                    } catch (XmlException exception)
                    {
                        CMD.Exception(exception);
                    }
                    stream.Close();
                }
            }
            settings = settings.OrderBy(x => x.Identifier).ToList();
        }

        /// <summary>
        /// Saves all settings
        /// </summary>
        /// <param name="showMessage">Will display a message in the console once finished</param>
        public static void SaveSettings(bool showMessage = true)
        {
            SaveSettings(Application.dataPath + "/../" + CMDStrings.CMD_SETTINGS_PATH, showMessage);
        }

        /// <summary>
        /// Saves the settings in the given folder
        /// </summary>
        /// <param name="pathToFolder">The (full) path to the target folder</param>
        /// <param name="showMessage">Will display a message in the console once finished</param>
        public static void SaveSettings(string pathToFolder, bool showMessage)
        {
            if(Application.isMobilePlatform)
            {
                CMD.Warning("CMDSettings is not supported on mobile devices");
                return;
            }

            float startTimeStamp = Time.realtimeSinceStartup;
            string pathToBaseFolder = Application.dataPath + "/../" + CMDStrings.CMD_DATA_FOLDER_PATH;
            string pathToSettingsFolder = Application.dataPath + "/../" + CMDStrings.CMD_SETTINGS_PATH;
            if (!Directory.Exists(pathToBaseFolder))
            {
                Directory.CreateDirectory(pathToBaseFolder);
            }
            if (!Directory.Exists(pathToSettingsFolder))
            {
                Directory.CreateDirectory(pathToSettingsFolder);
            }

            Dictionary<string, List<CMDSetting<object>>> settingFiles = GetAllPerFile();
            XmlSerializer serializer = new XmlSerializer(typeof(List<CMDSetting<object>>));
            foreach (KeyValuePair<string, List<CMDSetting<object>>> settingDictionary in settingFiles)
            {
                using (FileStream stream = new FileStream(pathToFolder + settingDictionary.Key, FileMode.Create))
                {
                    try
                    {
                        serializer.Serialize(stream, settingDictionary.Value);
                    } catch (XmlException exception)
                    {
                        CMD.Exception(exception);
                    }
                    stream.Close();
                }
            }

            if (!showMessage) { return; }
            string msg = string.Format("Saved {0} Settings in " + (Time.realtimeSinceStartup - startTimeStamp).ToString("0.000") + " seconds", settings.Count);
            if (CMD.Initialized) {
                CMD.Log(msg);
            }
            else {
                Debug.Log(msg);
            }
        }

        /// <summary>
        /// Set the value of a setting / create a setting if none with given identifier exist
        /// </summary>
        /// <param name="identifier">Identifier of the setting</param>
        /// <param name="data">The value of the setting</param>
        /// <param name="settingFile">The file the setting is located in</param>
        public static void Set(string identifier, object data, string settingFile)
        {
            if (Application.isMobilePlatform)
            {
                CMD.Warning("CMDSettings is not supported on mobile devices");
                return;
            }
            if (settings == null)
            {
                LoadSettings();
            }
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Identifier == identifier && settings[i].Data.GetType() == data.GetType())
                {
                    settings[i].Data = data;
                    settings[i].SettingsFile = settingFile;
                    
                    if (Has("CMDPLUS_ENABLED") && identifier == "CMDPLUS_ENABLED" && data.GetType() == typeof(bool)) {
                        CMDHelper.SetBuildSymbol((bool)data);
                    }
                    return;
                }
            }
            settings.Add(new CMDSetting<object>(identifier, data, settingFile));
        }

        /// <summary>
        /// Get the value of the setting with the given identifier
        /// </summary>
        /// <typeparam name="DataType">Type of the setting (result will be casted to this type)</typeparam>
        /// <param name="identifier">Identifier of the setting</param>
        /// <param name="postError">Should an error message be posted if the setting doesn't exist</param>
        /// <returns>The value of the setting casted to the given type</returns>
        public static DataType Get<DataType>(string identifier, bool postError = false)
        {
            if (settings == null)
            {
                LoadSettings();
            }
            for (int i = 0; i < settings.Count; i++)
            {
                if(settings[i].Identifier == identifier && settings[i].Data.GetType() == typeof(DataType))
                {
                    return (DataType)settings[i].Data;
                }
            }
            if (postError)
            {
                CMD.Error(CMDStrings.UnknownSettingException(identifier));
            }
            return default(DataType);
        }

        /// <summary>
        /// Remove a setting for the next time the settings are saved
        /// </summary>
        /// <param name="identifier">Identifier of the setting</param>
        /// <returns>If the setting existed in the first place</returns>
        public static bool Remove(string identifier)
        {
            if (settings == null)
            {
                LoadSettings();
            }
            for (int i = settings.Count - 1; i >= 0; i--)
            {
                if(settings[i].Identifier == identifier) 
                {
                    settings.RemoveAt(i);                    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Was the setting with the given identifier loaded
        /// </summary>
        /// <param name="identifier">Identifier of the setting</param>
        /// <param name="requiredType">The required type the setting should have</param>
        /// <returns>If the setting is loaded and has the correct type if one was given</returns>
        public static bool Has(string identifier, Type requiredType = null)
        {
            if (settings == null)
            {
                LoadSettings();
            }
            for (int i = 0; i < settings.Count; i++)
            {
                if(settings[i].Identifier == identifier && (requiredType == null || settings[i].Data.GetType() == requiredType))
                {
                    return true;
                }
            }
            return false;
        }
        
        public static void DeleteFile(string fileName) {
            string fullpath = Application.dataPath + "/../" + CMDStrings.CMD_SETTINGS_PATH + fileName;
            if (!File.Exists(fullpath)) {
                Debug.Log(CMDStrings.UnkownFileOrFolder(fullpath));
                return;
            }            
            File.Delete(fullpath);
        }

        public static void CreateFile(string fileName) {
            string fullpath = Application.dataPath + "/../" + CMDStrings.CMD_SETTINGS_PATH + fileName;
            string templatePath = Application.dataPath + "/" + CMDStrings.CMD_SETTINGS_PATH_TEMPLATE_FILE;

            if (Directory.Exists(fullpath)) {
                if (CMD.Initialized) {
                    CMD.Error(string.Format("File already exists: " + fileName));
                } else {
                    Debug.LogError(string.Format("File already exists: " + fileName));
                }
                return;
            }

            if (Directory.Exists(templatePath)) {
                if (CMD.Initialized) {
                    CMD.Error(string.Format("No template could be found at " + templatePath));
                }else {
                    Debug.LogError(string.Format("No template could be found at " + templatePath));
                }
                return;
            }

            File.Copy(templatePath, fullpath);
        }

        /// <summary>
        /// Get all the settings split per file
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<CMDSetting<object>>> GetAllPerFile() {
            Dictionary<string, List<CMDSetting<object>>> settingFiles = new Dictionary<string, List<CMDSetting<object>>>();
            for (int i = 0; i < settings.Count; i++) {
                if (!settingFiles.ContainsKey(settings[i].SettingsFile)) {
                    settingFiles.Add(settings[i].SettingsFile, new List<CMDSetting<object>>());
                }
                settingFiles[settings[i].SettingsFile].Add(settings[i]);
            }
            for (int i = 0; i < loadedFiles.Length; i++) {
                if (!settingFiles.ContainsKey(loadedFiles[i])) {
                    if (!settingFiles.ContainsKey(loadedFiles[i])) {
                        settingFiles.Add(loadedFiles[i], new List<CMDSetting<object>>());
                    }
                }
            }
            return settingFiles;
        }

        /// <summary>
        /// Get all loaded settings
        /// </summary>
        public static CMDSetting<object>[] GetAll()
        {
            return settings.ToArray();
        }

#if !CMDPLUS_EXAMPLES_ENABLED
        [CMDCommandButton("List Settings")]
        [CMDCommand("Shows all loaded settings")]
        public static void ListSettings()
        {
            for (int i = 0; i < settings.Count; i++)
            {
                Color castResult;
                if(CMDHelper.TryParseColor(settings[i].Data.ToString(), out castResult))
                {
                    CMD.Log(string.Format("<b>{0}</b> = <color=#{3}>{1}</color> in <i>{2}</i>", settings[i].Identifier, settings[i].Data, settings[i].SettingsFile, CMDHelper.ColorToHex(castResult)));
                } else
                {
                    CMD.Log(string.Format("<b>{0}</b> = <color=#6abfed>{1}</color> in <i>{2}</i>", settings[i].Identifier, settings[i].Data, settings[i].SettingsFile));
                }
            }
        }
#endif

        internal static void SetAt(int index, object data) {
            if(index >= 0 && index < settings.Count - 1) {
                if(settings[index].Data.GetType() == data.GetType()) {
                    settings[index] = new CMDSetting<object>(settings[index].Identifier, data, settings[index].SettingsFile);
                }
            }
        }

        private static void OnApplicationQuit()
        {

        }
    }
    

    [XmlRoot("Setting")]
    public class CMDSetting<SettingType>
    {
        [XmlElement("Data")] public SettingType Data;
        [XmlElement("Identifier")] public string Identifier;
        [XmlIgnore] public string SettingsFile;

        public CMDSetting(string identifier, SettingType data, string originFile)
        {
            Identifier = identifier;
            Data = data;
            SettingsFile = originFile;
            if (!Data.GetType().IsSerializable && Data.GetType() != typeof(Color))
            {
                CMD.Error(CMDStrings.NonSerializableTypeException(data.GetType()));
            }
        }

        public CMDSetting() {
            Identifier = string.Empty;
            SettingsFile = string.Empty;
        }
    }
}