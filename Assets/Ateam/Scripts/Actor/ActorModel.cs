using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class ActorModel : BaseModel
    {
        protected int _actorId = 0;
        public int ActorId
        { 
            get { return _actorId; } 
            set
            {
                _actorId = value;
                _observable.PushEvent("EVENT_ActorId", Common.CreateHashTable("actorId", _actorId));
            }
        }
    }
}