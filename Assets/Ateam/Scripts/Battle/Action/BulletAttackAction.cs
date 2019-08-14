using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class BulletAttackAction : BaseAction
    {
        [SerializeField]
        string _bulletPath = null;

        [SerializeField]
        int _bulletId = 1;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            base.Initialize();    
        }

        //---------------------------------------------------
        // StartEnter
        //---------------------------------------------------
        override protected void StartEnter(StateData data)
        {
            base.StartEnter(data);

            GameObject go = Resources.Load<GameObject>(_bulletPath);
            Instantiate (go).GetComponent<Bullet>().Initilaize(_bulletId
                , _character.CharacterModel.TeamId
                , _character.transform.position
                , _character.CharacterModel.Direction
                , _character.CharacterModel.AttackPowerBias);
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            base.Release();
        }
    }
}