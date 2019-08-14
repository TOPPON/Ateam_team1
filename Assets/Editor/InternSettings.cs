using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Reflection;

namespace Ateam
{
    [CustomEditor(typeof(InternSetting))]
    public class InternSettings : Editor
    {
        static readonly string WritePath        = "Assets/Intern/";
        static readonly string ReadFilePath     = "Assets/Editor/AISystemTemplate.cs";
        static readonly string SettingFilePath  = "Assets/Intern/Setting.asset";
        static readonly string BattleScenePath  = "Assets/Ateam/Scenes/BattleScene.unity";
        static readonly string TitleScenePath   = "Assets/Ateam/Scenes/TitleScene.unity";

        static readonly string ProgressSettingTitle = "インターン設定中";

        enum PROGRESS_SETTING_TYPE
        {
            NONE = 0,
            GENERATE,
            ATTACH,
            SET,
            DONE,
            MAX,
        }

        private PROGRESS_SETTING_TYPE _progressSettingType = PROGRESS_SETTING_TYPE.NONE;

        //---------------------------------------------------
        // OnInspectorGUI
        //---------------------------------------------------
        public override void OnInspectorGUI()
        {
            InternSetting data = target as InternSetting;

            EditorGUILayout.LabelField( "チーム名　※日本語可　ランキング用" );
            EditorGUILayout.LabelField( "チーム全員で同じにしてください" );
            data.TeamName = EditorGUILayout.TextField(data.TeamName);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField( "割り振れられたチームy記号を選択してください" );
            EditorGUILayout.LabelField( "チーム全員で同じにしてください" );
            data.AiClassName = (InternSetting.TEAM)EditorGUILayout.EnumPopup("チーム記号", data.AiClassName);

            EditorGUILayout.Space();

            Generate(data);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            ExportPackage(data);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            MasterMode(data);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        //---------------------------------------------------
        // SetTeam
        //---------------------------------------------------
        private void SetTeam(Define.Battle.TEAM_TYPE type, InternSetting data)
        {
            UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BattleScenePath);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(WritePath + data.AiClassName + ".prefab");
            GameObject.Find("BattleMain").GetComponent<BattleSystem>().AISystemList[(int)type] = go;
            GameObject.Find("BattleMain").GetComponent<BattleSystem>().TeamNameList[(int)type] = data.TeamName;
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(TitleScenePath);
        }

        //---------------------------------------------------
        // Generate
        //---------------------------------------------------
        private void Generate(InternSetting data)
        {
            try
            {
                if( GUILayout.Button( "1.Generate" ) )
                {
                    _progressSettingType = PROGRESS_SETTING_TYPE.GENERATE;

                    EditorUtility.DisplayProgressBar(ProgressSettingTitle, "作成", ((float)_progressSettingType / (float)PROGRESS_SETTING_TYPE.MAX));

                    //CSを生成
                    string readDataText = File.ReadAllText(ReadFilePath);
                    readDataText = readDataText.Replace("AISystemTemplate", Enum.GetName(typeof(InternSetting.TEAM), (int)data.AiClassName));

                    string csharpPath = WritePath + data.AiClassName + ".cs";
                    File.WriteAllText(csharpPath, readDataText);
                    AssetDatabase.ImportAsset(csharpPath);
                    AssetDatabase.Refresh();

                    //空のプレファブを生成
                    GameObject go = EditorUtility.CreateGameObjectWithHideFlags(Enum.GetName(typeof(InternSetting.TEAM), (int)data.AiClassName), HideFlags.HideInHierarchy);

                    PrefabUtility.CreatePrefab(WritePath + data.AiClassName + ".prefab", go);
                    Editor.DestroyImmediate(go);

                    AssetDatabase.RenameAsset(SettingFilePath, data.AiClassName + "Setting");
                    AssetDatabase.Refresh();

                    _progressSettingType = PROGRESS_SETTING_TYPE.ATTACH;
                    EditorUtility.DisplayProgressBar(ProgressSettingTitle, "アタッチ", ((float)_progressSettingType / (float)PROGRESS_SETTING_TYPE.MAX));
                }

                if (_progressSettingType == PROGRESS_SETTING_TYPE.ATTACH && !EditorApplication.isUpdating)
                {
                    Type classType = null;

                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (type.Name == Enum.GetName(typeof(InternSetting.TEAM), (int)data.AiClassName))
                            {
                                classType = type;
                            }
                        }
                    }

                    if(classType == null)
                    {
                        return;
                    }

                    //プレファブにアタッチ
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(WritePath + data.AiClassName + ".prefab");

                    if (go != null)
                    {
                        if (go.GetComponent(classType) == null)
                        {
                            go.AddComponent(classType);
                        }
                    }

                    _progressSettingType = PROGRESS_SETTING_TYPE.SET;
                    EditorUtility.DisplayProgressBar(ProgressSettingTitle, "セット", ((float)_progressSettingType / (float)PROGRESS_SETTING_TYPE.MAX));
                }

