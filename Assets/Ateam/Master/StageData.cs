using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ateam
{
    public class StageData : ScriptableObject
    {
        [SerializeField]
        List<StageDataList> _list = new List<StageDataList>();

        public StageDataList GetData(int id)
        {
            if (_list.Count < id  || id >= 0)
            {
                return _list[id];
            }

            return null;
        }

        public StageDataList GetData(Define.Stage.BLOCK_TYPE type)
        {
            return _list.Find(obj => obj.BlockType == type);
        }

        public int GetLength()
        {
            return _list.Count;
        }
          
        [System.SerializableAttribute]
        public class StageDataList
        {
            public string Name;
            public Define.Stage.BLOCK_TYPE BlockType;
            public string viewPrefabPath;
            public string viewPrefabPathTwo;
        }
    }
}