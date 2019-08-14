using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.IO;

namespace Ateam
{
    public class InternSetting : ScriptableObject
	{
        static readonly string DirectoryPath    = "Assets/Intern/";
        static readonly string SettingFileName  = "Setting.asset";

        public enum TEAM
        {
            Alfa = 0,
            Bravo,
            Charlie,
            Delta,
            Echo,
            Foxtrot,
            Golf,
            Hotel,
            India,
            Juliett,
            Kilo,
            Lima,
            Mike,
            November,
            Oscar,
            Papa,
            Quebec,
            Romeo,
            Sierra,
            Tango,
            Uniform,
            Victor,
            Whiskey,
            Xray,
            Yankee,
            Zulu,
        }

        [MenuItem("Intern/Create/Setting")]
        public static void CreatePrefab()
        {
            if (Directory.Exists(DirectoryPath) == false)
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            InternSetting exampleAsset = CreateInstance<InternSetting> ();
            AssetDatabase.CreateAsset (exampleAsset,  DirectoryPath + SettingFileName);
            AssetDatabase.Refresh ();

            Selection.activeInstanceID = exampleAsset.GetInstanceID();
        }

        [SerializeField]
        public string TeamName;         //チーム名

        [SerializeField]
        public TEAM AiClassName;      //ユニークなやつ
	}
}