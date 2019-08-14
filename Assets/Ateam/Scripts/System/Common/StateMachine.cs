using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    public class StateData
    {
        public float dt { get; set; }
        public float totalTime { get; set; }
        public float frameCount { get; set; }
        public Hashtable table { get; set; }
    }

    public class State 
    {
        public Action<StateData> Enter;
        public Action<StateData> Action;
        public Action<StateData> Exit;

        //---------------------------------------------------
        // Create
        //---------------------------------------------------
        public static State Create(Action<StateData> enter, Action<StateData> action, Action<StateData> exit)
        {
            State state = new State();

            state.Enter     = enter;
            state.Action    = action;
            state.Exit      = exit;

            return state;
        }
    }

    public class StateMachine<T>
    { 
        private Dictionary<T, State> _stateDic  = null;
        private State _currentState             = null;
        private T _nextStateKey                 = default(T);
        private Hashtable _nextDataTable        = null;
        private StateData _stateData            = new StateData();
        private T _currentStateKey              = default(T);

        public T CurrentStateKey
        {
            get { return _currentStateKey; }
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize(IEqualityComparer<T> comparer)
        {
            if (_stateDic == null)
            {
                _stateDic = new Dictionary<T, State>(comparer);
            }
        }

        //---------------------------------------------------
        // Add
        //---------------------------------------------------
        public void Add(T key, Action<StateData> enter, Action<StateData> action, Action<StateData> exit)
        {
            State state = State.Create(enter, action, exit);
            _stateDic.Add(key, state);
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        public void Update(float dt)
        {
            _stateData.dt           = dt;
            _stateData.totalTime    += _stateData.dt;
            _stateData.frameCount++;

            if (!_nextStateKey.Equals(default(T)))
            {
                if (_currentState != null)
                {
                    if (_currentState.Exit != null)
                    {
                        _currentState.Exit(_stateData);
                    }
                }

                _stateData.totalTime    = 0;
                _stateData.frameCount   = 0;
                _stateData.table        = _nextDataTable;
                _currentState           = _stateDic[_nextStateKey];

                _currentStateKey        = _nextStateKey;
                _nextStateKey           = default(T);

                if (_currentState != null && _currentState.Enter != null)
                {
                    _currentState.Enter(_stateData);
                }
            }
            else if (_currentState != null && _currentState.Action != null)
            {
                _currentState.Action(_stateData);
            }
        }

        //---------------------------------------------------
        // SetNextState
        //---------------------------------------------------
        public void SetNextState(T key, Hashtable table = null)
        {
            _nextStateKey   = key;
            _nextDataTable  = table;
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        public void Release()
        {
            if (_stateDic != null)
            {
                _stateDic.Clear();
                _stateDic = null;
                _stateData = null;
            }
        }
    }
}