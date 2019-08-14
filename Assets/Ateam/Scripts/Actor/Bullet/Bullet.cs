using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class Bullet : Actor
    {
        BulletModel _bulletModel;
        public BulletModel BulletModel
        {
            get { return _bulletModel; }
        }

        BulletView _bulletView      = null;
        BattleSystem _battleSystem  = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            base.Initialize();

            _bulletModel = new BulletModel ();
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            base.Release();

            if (_bulletModel != null) 
            {
                _bulletModel.Release ();
                _bulletModel = null;
            }
        }

        //---------------------------------------------------
        // Initilaize
        //---------------------------------------------------
        public void Initilaize(int id, Define.Battle.TEAM_TYPE teamId, Vector3 pos, Vector3 moveVec, float damageBias)
        {
            BulletData.BulletDataList data = ApplicationManager.Instance.Master.BulletData.GetData (id);

            _bulletModel.Speed             = data.Speed;
            _bulletModel.LifeFrameCount = data.LifeFrameCount;
            _bulletModel.RangeBlock     = data.RangeBlock;
            _bulletModel.AttackPower     = data.AttackPower;
            _bulletModel.DamageType     = data.DamageType;
            _bulletModel.MoveVec        = moveVec;
            transform.position          = pos;
            _bulletModel.TeamId         = teamId;
            _bulletModel.AttackPowerBias = damageBias;
            _battleSystem               = ApplicationManager.Instance.Battlesystem;

            GameObject view = new GameObject("BulletView");
            view.transform.SetParent(gameObject.transform, false);
            _bulletView = view.AddComponent<BulletView>();
            _bulletView.Initialize(data);

            if (data.ViewPrefabPath != "")
            {
                _bulletView.SetAvatar(data.ViewPrefabPath);
            }

            base.Initialize(Define.ActorType.BULLET, _bulletModel, _bulletView);
        }
    
        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {   
            Vector2 blockData                   = _battleSystem.StageManager.getPositionBlock(transform.position);
            Define.Stage.BLOCK_TYPE blockType   = _battleSystem.StageManager.GetBlockType(blockData);            
            transform.position                  += _bulletModel.MoveVec * _bulletModel.Speed;

            if (_bulletModel.MoveVec != Vector3.zero)
            {
                transform.forward = _bulletModel.MoveVec;
            }

            _bulletModel.CurrentLifeFrameCount++;

            if (blockType != (int)Define.Stage.BLOCK_TYPE.NORMAL ||
                _bulletModel.CurrentLifeFrameCount >= _bulletModel.LifeFrameCount)
            {
                if(_bulletModel.DamageType == BulletModel.DAMAGE_TYPE.END_LIFE)
                {
                    CreateDamage();
                }

                Destroy(gameObject);
            }
        }

        //---------------------------------------------------
        // OnTriggerEnter
        //---------------------------------------------------
        public void OnTriggerEnter(Collider collider)
        {
            Character character = collider.gameObject.GetComponent<Character>();
            if (character == null)
            {
                return;
            }

            if(_bulletModel.DamageType == BulletModel.DAMAGE_TYPE.HIT 
                && character.CharacterModel.TeamId != _bulletModel.TeamId)
            {
                CreateDamage();
                Destroy(gameObject);
                this.enabled = false;
            }
        }

        //---------------------------------------------------
        // CreateDamage
        //---------------------------------------------------
        void CreateDamage()
        {
            Vector3 range;
            range.x = Define.Battle.BLOCK_SIZE * _bulletModel.RangeBlock.x;
            range.y = Define.Battle.BLOCK_SIZE;
            range.z = Define.Battle.BLOCK_SIZE * _bulletModel.RangeBlock.y;
            
            _battleSystem.CreateDamageObject(_bulletModel.TeamId, transform.position, range, 1, _bulletModel.AttackPower * _bulletModel.AttackPowerBias);
        }
    }
}