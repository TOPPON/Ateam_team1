using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    public class Actor : BaseMonoBehaviour, IObserver
    {
        ActorView _actorView    = null;

        ActorModel _actorModel  = null;
        public ActorModel ActorModel
        {
            get { return _actorModel; }
        }
     
        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            if (_actorView != null)
            {
                _actorModel.RemoveListerner(_actorView);
                _actorView = null;
            }

            if (_actorModel != null)
            {
                ApplicationManager.Instance.ActorManager.RemoveActor(_actorModel.ActorId);

                _actorModel.RemoveListerner(this);
                _actorModel.Release();
                _actorModel = null;
            }
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        protected void Initialize(Define.ActorType actorType, ActorModel model, ActorView actorView)
        {
            int actorId = ApplicationManager.Instance.ActorManager.AddActor(this, actorType);
            _actorModel = model;

            _actorModel.AddListerner(this);
            _actorModel.ActorId = actorId;

            if (actorView != null)
            {
                _actorView = actorView;
                _actorModel.AddListerner(_actorView);
            }
        }

        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        virtual public void OnNotify(string eventName, Hashtable hashTable)
        {
            // sample
            /*
            Debug.Log(eventName);

            if (hashTable.ContainsKey("actorId"))
            {
                Debug.Log((int)hashTable["actorId"]);
            }
            */
        }
    }
}