using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace Ateam
{
    public class Master : BaseCoroutineMonobehaviour
    {
        static readonly string SettingDataFilePath = "Setting/SettingData";

        public enum TYPE
        {
            CHARACTER = 0,
            STAGE,
            BULLET,
            ACTION,
            ITEM,
            EFFECT,
            SEED,
            MAX,
        }

        public CharacterData CharacterData
        {
            get;
            set;
        }

        public StageData StageData
        {
            get;
            set;
        }

        public BulletData BulletData
        {
            get;
            set;
        }

        public ActionData ActionData
        {
            get;
            set;
        }

        public ItemData ItemData
        {
            get;
            set;
        }

        public EffectData EffectData
        {
            get;
            set;
        }

        public SeedData SeedData
        {
            get;
            set;
        }

        public bool IsMasterMode
        {
            get;
            private set;
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            base.Initialize();

            SettingData data = Resources.Load<SettingData>(SettingDataFilePath);

            IsMasterMode = data.InternMaster;
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            CharacterData = null;
        }

        //---------------------------------------------------
        // InitializeCoroutine
        //---------------------------------------------------
        override protected IEnumerator InitializeCoroutine()
        {
            yield return Common.LoadAsync(LoadPath("CharacterData"),
                                    (obj) =>
                                    {
                                        CharacterData = obj as CharacterData;
                                    });

            yield return Common.LoadAsync(LoadPath("StageData"),
                (obj) =>
                {
                    StageData = obj as StageData;
                });

            yield return Common.LoadAsync(LoadPath("BulletData"),
                (obj) =>
                {
                    BulletData = obj as BulletData;
                });

            yield return Common.LoadAsync(LoadPath("ActionData"),
                (obj) =>
                {
                    ActionData = obj as ActionData;
                });

            yield return Common.LoadAsync(LoadPath("ItemData"),
                (obj) =>
                {
                    ItemData = obj as ItemData;
                });

            yield return Common.LoadAsync(LoadPath("EffectData"),
                (obj) =>
                {
                    EffectData = obj as EffectData;
                });

            yield return Common.LoadAsync(LoadPath("SeedData"),
                (obj) =>
                {
                    SeedData = obj as SeedData;
                });

            EndInitilaize();

        }

        //---------------------------------------------------
        // LoadPath
        //---------------------------------------------------
        string LoadPath(string masterName)
        {
            return "Master/" + masterName;
        }

        //---------------------------------------------------
        // GetMasterName
        //---------------------------------------------------
        static public string GetMasterName(Master.TYPE type)
        {
            string[] names = {"CharacterData", "StageData", "BulletData", "ActionData", "ItemData", "EffectData"};
            return names[(int)type];
        }

        //---------------------------------------------------
        // CreateActionDataHash
        //---------------------------------------------------
        public static byte[] CreateActionDataHash(ActionData actionData)
        {
            string src = "";

            for(int i = 0; i < actionData.GetLength(); i++)
            {
                src += actionData.GetData(i).ActionIntervalFrameCount.ToString();
                src += actionData.GetData(i).ActionType.ToString();
                src += actionData.GetData(i).ActionPrefabPath.ToString();
            }

            byte[] tempByte = ASCIIEncoding.ASCII.GetBytes(src);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(tempByte);
            return hash;
        }

        //---------------------------------------------------
        // CreateActionDataHash
        //---------------------------------------------------
        public static byte[] CreateBulletDataHash(BulletData data)
        {
            string src = "";

            for(int i = 0; i < data.GetLength(); i++)
            {
                src += data.GetData(i).Speed.ToString("f10");
                src += data.GetData(i).RangeBlock.ToString("f10");
                src += data.GetData(i).AttackPower.ToString("f10");
                src += data.GetData(i).LifeFrameCount.ToString("f10");
                src += data.GetData(i).DamageType.ToString();
                src += data.GetData(i).ViewPrefabPath.ToString();
                src += data.GetData(i).SpawnEffectPrefabPath;
            }

            byte[] tempByte = ASCIIEncoding.ASCII.GetBytes(src);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(tempByte);
            return hash;
        }

        //---------------------------------------------------
        // CreateActionDataHash
        //---------------------------------------------------
        public static byte[] CreateCharacterDataHash(CharacterData data)
        {
            string src = "";

            for(int i = 0; i < data.GetLength(); i++)
            {
                src += data.GetData(i).Hp.ToString("f10");
                src += data.GetData(i).AttackPowerBias.ToString("f10");
                src += data.GetData(i).MoveSpeed.ToString("f10");
                src += data.GetData(i).InitCorrectionPos.ToString("f10");
                src += data.GetData(i).ViewPrefabPath.ToString();
            }

            byte[] tempByte = ASCIIEncoding.ASCII.GetBytes(src);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(tempByte);
            return hash;
        }

        //---------------------------------------------------
        // CreateActionDataHash
        //---------------------------------------------------
        public static byte[] CreateEffectDataHash(EffectData data)
        {
            string src = "";

            for(int i = 0; i < data.GetLength(); i++)
            {
                src += data.GetData(i).Type.ToString();
                src += data.GetData(i).PrefabPath.ToString();
            }

            byte[] tempByte = ASCIIEncoding.ASCII.GetBytes(src);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(tempByte);
            return hash;
        }

        //---------------------------------------------------
        // CreateItemDataHash
        //---------------------------------------------------
        public static byte[] CreateItemDataHash(ItemData data)
        {
            string src = "";

            for(int i = 0; i < data.GetLength(); i++)
            {
                src += data.GetData(i).ItemType.ToString();
                src += data.GetData(i).EffectiveFrameCount.ToString("f10");
                src += data.GetData(i).Value.ToString();
                src += data.GetData(i).ActionPrefabPath.ToString();
                src += data.GetData(i).ViewPrefabPath.ToString();
                src += data.GetData(i).InitCorrectionPos.ToString("f10");
            }

            byte[] tempByte = ASCIIEncoding.ASCII.GetBytes(src);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(tempByte);
            return hash;
        }

        //---------------------------------------------------
        // CreateStageDataHash
        //---------------------------------------------------
        public static byte[] CreateStageDataHash(StageData data)
        {
            string src = "";

            for(int i = 0; i < data.GetLength(); i++)
            {
                src += data.GetData(i).BlockType.ToString();
                src += data.GetData(i).viewPrefabPath.ToString();
            }

            byte[] tempByte = ASCIIEncoding.ASCII.GetBytes(src);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(tempByte);
            return hash;
        }

        //---------------------------------------------------
        // CreateSeedDataHash
        //---------------------------------------------------
        public static byte[] CreateSeedDataHash(SeedData data)
        {
            string src = "";

            for(int i = 0; i < data.GetLength(); i++)
            {
                src += data.GetData(i).ToString();
            }

            byte[] tempByte = ASCIIEncoding.ASCII.GetBytes(src);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(tempByte);
            return hash;
        }

        //---------------------------------------------------
        // CheckData
        //---------------------------------------------------
        public bool CheckData(Master.TYPE type)
        {
            SettingData securityObject = Resources.Load<SettingData>(SettingDataFilePath);

            if (type == TYPE.ACTION)
            {
                return Common.CheckHash(securityObject.GetHashData(type), CreateActionDataHash(ActionData));
            }
            else if (type == TYPE.BULLET)
            {
                return Common.CheckHash(securityObject.GetHashData(type), CreateBulletDataHash(BulletData));
            }
            else if (type == TYPE.CHARACTER)
            {
                return Common.CheckHash(securityObject.GetHashData(type), CreateCharacterDataHash(CharacterData));
            }
            else if (type == TYPE.EFFECT)
            {
                return Common.CheckHash(securityObject.GetHashData(type), CreateEffectDataHash(EffectData));
            }
            else if (type == TYPE.ITEM)
            {
                return Common.CheckHash(securityObject.GetHashData(type), CreateItemDataHash(ItemData));
            }
            else if (type == TYPE.STAGE)
            {
                return Common.CheckHash(securityObject.GetHashData(type), CreateStageDataHash(StageData));
            }
            else if (type == TYPE.SEED)
            {
                return Common.CheckHash(securityObject.GetHashData(type), CreateSeedDataHash(SeedData));
            }

            return false;
        }

        //---------------------------------------------------
        // CheckDataAll
        //---------------------------------------------------
        public bool CheckDataAll()
        {
            for (int i = 0; i < (int)Master.TYPE.MAX; i++)
            {
                if (CheckData((Master.TYPE)i) == false)
                {
                    return false;
                }
            }

            return true;
        }


    }
}
