using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ateam
{
    public class ActionManager : BaseMonoBehaviour 
    {
        List<BaseAction> _actionList = new List<BaseAction>();

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {

        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize(Character character)
        {
            ActionData Master =  ApplicationManager.Instance.Master.ActionData;

            for(int i = 0; i < Master.GetLength(); i++)
            {
                ActionData.ActionDataList data = Master.GetData(i);

                if (data.ActionPrefabPath != "")
                {
                    GameObject go                   = Instantiate(Resources.Load(data.ActionPrefabPath)) as GameObject;
                    BaseAction baseAction           = go.GetComponent<BaseAction>();

                    go.transform.SetParent(gameObject.transform);
                    _actionList.Insert((int)data.ActionType, baseAction);
                    baseAction.Initialize(data.ActionIntervalFrameCount, character);
                }
            }
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            if (_actionList != null)
            {
                _actionList.Clear();
                _actionList = null;
            }
        }
            
        //---------------------------------------------------
        // StartAction
        //---------------------------------------------------
        public bool StartAction(Define.Battle.ACTION_TYPE actionType)
        {
            return _actionList[(int)actionType].ActionStart();
        }

        //---------------------------------------------------
        // SetEnableAll
        //---------------------------------------------------
        public void SetEnableAll(bool enable)
        {
            for (int i = 0; i < _actionList.Count; i++)
            {
                _actionList[i].enabled = enable;
            }
        }
    }
}