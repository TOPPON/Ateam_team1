using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public class CharacterData : ScriptableObject
    {
        [SerializeField]
        List<CharacterDataList> _list = new List<CharacterDataList>();

        public CharacterDataList GetData(int id)
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
        public class CharacterDataList
        {
            public string Name;
            public int Hp;
            public float AttackPowerBias;
            public float MoveSpeed;
            public string ViewPrefabPath;
            public Vector3 InitCorrectionPos;
        }
    }
}