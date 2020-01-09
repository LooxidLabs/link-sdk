using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

namespace Looxid.Link
{
    public class LooxidLinkProcess
    {
        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInitWrapper()
        {
            #if !UNITY_EDITOR
                OnInit();
            #endif
        }

        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        #endif
        static void OnInit()
        {
            string roamingFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string looxidlabsFolderPath = "looxidlabs";
            string looxidlinkFolderPath = "Looxid Link Core";
            string appLocationFileName = "app_location.txt";

            string roamingFile = roamingFolderPath + "\\" + looxidlabsFolderPath + "\\" + looxidlinkFolderPath + "\\" + appLocationFileName;
            if (!File.Exists(roamingFile))
            {
                LXDebug.LogError("File does not exist: " + roamingFile);
            }
            StreamReader reader = new System.IO.StreamReader(roamingFile);

            string appPath = reader.ReadToEnd();
            if (!File.Exists(appPath))
            {
                LXDebug.LogError("File does not exist: " + appPath);
            }

            //LXDebug.Log("LooxidLinkCore: " + appPath);
            Process.Start(appPath);
        }
    }
}