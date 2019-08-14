using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    public class Item :Actor
    {
        ItemModel _itemModel;
        public ItemModel ItemModel
        {
            get { return _itemModel; }
        }

        Dictionary<string, Action<Hashtable>> _notifyList   = new Dictionary<string, Action<Hashtable>>();
        ItemView _itemView                                  = null;
        BaseAction _action                                  = null;
        BoxCollider _collider                               = null;

        public delegate void EndCallBack(Item item);
        EndCallBack EndCallBackDelegate;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            base.Initialize();

            _itemModel = new ItemModel();
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize(int id, EndCallBack callback)
        {
            ItemData.ItemDataList data = ApplicationManager.Instance.Master.ItemData.GetData (id);

            _itemModel.EffectiveFrameCount  = data.EffectiveFrameCount;
            _itemModel.ItemData             = data;
            EndCallBackDelegate             += callback;

            GameObject view = new GameObject("ItemView");
            view.transform.SetParent(gameObject.transform, false);
            _itemView = view.AddComponent<ItemView>();

            if (data.ViewPrefabPath != "")
            {
                _itemView.SetAvatar(data.ViewPrefabPath);
            }

            _itemModel.AddListerner(this);
            _itemModel.AddListerner(_itemView);

            if (data.ActionPrefabPath != "")
            {
                GameObject action = Instantiate(Resources.Load(data.ActionPrefabPath)) as GameObject;
                action.transform.SetParent(gameObject.transform, false);
                _action = action.GetComponent<BaseAction>();
                _action.ActionModel.AddListerner(this);
            }

            _collider           = GetComponent<BoxCollider>();
            transform.position += data.InitCorrectionPos;

            InitializeObderver();

            base.Initialize(Define.ActorType.ITEM, _itemModel, _itemView);
        }

        //---------------------------------------------------
        // InitializeObderver
        //---------------------------------------------------
        void InitializeObderver()
        {
            _notifyList.Add("EVENT_IsEnd", EVENT_IsEnd);
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            base.Release();

            if (_notifyList != null)
            {
                _notifyList.Clear();
                _notifyList = null;
            }
        }

        //---------------------------------------------------
        // OnTriggerEnter
        //---------------------------------------------------
        public void OnTriggerEnter(Collider collider)
        {
            Character character = collider.gameObject.GetComponent<Character>();
            if (character == null)
            {
                return;
            }
                
            if (_action != null)
            {
                _collider.enabled = false;
                _itemView.gameObject.SetActive(false);

                _action.Initialize(_itemModel.ItemData.EffectiveFrameCount, character);
                _action.ActionStart();

                EndCallBackDelegate(this);
            }
        }

        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        override public void OnNotify(string eventName, Hashtable hashTable)
        {
            if (_notifyList.ContainsKey(eventName))
            {
                _notifyList[eventName](hashTable);
            }
        }

        //---------------------------------------------------
        // EVENT_IsEnd
        //---------------------------------------------------
        public void EVENT_IsEnd(Hashtable table)
        {
            if (table.ContainsKey("isEnd"))
            {
                if ((bool)table["isEnd"] == true)
                {
                    Destroy(_action.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}