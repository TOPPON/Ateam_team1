using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public class SettingData : ScriptableObject
    {
        public bool InternMaster = false;

        public List<HashDataList> _hashList = new List<HashDataList>();

        public void AddHashData(HashDataList data)
        {
            if (GetHashData(data.type) == null)
            {
                _hashList.Add(data);
            }
            else
            {
                foreach (HashDataList list in _hashList)
                {
                    if (list.type == data.type)
                    {
                        list.hash = data.hash;
                        break;
                    }
                }
            }
        }

        public byte[] GetHashData(Master.TYPE type)
        {
            foreach (HashDataList data in _hashList)
            {
                if (data.type == type)
                {
                    return data.hash;
                }
            }

            return null;
        }

        [System.SerializableAttribute]
        public class HashDataList
        {
            public Master.TYPE type;
            public byte[] hash;

            public HashDataList(Master.TYPE type, byte[] hash)
            {
                this.type = type;
                this.hash = hash;
            }
        }
    }
}