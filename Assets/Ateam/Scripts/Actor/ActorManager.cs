using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public struct ActorData
    {
        public ActorData(Define.ActorType type, Actor actor)
        {
            this.Type   = type;
            this.Actor  = actor;
        }

        public Define.ActorType Type;
        public Actor Actor;
    }

    public class ActorManager
    {
        private Dictionary<int, ActorData> _actorList = new Dictionary<int, ActorData>();
        private int _currentIndex = 0;

        public int Count
        {
            get { return _actorList.Count; }
        }
                        
        //---------------------------------------------------
        // AddActor
        //---------------------------------------------------
        public int AddActor(Actor actor, Define.ActorType type)
        {
            _actorList.Add(_currentIndex, new ActorData(type, actor));

            return _currentIndex++;
        }

        //---------------------------------------------------
        // RemoveActor
        //---------------------------------------------------
        public void RemoveActor(int actorId)
        {
            _actorList.Remove(actorId);
        }

        //---------------------------------------------------
        // RemoveAll
        //---------------------------------------------------
        public void RemoveAll()
        {
            _currentIndex = 0;
            _actorList.Clear();
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        public void Release()
        {
            _actorList.Clear();
            _actorList = null;
        }

        //---------------------------------------------------
        // GetActor
        //---------------------------------------------------
        public ActorData? GetActor(int id)
        {
            if (_actorList.ContainsKey(id))
            {
                return _actorList[id];
            }

            return null;
        }

    }
}