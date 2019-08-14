using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class CharacterModel : ActorModel
    {
        /// <summary>
        /// ゲームAI公開用のキャラクターデータ
        /// </summary>
        public class Data
        {
            /// <summary>
            /// 初期化コンストラクタ
            /// </summary>
            public Data(int id, Define.Battle.TEAM_TYPE type, float hp, float maxHp, Vector3 forward, Vector2 blockPos, bool isMoveEnable)
            {
                this.ActorId        = id;
                this.TeamType       = type;
                this.Hp             = hp;
                this.MaxHp          = maxHp;
                this.BlockPos       = blockPos;
                this.IsMoveEnable   = isMoveEnable;
                this.Forward        = forward;
            }

            /// <summary>
            /// アクターID
            /// </summary>
            public int ActorId;

            /// <summary>
            /// 所属するチーム種類
            /// </summary>
            public Define.Battle.TEAM_TYPE TeamType;

            /// <summary>
            /// 現在のHP
            /// </summary>
            public float Hp;

            /// <summary>
            /// 最大のHP
            /// </summary>
            public float MaxHp;

            /// <summary>
            /// 現在自分がいるステージ上のブロック位置
            /// </summary>
            public Vector2 BlockPos;

            /// <summary>
            /// 移動可能か
            /// false : 移動中につき移動命令不可
            /// true  : 移動していないので移動可能
            /// </summary>
            public bool IsMoveEnable;

            /// <summary>
            /// 正面ベクトル
            /// </summary>
            public Vector2 Forward;
        }

        //AI用の公開データ
        Data _characterData = new Data(0, 0, 0, 0, Vector3.zero, Vector2.zero, true);
        public Data PublicCharacterData
        {
            get { return _characterData; }
        }

        int _characterId = 0;
        public int CharacterId
        {
            get { return _characterId; }
            set
            {
                _characterId = value;
                _observable.PushEvent("EVENT_CharacterId", Common.CreateHashTable("characterId", _characterId));
            }
        }
            
        public Define.Battle.TEAM_TYPE TeamId
        {
            get { return _characterData.TeamType; }
            set 
            {
                _characterData.TeamType = value;
                _observable.PushEvent("EVENT_TeamId", Common.CreateHashTable("teamId", _characterData.TeamType));
            }
        }

        float _speed = 0;
        public float Speed
        {
            get { return _speed; }
            set 
            {
                float diff  = value - _speed;
                _speed      = value;
                _observable.PushEvent("EVENT_Speed", Common.CreateHashTable("speed", _speed, "diff", diff));
            }
        }

        public float Hp
        {
            get { return _characterData.Hp; }
            set 
            {
                float diff              = value - _characterData.Hp;
                _characterData.Hp       = value;
                _observable.PushEvent("EVENT_Hp", Common.CreateHashTable("hp", _characterData.Hp, "diff", diff, "teamId", _characterData.TeamType));
            }
        }

        float _maxHp = 0;
        public float MaxHp
        {
            get { return _maxHp; }
            set
            { 
                _maxHp                  = value;
                _characterData.MaxHp    = value;
                _observable.PushEvent("EVENT_MaxHp", Common.CreateHashTable("maxHp", _maxHp));
            }
        }

        bool _invincible = false;
        public bool Invincible
        {
            get { return _invincible; }
            set 
            {
                _invincible = value;
                _observable.PushEvent("EVENT_Invincible", Common.CreateHashTable("invincible", _invincible));
            }
        }

        Vector3 _targetPos;
        public Vector3 TargetPos
        {
            get { return _targetPos; }
            set
            {
                _targetPos = value;
                _observable.PushEvent("EVENT_TargetPos", Common.CreateHashTable("targetPos", _targetPos));
            }
        }

        bool _isMoveUpdate = false;
        public bool IsMoveUpdate
        {
            get { return _isMoveUpdate; }
            set
            {
                _isMoveUpdate               = value;
                _characterData.IsMoveEnable = !value;

                _observable.PushEvent("EVENT_IsMoveUpdate", Common.CreateHashTable("isMoveUpdate", _isMoveUpdate));
            }
        }

        Vector3 _moveVec;
        public Vector3 MoveVec
        {
            get { return _moveVec; }
            set 
            {
                _moveVec = value;

                _observable.PushEvent("EVENT_MoveVec", Common.CreateHashTable("moveVec", _moveVec));
            }
        }

        Vector3 _direction;
        public Vector3 Direction
        {
            get { return _direction; }
            set
            { 
                _direction                  = value;
                _characterData.Forward.x    = value.x;
                _characterData.Forward.y    = value.z;
                _characterData.Forward.Normalize();

                _observable.PushEvent("EVENT_Direction", Common.CreateHashTable("direction", _direction));
            }
        }

        float _attackPowerBias = 1;
        public float AttackPowerBias
        {
            get { return _attackPowerBias; }
            set
            { 
                float diff          = value - _attackPowerBias;
                _attackPowerBias    = value;
                _observable.PushEvent("EVENT_AttackPowerBias", Common.CreateHashTable("attackPowerBias", _attackPowerBias, "diff", diff));
            }
        }

        public Vector2 BlockPos
        {
            get { return _characterData.BlockPos; }
            set
            {
                _characterData.BlockPos = value;
                _observable.PushEvent("EVENT_BlockPos", Common.CreateHashTable("blockPos", _characterData.BlockPos));
            }
        }


        private CharacterData.CharacterDataList _basedata;
        public CharacterData.CharacterDataList Basedata
        {
            get { return _basedata; }
            set
            { 
                _basedata = value;
                _observable.PushEvent("EVENT_Basedata", Common.CreateHashTable("basedata", _basedata));
            }
        }

        //---------------------------------------------------
        // SetActorId
        //---------------------------------------------------
        public void SetActorId(int id)
        {
            _characterData.ActorId = id;
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override public void Release()
        {
            base.Release();
        }
    }
}