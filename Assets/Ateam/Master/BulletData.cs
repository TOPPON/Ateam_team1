using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ateam
{
    public class BulletData : ScriptableObject
    {
        [SerializeField]
        List<BulletDataList> _list = new List<BulletDataList>();

        public BulletDataList GetData(int id)
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
        public class BulletDataList
        {
            public string Name;
            public float Speed;
            public Vector2 RangeBlock;
            public int AttackPower;
            public int LifeFrameCount;
            public BulletModel.DAMAGE_TYPE DamageType;
            public string ViewPrefabPath;
            public string SpawnEffectPrefabPath;
        }
    }
}