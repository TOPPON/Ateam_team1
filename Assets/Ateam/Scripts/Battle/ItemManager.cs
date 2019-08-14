using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    public struct ItemSpawnData
    {
        public ItemSpawnData(ItemData.ITEM_TYPE type, Vector2 blockPos, int actorId)
        {
            this.ItemType   = type;
            this.BlockPos   = blockPos;
            this.ActorId    = actorId;
        }

        public ItemData.ITEM_TYPE ItemType;
        public Vector2 BlockPos;
        public int ActorId;
    }

    public class ItemManager : BaseMonoBehaviour, IObserver
    {
        [SerializeField]
        StageManager _stageManager = null;

        [SerializeField]
        string _itemPrefabPath = null;

        [SerializeField]
        int _itemCreateInterval = 30;

        bool _isInitilaize = false;
        public bool IsInitilaize
        {
            get { return _isInitilaize; }
        }

        public delegate void ItemSpawnCallBack(ItemSpawnData ItemData);
        public ItemSpawnCallBack ItenSpawnCallBackDelegate
        {
            get;
            set;
        }

        Dictionary<string, Action<Hashtable>> _notifyList   = new Dictionary<string, Action<Hashtable>>();
        BattleModel _battleModel                            = null; 
        GameObject _itemGameObject                          = null;
        bool _enable                                        = false;
        List<Item> _itemList                                = new List<Item>();
        int _itemDataCurrentIndex                           = 0;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize(BattleModel _model)
        {
            _battleModel = _model;

            _battleModel.AddListerner(this);

            _itemGameObject = Resources.Load(_itemPrefabPath) as GameObject;

            InitializeObserver();

            _isInitilaize = true;
        }

        //---------------------------------------------------
        // InitializeObserver
        //---------------------------------------------------
        public void InitializeObserver()
        {
            _notifyList.Add("EVENT_Time", EVENT_Time);
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            _stageManager   = null;

            _battleModel    = null;
        }
    
        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        public void OnNotify(string eventName, Hashtable hashTable)
        {
            if (_notifyList.ContainsKey(eventName))
            {
                _notifyList[eventName](hashTable);
            }
        }

        //---------------------------------------------------
        // EVENT_Time
        //---------------------------------------------------
        public void EVENT_Time(Hashtable table)
        {
            if (_enable == false)
            {
                return;
            }

            if (table.ContainsKey("frameTime"))
            {
                if ((int)table["frameTime"] % _itemCreateInterval == 0)
                {
                    CreateItem();   
                }
            }
        }

        //---------------------------------------------------
        // CreateItem
        //---------------------------------------------------
        void CreateItem()
        {
            while (true)
            {
                int y = UnityEngine.Random.Range(0, _stageManager.VerticalBlock);
                int x = UnityEngine.Random.Range(0, _stageManager.HorizontalBlock);

                if (_stageManager.GetBlockType(new Vector2(x, y)) == Define.Stage.BLOCK_TYPE.NORMAL)
                {
                    ItemData itemList   = ApplicationManager.Instance.Master.ItemData;

                    GameObject go           = Instantiate(_itemGameObject);
                    go.transform.position   = _stageManager.GetBlockPosition(x, y);
                    Item item               = go.AddComponent<Item>();

                    item.Initialize(_itemDataCurrentIndex, obj =>{

                        _itemList.Remove(obj);
                    });

                    _itemList.Add(item);
                    ItenSpawnCallBackDelegate(new ItemSpawnData(itemList.GetData(_itemDataCurrentIndex).ItemType, new Vector2(x, y), item.ActorModel.ActorId));

                    _itemDataCurrentIndex++;

                    if (_itemDataCurrentIndex >= itemList.GetLength())
                    {
                        _itemDataCurrentIndex = 0;
                    }

                    break;
                }
            }
        }

        //---------------------------------------------------
        // CreateStart
        //---------------------------------------------------
        public void CreateStart()
        {
            _enable = true;
            CreateItem();
        }

        //---------------------------------------------------
        // GetItemList
        //---------------------------------------------------
        public List<Item> GetItemList()
        {
            return _itemList;
        }
    }
}