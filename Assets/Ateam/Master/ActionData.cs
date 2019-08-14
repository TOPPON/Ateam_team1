using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Ateam
{
    public class ActionData : ScriptableObject
    {
        [SerializeField]
        List<ActionDataList> _list = new List<ActionDataList>();

        public ActionDataList GetData(int id)
        {
            if (_list.Count < id  || id >= 0)
            {
                return _list[id];
            }

            return null;
        }

        public ActionDataList GetData(Define.Battle.ACTION_TYPE type)
        {
            return _list.Find(obj => obj.ActionType == type);
        }

        public int GetLength()
        {
            return _list.Count;
        }

        [System.SerializableAttribute]
        public class ActionDataList
        {
            public string Name;
            public Define.Battle.ACTION_TYPE ActionType;
            public int ActionIntervalFrameCount;
            public string ActionPrefabPath;
        }
    }
}