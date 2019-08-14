using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public class ItemData : ScriptableObject
    {
        public enum ITEM_TYPE
        {
            ATTACK_UP,
            SPEED_UP,
            HP_RECOVER,
        }

        [SerializeField]
        List<ItemDataList> _list = new List<ItemDataList>();

        public ItemDataList GetData(int id)
        {
            if (_list.Count < id  || id >= 0)
            {
                return _list[id];
            }

            return null;
        }

        public int GetLength()
        {
            return _list.Count;
        }

        [System.SerializableAttribute]
        public class ItemDataList
        {
            public string Name;
            public ITEM_TYPE ItemType;
            public float Value;
            public int EffectiveFrameCount;
            public string ActionPrefabPath;
            public string ViewPrefabPath;
            public Vector3 InitCorrectionPos;
        }
    }
}