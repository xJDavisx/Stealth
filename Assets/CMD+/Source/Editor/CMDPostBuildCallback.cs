using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DaanRuiter.CMDPlus.Editor {
    public class CMDPostBuildCallback : MonoBehaviour {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
            if (target == BuildTarget.StandaloneWindows ||
                target == BuildTarget.StandaloneWindows64 ||
                target == BuildTarget.StandaloneOSXIntel ||
                target == BuildTarget.StandaloneOSXIntel64 ||
                target == BuildTarget.StandaloneOSX ||
                target == BuildTarget.StandaloneLinux ||
                target == BuildTarget.StandaloneLinux64 ||
                target == BuildTarget.StandaloneLinuxUniversal) {
#if !CMDPLUS_EXCLUDED_IN_BUILD
                string targetfolder = Path.GetDirectoryName(pathToBuiltProject) + "/CMD";
                DirectoryCopy(Application.dataPath + "/../" + CMDStrings.CMD_DATA_FOLDER_PATH, targetfolder, true);
#endif
                if (!CMDSettings.Has("CMDPLUS_ENABLED")) {
                    CMDHelper.SetBuildSymbol(true);
                }
            }
        }

        //From: https://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}