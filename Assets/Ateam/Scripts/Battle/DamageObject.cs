using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class DamageObject : BaseMonoBehaviour
    {
        [SerializeField]
        BoxCollider _boxCollider            = null;

        bool _isInitilaize                  = false;
        ActorManager _actorManager          = null;
        float _attackPower                    = 0;
        int _endFrameCount                  = 0;
        Define.Battle.TEAM_TYPE _teamType;

        public delegate void AttackHit(Actor hitActor, float attackPower);
        public AttackHit AttackHitDelegate
        {
            get;
            set;
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
    
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize(Define.Battle.TEAM_TYPE teamType, Vector3 pos , Vector3 range,int attackFrame, float attackPower)
        {
            _teamType                       = teamType;
            _boxCollider.size               = range;
            _actorManager                   = ApplicationManager.Instance.ActorManager;
            _attackPower                    = attackPower;
            _endFrameCount                  = attackFrame;
            gameObject.transform.position   = pos;

            _isInitilaize = true;
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        protected override void Release()
        {
    
        }
    
        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            if (_endFrameCount <= 0)
            {
                EndAttack();
            }

            _endFrameCount--;
        }

        //---------------------------------------------------
        // OnTriggerEnter
        //---------------------------------------------------
        public void OnTriggerEnter(Collider collider)
        {
            if (_isInitilaize && _endFrameCount > 0)
            {
                return;
            }

            Attack(collider);
        }

        //---------------------------------------------------
        // OnTriggerStay 
        //---------------------------------------------------
        public void OnTriggerStay(Collider collider)
        {
            if (_isInitilaize && _endFrameCount > 0)
            {
                return;
            }

            Attack(collider);
        }

        //---------------------------------------------------
        // OnTriggerExit
        //---------------------------------------------------
        public void OnTriggerExit(Collider collider)
        {
            if (_isInitilaize && _endFrameCount > 0)
            {
                return;
            }

            Attack(collider);
        }

        //---------------------------------------------------
        // Attack
        //---------------------------------------------------
        void Attack(Collider collider)
        {
            ActorData? hitData      = _actorManager.GetActor(collider.gameObject.GetComponent<Actor>().ActorModel.ActorId);

            if (hitData != null 
                && hitData.Value.Type == Define.ActorType.CHARACTER)
            {
                if (collider.gameObject.GetComponent<Character>().CharacterModel.TeamId
                    != _teamType)
                {
                    AttackHitDelegate(hitData.Value.Actor, _attackPower);
                }
            }
        }

        //---------------------------------------------------
        // EndAttack
        //---------------------------------------------------
        void EndAttack()
        {
            Destroy(gameObject);
        }
    }
}