                if ( _progressSettingType == PROGRESS_SETTING_TYPE.SET)
                {
                    SetTeam(Ateam.Define.Battle.TEAM_TYPE.ALPHA, data);
                    _progressSettingType = PROGRESS_SETTING_TYPE.DONE;
                    EditorUtility.DisplayProgressBar(ProgressSettingTitle, "終了確認", ((float)_progressSettingType / (float)PROGRESS_SETTING_TYPE.MAX));
                    EditorUtility.ClearProgressBar();
                }

                if(_progressSettingType == PROGRESS_SETTING_TYPE.DONE)
                {
                    _progressSettingType = PROGRESS_SETTING_TYPE.NONE;

                    EditorUtility.DisplayDialog("完了", "正常に処理が完了しました", "OK");
                }
            }
            catch(Exception exception)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog(" 作成に失敗しました", "Generateボタンを何回かリトライしてダメなら講師へ報告してください\n" + "進捗状況 : " + _progressSettingType.ToString(), "OK");
                Debug.LogError(exception.ToString());
                _progressSettingType = PROGRESS_SETTING_TYPE.NONE;
            }
        }

        //---------------------------------------------------
        // ExportPackage
        //---------------------------------------------------
        private void ExportPackage(InternSetting data)
        {
            try
            {
                if( GUILayout.Button( "2.ExportPackage" ) )
                {
                    DirectoryInfo dir = new DirectoryInfo(WritePath);
                    FileInfo[] info = dir.GetFiles("*");
                    List<string> list = new List<string>();

                    foreach(FileInfo fi in info)
                    {
                        if(fi.Name.Contains(".meta"))
                        {
                            continue;
                        }

                        list.Add(WritePath + fi.Name);
                        Debug.Log(WritePath + fi.Name);
                    }

                    AssetDatabase.ExportPackage(list.ToArray(), data.TeamName + ".unitypackage");
                    EditorUtility.DisplayDialog("完了", "正常に処理が完了しました", "OK");
                }
            }
            catch(Exception exception)
            {
                EditorUtility.DisplayDialog("作成に失敗しました", "ExportPackageボタンを何回かリトライしてダメなら講師へ報告してください", "OK");
                Debug.LogError(exception.ToString());
            }
        }

        //---------------------------------------------------
        // MasterMode
        //---------------------------------------------------
        private void MasterMode(InternSetting data)
        {
            if (Resources.Load<SettingData>("Setting/SettingData").InternMaster)
            {
                if (GUILayout.Button("ALPHA"))
                {
                    SetTeam(Ateam.Define.Battle.TEAM_TYPE.ALPHA, data);
                }

                if (GUILayout.Button("BRAVO"))
                {
                    SetTeam(Ateam.Define.Battle.TEAM_TYPE.BRAVO, data);
                }

                if (GUILayout.Button("RESET"))
                {
                    UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BattleScenePath);
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/AI/CustomAI.prefab");
                    GameObject.Find("BattleMain").GetComponent<BattleSystem>().AISystemList[(int)Define.Battle.TEAM_TYPE.ALPHA] = go;
                    GameObject.Find("BattleMain").GetComponent<BattleSystem>().AISystemList[(int)Define.Battle.TEAM_TYPE.BRAVO] = go;
                    GameObject.Find("BattleMain").GetComponent<BattleSystem>().TeamNameList[(int)Define.Battle.TEAM_TYPE.ALPHA] = "Enemy_Alpha";
                    GameObject.Find("BattleMain").GetComponent<BattleSystem>().TeamNameList[(int)Define.Battle.TEAM_TYPE.BRAVO] = "Enemy_Bravo";
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(TitleScenePath);
                }
            }
        }
    }
}
