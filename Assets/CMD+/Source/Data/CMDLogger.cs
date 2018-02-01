using System.IO;
using System.Text;
using UnityEngine;

namespace DaanRuiter.CMDPlus
{
    public static class CMDLogger
    {
        public static void SaveEntriesToFile(CMDLogEntry[] entries, string path)
        {
            if(Application.isMobilePlatform) { return; }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            CreateFoldersIfNecessary();

            string stringToWrite = "";
            for (int i = 0; i < entries.Length; i++)
            {
                stringToWrite += string.Format("[{0}] ", entries[i].PostTime) + entries[i].SerializableMessage;
                if(i + 1 < entries.Length)
                {
                    stringToWrite += "\r\n";
                }
            }

            using (FileStream stream = File.Create(path))
            {
                byte[] content = new UTF8Encoding(true).GetBytes(stringToWrite);
                stream.Write(content, 0, content.Length);
            }
        }

        public static void CreateFoldersIfNecessary()
        {
            if(Application.isMobilePlatform) { return; }

            string pathToBaseFolder = Application.dataPath + "/../" + CMDStrings.CMD_DATA_FOLDER_PATH;
            string pathToLogsFolder = Application.dataPath + "/../" + CMDStrings.CMD_LOGGER_PATH;
            if (!Directory.Exists(pathToBaseFolder))
            {
                Directory.CreateDirectory(pathToBaseFolder);
            }
            if (!Directory.Exists(pathToLogsFolder))
            {
                Directory.CreateDirectory(pathToLogsFolder);
            }
        }
    }
}