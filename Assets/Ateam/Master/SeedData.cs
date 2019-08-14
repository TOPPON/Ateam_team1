using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ateam
{
    public class SeedData : ScriptableObject
    {
        [SerializeField]
        private List<SeedDataList> _list = new List<SeedDataList>();

        public int GetData(int index)
        {
            if (_list.Count > index  && index >= 0)
            {
                return _list[index].Seed;
            }

            return 0;
        }

        public List<SeedDataList> GetList()
        {
            return _list;
        }

        public int GetLength()
        {
            return _list.Count;
        }

        [System.SerializableAttribute]
        public class SeedDataList
        {
            public int Seed;
        }
    }
}