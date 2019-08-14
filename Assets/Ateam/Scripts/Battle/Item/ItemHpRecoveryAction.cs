using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class ItemHpRecoveryAction : BaseAction
    {
        [SerializeField]
        int _masterId = 0;

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

            if (_character.CharacterModel.Hp + ApplicationManager.Instance.Master.ItemData.GetData(_masterId).Value > _character.CharacterModel.MaxHp)
            {
                _character.CharacterModel.Hp = _character.CharacterModel.MaxHp;
            }
            else
            {
                _character.CharacterModel.Hp += ApplicationManager.Instance.Master.ItemData.GetData(_masterId).Value;
            }
        }

        //---------------------------------------------------
        // EndEnter
        //---------------------------------------------------
        protected override void EndEnter(StateData data)
        {
            base.EndEnter(data);
        }
    }
}