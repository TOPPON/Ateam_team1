using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ateam
{
    public class BaseActionModel : BaseModel 
    {
        int _intervalFrameCount = 0;
        public int InterValFrameCount
        {
            get { return _intervalFrameCount; }
            set 
            {
                _intervalFrameCount = value;
                _observable.PushEvent("EVENT_InterValFrameCount", Common.CreateHashTable("intervalFrameCount", _intervalFrameCount));
            }
        }

        int _currentIntervalFrameCount = 0;
        public int CurrentInterValFrameCount
        {
            get { return _currentIntervalFrameCount; }
            set 
            {
                _currentIntervalFrameCount = value;
                _observable.PushEvent("EVENT_CurrentInterValFrameCount", Common.CreateHashTable("currentIntervalFrameCount", _currentIntervalFrameCount));
            }
        }

        bool _isEnd = true;
        public bool IsEnd
        {
            get { return _isEnd; }
            set
            {
                _isEnd = value;
                _observable.PushEvent("EVENT_IsEnd", Common.CreateHashTable("isEnd", _isEnd));
            }
        }
    }
}
