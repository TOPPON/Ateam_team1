using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ateam
{
    public class BulletView : ActorView
    {
        BulletData.BulletDataList _data = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {

        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize(BulletData.BulletDataList data)
        {
            _data = data;

            if (_data.SpawnEffectPrefabPath != "")
            {
                ApplicationManager.Instance.EffectManager.Play(_data.SpawnEffectPrefabPath, transform.position, Vector3.zero);
            }
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            _data = null;
        }

        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        override public void OnNotify(string eventName, Hashtable hashTable)
        {

        }
    }
}