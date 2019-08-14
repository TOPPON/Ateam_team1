using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class InvincibleAction : BaseAction
    {
        [SerializeField]
        int _invincibleFrameCount = 0;

        [SerializeField]
        GameObject _effectPrefab = null;

        GameObject _effect = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            base.Initialize();  
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void StartEnter(StateData data)
        {
            base.StartEnter(data);

            _effect = Instantiate(_effectPrefab);
            _character.CharacterModel.Invincible = true;
        }

        //---------------------------------------------------
        // UpdateAction
        //---------------------------------------------------
        override protected void UpdateAction(StateData data)
        {   
            base.UpdateAction(data);


            if (data.frameCount < _invincibleFrameCount)
            {
                _effect.transform.position = _character.transform.position;
            }
            else
            {
                _character.CharacterModel.Invincible = false;

                if(_effect != null)
                {
                    Destroy(_effect);
                }
            }
        }

        //---------------------------------------------------
        // EndEnter
        //---------------------------------------------------
        override protected void EndEnter(StateData data)
        {
            base.EndEnter(data);
            _character.CharacterModel.Invincible = false;
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