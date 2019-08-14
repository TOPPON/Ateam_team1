using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    public class Character : Actor
    {
        [SerializeField]
        BoxCollider _boxCollider = null;

        Dictionary<string, Action<Hashtable>> _notifyList   = new Dictionary<string, Action<Hashtable>>();

        CharacterView   _characterView = null;
        public CharacterView Characterview
        {
            get { return _characterView; }
        }

        CharacterModel  _characterModel = null;
        public CharacterModel CharacterModel
        {
            get { return _characterModel; }
        }

        ActionManager _actionManager = null;
        public ActionManager ActionManager
        {
            get { return _actionManager; }
        }
                   
        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            base.Initialize();

            _characterModel = new CharacterModel();
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize(int characterId, Define.Battle.TEAM_TYPE teamId, Vector3 pos)
        {
            CharacterData.CharacterDataList data = ApplicationManager.Instance.Master.CharacterData.GetData(characterId);

            _characterModel.MaxHp                       = data.Hp;
            _characterModel.Hp                          = _characterModel.MaxHp;
            _characterModel.Speed                       = data.MoveSpeed;
            _characterModel.TeamId                      = teamId;
            _characterModel.AttackPowerBias             = data.AttackPowerBias;
            transform.position                          = pos + data.InitCorrectionPos;
            _characterModel.Direction                   = transform.forward;
            _characterModel.TargetPos                   = transform.position;
            _actionManager                              = gameObject.AddComponent<ActionManager>();
            _characterModel.Basedata                    = data;

            _actionManager.Initialize(this);

            GameObject view = new GameObject("CharacterView");
            view.transform.SetParent(gameObject.transform, false);

            _characterView = view.AddComponent<CharacterView>();
            _characterView.SetTeamId(teamId);
            _characterView.SetAvatar(data.ViewPrefabPath);

            InitializeObderver();

            Initialize(Define.ActorType.CHARACTER, _characterModel, _characterView);
            _characterModel.SetActorId(_characterModel.ActorId);
        }

        //---------------------------------------------------
        // InitializeObderver
        //---------------------------------------------------
        void InitializeObderver()
        {
            _notifyList.Add("EVENT_Hp", EVENT_Hp);
            _notifyList.Add("EVENT_TargetPos", EVENT_TargetPos);
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            base.Release();
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            if (_characterModel.Hp <= 0)
            {
                return;
            }

            Vector3 vec = _characterModel.TargetPos - transform.position;

            if (vec == Vector3.zero || vec.magnitude < _characterModel.MoveVec.magnitude)
            {
                transform.position                      = _characterModel.TargetPos;
                _characterModel.IsMoveUpdate            = false;
            }
            else
            {
                transform.position += _characterModel.MoveVec;
            }

            Vector3 targetPositon       = transform.position + _characterModel.Direction;
            Quaternion targetRotation   = Quaternion.LookRotation(targetPositon - transform.position);
            transform.rotation          = Quaternion.Slerp(transform.rotation, targetRotation, Define.Battle.CHARACTER_ROT_SPEED);

            _characterModel.BlockPos = ApplicationManager.Instance.Battlesystem.StageManager.getPositionBlock(transform.position);
        }

        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        override public void OnNotify(string eventName, Hashtable hashTable)
        {
            if (_notifyList.ContainsKey(eventName))
            {
                _notifyList[eventName](hashTable);
            }
        }

        //---------------------------------------------------
        // EVENT_Hp
        //---------------------------------------------------
        void EVENT_Hp(Hashtable table)
        {
            if (table.ContainsKey("hp"))
            {
                bool enable = false;

                if ((float)table["hp"] <= 0)
                {
                    enable = false;
                }
                else
                {
                    enable = true;
                }

                _boxCollider.enabled = enable;
                _actionManager.SetEnableAll(enable);
            }
        }

        //---------------------------------------------------
        // EVENT_TargetPos
        //---------------------------------------------------
        void EVENT_TargetPos(Hashtable table)
        {
            if (table.ContainsKey("targetPos"))
            {
                _characterModel.IsMoveUpdate = true;

                Vector3 vec = (Vector3)table["targetPos"] - transform.position;

                _characterModel.MoveVec = vec.normalized * _characterModel.Speed;
                _characterModel.Direction = vec.normalized;
            }
        }
    }
}