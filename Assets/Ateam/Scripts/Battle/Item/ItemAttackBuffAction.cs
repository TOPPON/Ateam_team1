using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class ItemAttackBuffAction : BaseAction
    {
        [SerializeField]
        int _masterId = 0;

        float bias = 0;

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
        override protected void Release()
        {
            base.Release();
        }
    
        //---------------------------------------------------
        // StartEnter
        //---------------------------------------------------
        override protected void StartEnter(StateData data)
        {
            base.StartEnter(data);

            bias = _character.CharacterModel.AttackPowerBias * ApplicationManager.Instance.Master.ItemData.GetData(_masterId).Value;;
        }

        //---------------------------------------------------
        // UpdateAction
        //---------------------------------------------------
        override protected void UpdateAction(StateData data)
        {   
            base.UpdateAction(data);

            _character.CharacterModel.AttackPowerBias = bias;
        }

        //---------------------------------------------------
        // EndEnter
        //---------------------------------------------------
        protected override void EndEnter(StateData data)
        {
            base.EndEnter(data);

            _character.CharacterModel.AttackPowerBias = _character.CharacterModel.Basedata.AttackPowerBias;
        }
    }
}