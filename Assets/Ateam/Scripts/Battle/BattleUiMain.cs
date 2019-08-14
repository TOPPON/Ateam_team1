using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    public class BattleUiMain : BaseUi, IObserver
    {
        Dictionary<string, Action<Hashtable>> _notifyList = new Dictionary<string, Action<Hashtable>>();

        [SerializeField]
        private Text _timerText = null;

        [SerializeField]
        private Text _playerTeamName = null;
        [SerializeField]
        private Text _enemyTeamName = null;

        [SerializeField]
        private Slider _playerTotalHpSlider = null;
        [SerializeField]
        private Slider _enemyTotalHpSlider = null;

        [SerializeField]
        private ResultUi _resultUi = null;

        [SerializeField]
        private Animator _animator = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            base.Initialize();

            _resultUi.gameObject.SetActive(false);
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            base.Release(); 
        }

        //---------------------------------------------------
        // InitializeCoroutine
        //---------------------------------------------------
        override protected IEnumerator InitializeCoroutine()
        {
            yield return null;

            InitializeObderver();

            EndInitilaize();
        }

        //---------------------------------------------------
        // InitializeObderver
        //---------------------------------------------------
        void InitializeObderver()
        {
            _notifyList.Add("EVENT_SetTeamName", EVENT_SetTeamName);
            _notifyList.Add("EVENT_Time", EVENT_Time);
            _notifyList.Add("EVENT_SetTotalHp", EVENT_SetTotalHp);
            _notifyList.Add("EVENT_SetMaxHp", EVENT_SetMaxHp);
            _notifyList.Add("EVENT_ShowResultWindow", EVENT_ShowResultWindow);
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
        // SetTeamName
        //---------------------------------------------------
        public void SetTeamName(List<string> teamNameList)
        {
            _playerTeamName.text  = teamNameList[0];
            _enemyTeamName.text  = teamNameList[1];
        }

        //---------------------------------------------------
        // EVENT_SetTeamName
        //---------------------------------------------------
        void EVENT_SetTeamName(Hashtable table)
        {
            Define.Battle.TEAM_TYPE teamId = (Define.Battle.TEAM_TYPE)table["teamType"];
            string teamName = (string)table["name"];

            if (teamId == Define.Battle.TEAM_TYPE.ALPHA)
            {
                _playerTeamName.text = teamName;
            }
            else if (teamId == Define.Battle.TEAM_TYPE.BRAVO)
            {
                _enemyTeamName.text = teamName;
            }
        }

        //---------------------------------------------------
        // EVENT_Time
        //---------------------------------------------------
        void EVENT_Time(Hashtable table)
        {
            int frameTime = (int)table["frameTime"];

            string minutes = Mathf.Floor(frameTime / (60 * Define.Config.FPS)).ToString("00");
            string seconds = Mathf.Floor((frameTime / Define.Config.FPS) % 60).ToString("00");

            _timerText.text = minutes + ":" + seconds;
        }

        //---------------------------------------------------
        // EVENT_SetTotalHp
        //---------------------------------------------------
        void EVENT_SetTotalHp(Hashtable table)
        {
            Define.Battle.TEAM_TYPE teamId = (Define.Battle.TEAM_TYPE)table["teamType"];
            float totalHp = (float)table["hp"];

            if (teamId == Define.Battle.TEAM_TYPE.ALPHA)
            {
                _playerTotalHpSlider.value = totalHp;
            }
            else if (teamId == Define.Battle.TEAM_TYPE.BRAVO)
            {
                _enemyTotalHpSlider.value = totalHp;
            }
        }

        //---------------------------------------------------
        // EVENT_SetMaxHp
        //---------------------------------------------------
        void EVENT_SetMaxHp(Hashtable table)
        {
            Define.Battle.TEAM_TYPE teamId = (Define.Battle.TEAM_TYPE)table["teamType"];
            float maxHp = (float)table["hp"];

            if (teamId == Define.Battle.TEAM_TYPE.ALPHA)
            {
                _playerTotalHpSlider.maxValue = maxHp;
                _playerTotalHpSlider.value = maxHp;
            }
            else if (teamId == Define.Battle.TEAM_TYPE.BRAVO)
            {
                _enemyTotalHpSlider.maxValue = maxHp;
                _enemyTotalHpSlider.value = maxHp;
            }
        }

        //---------------------------------------------------
        //EVENT_ShowResultWindow
        //---------------------------------------------------
        void EVENT_ShowResultWindow(Hashtable table)
        {
            _resultUi.SetResultData(table);
            _resultUi.gameObject.SetActive(true);
            _animator.Play("ShowResult", 0, 0.0f);
        }
    }
}