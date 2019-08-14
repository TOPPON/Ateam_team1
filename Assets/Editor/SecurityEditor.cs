using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Reflection;
using System.Security.Cryptography;

namespace Ateam
{
    [CustomEditor(typeof(SettingData))]
    public class SecurityEditor : Editor
    {
        static readonly string MasterFilePath   = "Assets/Resources/Master/";
        static readonly string SecurityFilePath = "Setting/SettingData";
        static readonly int SeedListMax = 30;
        static readonly int SeedMax = 2147483647;

        //---------------------------------------------------
        // OnInspectorGUI
        //---------------------------------------------------
        public override void OnInspectorGUI()
        {
            SettingData settingData = target as SettingData;

            bool enable = EditorGUILayout.ToggleLeft ("MasterMode", settingData.InternMaster);

            if (settingData.InternMaster != enable)
            {
                AssetDatabase.StartAssetEditing();

                settingData.InternMaster = enable;

                AssetDatabase.StopAssetEditing();
                EditorUtility.SetDirty(settingData);
                AssetDatabase.SaveAssets();;
            }

            //マスターのハッシュ値作成
            if (settingData.InternMaster && GUILayout.Button("Generate"))
            {
                AssetDatabase.StartAssetEditing();

                //settingData._hashList.Clear();

                string[] filePathArray = Directory.GetFiles (MasterFilePath, "*", SearchOption.AllDirectories);

                foreach (string filePath in filePathArray)
                {
                    UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);

                    if (asset != null)
                    {
                        Debug.Log(asset.name);

                        CreateData(settingData, asset);
                    }
                }

                AssetDatabase.StopAssetEditing();
                EditorUtility.SetDirty(settingData);
                AssetDatabase.SaveAssets();
            }

            //シード値作成
            if (settingData.InternMaster && GUILayout.Button("Random Seed Generate"))
            {
                SeedData asset = AssetDatabase.LoadAssetAtPath<SeedData>(MasterFilePath + "SeedData.asset");

                if (asset != null)
                {
                    AssetDatabase.StartAssetEditing();

                    asset.GetList().Clear();

                    for (int i = 0; i < SeedListMax; i++)
                    {
                        SeedData.SeedDataList data = new SeedData.SeedDataList();
                        data.Seed = UnityEngine.Random.Range(0, SeedMax);

                        asset.GetList().Add(data);
                    }

                    AssetDatabase.StopAssetEditing();
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssets();

                }
            }

        }

        //---------------------------------------------------
        // CreateData
        //---------------------------------------------------
        static byte[] CreateData(SettingData securityObject, UnityEngine.Object asset)
        {
            securityObject = Resources.Load<SettingData>(SecurityFilePath);
            
            ActionData actionData = asset as ActionData;

            //アクションデータ
            if ((actionData) != null)
            {
                byte[] hash = Master.CreateActionDataHash(actionData);
                securityObject.AddHashData(new SettingData.HashDataList(Master.TYPE.ACTION, hash));

                return hash;
            }

            BulletData bulletData = asset as BulletData;

            //弾データ
            if (bulletData != null)
            {
                byte[] hash = Master.CreateBulletDataHash(bulletData);
                securityObject.AddHashData(new SettingData.HashDataList(Master.TYPE.BULLET, hash));

                return hash;
            }

            CharacterData characterData = asset as CharacterData;

            //キャラクターデータ
            if (characterData != null)
            {
                byte[] hash = Master.CreateCharacterDataHash(characterData);
                securityObject.AddHashData(new SettingData.HashDataList(Master.TYPE.CHARACTER, hash));

                return hash;
            }

            EffectData effectData = asset as EffectData;

            //エフェクトデータ
            if (effectData != null)
            {
                byte[] hash = Master.CreateEffectDataHash(effectData);
                securityObject.AddHashData(new SettingData.HashDataList(Master.TYPE.EFFECT, hash));

                return hash;
            }

            ItemData itemData = asset as ItemData;

            //アイテムデータ
            if (itemData != null)
            {
                byte[] hash = Master.CreateItemDataHash(itemData);
                securityObject.AddHashData(new SettingData.HashDataList(Master.TYPE.ITEM, hash));

                return hash;
            }

            StageData stageData = asset as StageData;

            //ステージデータ
            if (stageData != null)
            {
                byte[] hash = Master.CreateStageDataHash(stageData);
                securityObject.AddHashData(new SettingData.HashDataList(Master.TYPE.STAGE, hash));

                return hash;
            }

            SeedData seedData = asset as SeedData;

            if (seedData != null)
            {
                byte[] hash = Master.CreateSeedDataHash(seedData);
                securityObject.AddHashData(new SettingData.HashDataList(Master.TYPE.SEED, hash));

                return hash;
            }

            return null;
        }
    }
}