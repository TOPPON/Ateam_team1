using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Ateam
{
    namespace Define
    {
        public enum EffectType
        {
            HIT,
            EXPLOSION,
            ATTACK_POWERUP,
            SPEED_UP,
            HP_RECOVER,
            SHORT_ATTACK,
        }
    }

    public class EffectData : ScriptableObject
    {
        [SerializeField]
        List<EffectDataList> _list = new List<EffectDataList>();

        public EffectDataList GetData(int id)
        {
            if (_list.Count < id  || id >= 0)
            {
                return _list[id];
            }

            return null;
        }

        public string GetPath(Define.EffectType type)
        {
            EffectDataList data = _list.Find(obj => obj.Type == type);

            return data.PrefabPath;
        }

        public int GetLength()
        {
            return _list.Count;
        }

        [System.SerializableAttribute]
        public class EffectDataList
        {
            public string Name;
            public Define.EffectType Type;
            public string PrefabPath;
        }
    }
}