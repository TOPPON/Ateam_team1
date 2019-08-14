using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Ateam
{
    public enum ACTION_STATE
    {
        NONE,
        START,
        UPDATE,
        END,
    }

    public class BaseAction : BaseMonoBehaviour, IObserver
    {    
        class JobtypeCompare : IEqualityComparer<ACTION_STATE>
        {
            public bool Equals(ACTION_STATE x, ACTION_STATE y)
            {
                return x == y;
            }

            public int GetHashCode(ACTION_STATE obj)
            {
                return (int)obj;
            }
        }

        StateMachine<ACTION_STATE> _stateMachine = new StateMachine<ACTION_STATE>();

        BaseActionModel _actionModel = new BaseActionModel();
        public BaseActionModel ActionModel
        {
            get { return _actionModel; }
        }

        protected Character _character;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            _actionModel.AddListerner (this);

            _stateMachine.Initialize(new JobtypeCompare());    
            InitilazeState();
        }

        //---------------------------------------------------
        // Initilaize
        //---------------------------------------------------
        public void Initialize(int intervalFrameCount, Character character)
        {
            _character                      = character;
            _actionModel.InterValFrameCount = intervalFrameCount;
        }

        //---------------------------------------------------
        // InitilazeState
        //---------------------------------------------------
        void InitilazeState()
        {
            _stateMachine.Add(ACTION_STATE.START, StartEnter, null, null);
            _stateMachine.Add(ACTION_STATE.UPDATE, UpdateEnter, UpdateAction, UpdateEnd);
            _stateMachine.Add(ACTION_STATE.END, EndEnter, null, null);
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            if (_actionModel != null) 
            {
                _actionModel.RemoveListerner (this);
                _actionModel.Release();
            }
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }

        //---------------------------------------------------
        // StartAction
        //---------------------------------------------------
        public bool ActionStart()
        {
            if (_actionModel.IsEnd)
            {
                _actionModel.CurrentInterValFrameCount = 0;
                _stateMachine.SetNextState(ACTION_STATE.START);    

                return true;
            }
            else
            {
                return false;
            }
        }

        //---------------------------------------------------
        // StartEnter
        //---------------------------------------------------
        virtual protected void StartEnter(StateData data)
        {
            _actionModel.IsEnd = false; 
            _stateMachine.SetNextState(ACTION_STATE.UPDATE);
        }

        //---------------------------------------------------
        // StartEnter
        //---------------------------------------------------
        virtual protected void UpdateEnter(StateData data)
        {

        }

        //---------------------------------------------------
        // UpdateAction
        //---------------------------------------------------
        virtual protected void UpdateAction(StateData data)
        {    
            _actionModel.CurrentInterValFrameCount++;

            if (_actionModel.CurrentInterValFrameCount > _actionModel.InterValFrameCount)
            {
                _stateMachine.SetNextState(ACTION_STATE.END);
            }
        }

        //---------------------------------------------------
        // UpdateEnd
        //---------------------------------------------------
        virtual protected void UpdateEnd(StateData data)
        {

        }

        //---------------------------------------------------
        // EndEnter
        //---------------------------------------------------
        virtual protected void EndEnter(StateData data)
        {
            _actionModel.IsEnd = true;
        }

        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        public void OnNotify(string eventName, Hashtable hashTable)
        {
            
        }

    }
}