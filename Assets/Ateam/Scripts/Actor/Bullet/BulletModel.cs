using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class BulletModel : ActorModel
    {
        public enum DAMAGE_TYPE
        {
            HIT,        //当たったらダメージ
            END_LIFE,    //ライフフレームカウント０時にダメージ
        }

        protected float _speed = 0;
        public float Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                _observable.PushEvent("EVENT_Speed", Common.CreateHashTable("speed", _speed));
            }
        }

        protected Vector2 _rangeBlock = Vector2.zero;
        public Vector2 RangeBlock
        {
            get { return _rangeBlock; }
            set 
            {
                _rangeBlock = value;
                _observable.PushEvent("EVENT_RangeBlock", Common.CreateHashTable("rangeBlock", _rangeBlock));
            }
        }

        protected float _attackPower; 
        public float AttackPower
        {
            get { return _attackPower; }
            set
            {
                _attackPower = value;
                _observable.PushEvent("EVENT_AttackPower", Common.CreateHashTable("attackPower", _attackPower));
            }
        }

        protected int _lifeFrameCount = 0;
        public int LifeFrameCount
        {
            get { return _lifeFrameCount; }
            set
            { 
                _lifeFrameCount = value;
                _observable.PushEvent("EVENT_LifeFrameCount", Common.CreateHashTable("lifeFrameCount", _lifeFrameCount));
            }
        }

        protected int _currentLifeFrameCount = 0;
        public int CurrentLifeFrameCount
        {
            get { return _currentLifeFrameCount; }
            set
            { 
                _currentLifeFrameCount = value;
                _observable.PushEvent("EVENT_CurrentLifeFrameCount", Common.CreateHashTable("currentLifeFrameCount", _currentLifeFrameCount));
            }
        }

        protected DAMAGE_TYPE _damageType;
        public DAMAGE_TYPE DamageType
        {
            get { return _damageType; }
            set
            { 
                _damageType = value;
                _observable.PushEvent("EVENT_DamageType", Common.CreateHashTable("damageType", _damageType));
            }
        }

        protected Vector3 _moveVec;
        public Vector3 MoveVec
        {
            get { return _moveVec; }
            set
            { 
                _moveVec = value;
                _observable.PushEvent("EVENT_MoveVec", Common.CreateHashTable("moveVec", _moveVec));
            }
        }

        protected Define.Battle.TEAM_TYPE _teamId = 0;
        public Define.Battle.TEAM_TYPE TeamId
        {
            get { return _teamId; }
            set
            {
                _teamId = value;
                _observable.PushEvent("EVENT_TeamId", Common.CreateHashTable("teamId", _teamId));
            }
        }

        protected float _attackPowerBias = 1;
        public float AttackPowerBias
        {
            get { return _attackPowerBias; }
            set
            {
                _attackPowerBias = value;
                _observable.PushEvent("EVENT_AttackPowerBias", Common.CreateHashTable("attackPowerBias", _attackPowerBias));
            }
        }

    }
}