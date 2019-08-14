using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public class BattleModel : BaseModel
    {
        protected int _frameTime = 0;
        public int FrameTime
        {
            get { return _frameTime; }
            set 
            { 
                _frameTime = value;
                _observable.PushEvent("EVENT_Time", Common.CreateHashTable("frameTime", _frameTime));
            }
        }

        Dictionary<Define.Battle.TEAM_TYPE, float> _totalHpList = new Dictionary<Define.Battle.TEAM_TYPE, float>();
        public Dictionary<Define.Battle.TEAM_TYPE, float> TotalHp
        {
            get { return _totalHpList; }
        }

        Dictionary<Define.Battle.TEAM_TYPE, float> _maxHpList = new Dictionary<Define.Battle.TEAM_TYPE, float>();
        public Dictionary<Define.Battle.TEAM_TYPE, float> MaxHp
        {
            get { return _maxHpList; }
        }

        protected Dictionary<Define.Battle.TEAM_TYPE, List<Character>> _teamCharacterList = new Dictionary<Define.Battle.TEAM_TYPE, List<Character>>(); 
        public Dictionary<Define.Battle.TEAM_TYPE, List<Character>> TeamCharacterList
        {
            get { return _teamCharacterList; }
        }

        protected Dictionary<Define.Battle.TEAM_TYPE, List<CharacterModel.Data>> _teamCharacterDataList = new Dictionary<Define.Battle.TEAM_TYPE, List<CharacterModel.Data>>();
        public Dictionary<Define.Battle.TEAM_TYPE, List<CharacterModel.Data>> TeamCharacterDataList
        {
            get { return _teamCharacterDataList; }
        }
            
        protected Dictionary<Define.Battle.TEAM_TYPE, BaseBattleAISystem> _AISystemList = new Dictionary<Define.Battle.TEAM_TYPE, BaseBattleAISystem>();
        public Dictionary<Define.Battle.TEAM_TYPE, BaseBattleAISystem> AISystemList
        {
            get { return _AISystemList; }
        }

        protected Dictionary<Define.Battle.TEAM_TYPE, string> _TeamNameList = new Dictionary<global::Ateam.Define.Battle.TEAM_TYPE, string>();
        public Dictionary<Define.Battle.TEAM_TYPE, string> TeamNameList
        {
            get { return _TeamNameList; }
        }

        //---------------------------------------------------
        // AddAISystem
        //---------------------------------------------------
        public void AddAISystem(Define.Battle.TEAM_TYPE type, BaseBattleAISystem aiSystem, string name)
        {
            _AISystemList.Add(type, aiSystem);
            _TeamNameList.Add(type, name);

            _observable.PushEvent("EVENT_SetTeamName", Common.CreateHashTable("teamType", type, "name", name));
        }

        //---------------------------------------------------
        // AddCharacter
        //---------------------------------------------------
        public void AddCharacter(Define.Battle.TEAM_TYPE type, Character character)
        {
            if (_teamCharacterList.ContainsKey(type))
            {
                _teamCharacterList[type].Add(character);
                _teamCharacterDataList[type].Add(character.CharacterModel.PublicCharacterData);
            }
            else
            {
                List<Character> list = new List<Character>();
                list.Add(character);
                _teamCharacterList.Add(type, list);

                List<CharacterModel.Data> dataList = new List<CharacterModel.Data>();
                dataList.Add(character.CharacterModel.PublicCharacterData);
                _teamCharacterDataList.Add(type, dataList);
            }
        }

        //---------------------------------------------------
        // SetTotalHp
        //---------------------------------------------------
        public void SetTotalHp(Define.Battle.TEAM_TYPE type, float hp)
        {
            if (_totalHpList.ContainsKey(type))
            {
                _totalHpList[type] += hp;
            }
            else
            {
                _totalHpList.Add(type, hp);
            }

            _observable.PushEvent("EVENT_SetTotalHp", Common.CreateHashTable("teamType", type, "hp", _totalHpList[type]));
        }

        //---------------------------------------------------
        // SetMaxHp
        //---------------------------------------------------
        public void SetMaxHp(Define.Battle.TEAM_TYPE type, float hp)
        {
            if (_maxHpList.ContainsKey(type))
            {
                _maxHpList[type] = hp;
            }
            else
            {
                _maxHpList.Add(type, hp);
            }

            _observable.PushEvent("EVENT_SetMaxHp", Common.CreateHashTable("teamType", type, "hp", _maxHpList[type]));
        }

        //---------------------------------------------------
        // ShowResultWindow
        //---------------------------------------------------
        public void ShowResultWindow(Hashtable table)
        {
            _observable.PushEvent("EVENT_ShowResultWindow", table);
        }
    }
}