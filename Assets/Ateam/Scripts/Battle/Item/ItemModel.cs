using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class ItemModel : ActorModel
    {
        protected int _effectiveFrameCount = 0;
        public int EffectiveFrameCount
        {
            get { return _effectiveFrameCount; }
            set
            { 
                _effectiveFrameCount = value;
                _observable.PushEvent("EVENT_EffectiveFrameCount", Common.CreateHashTable("effectiveFrameCount", _effectiveFrameCount));
            }
        }

        protected ItemData.ItemDataList _itemData;
        public ItemData.ItemDataList ItemData
        {
            get { return _itemData; }
            set
            { 
                _itemData = value;
                _observable.PushEvent("EVENT_ItemData", Common.CreateHashTable("itemData", _itemData));
            }
        }
    }
}