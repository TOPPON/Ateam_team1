using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class ItemSpeedBuffAction : BaseAction
    {
        [SerializeField]
        int _masterId = 0;

        float speed = 0;

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

            speed = _character.CharacterModel.Speed * ApplicationManager.Instance.Master.ItemData.GetData(_masterId).Value;
            _character.CharacterModel.Speed = speed;
        }

        //---------------------------------------------------
        // UpdateAction
        //---------------------------------------------------
        override protected void UpdateAction(StateData data)
        {   
            base.UpdateAction(data);

            _character.CharacterModel.Speed = speed;
        }

        //---------------------------------------------------
        // EndEnter
        //---------------------------------------------------
        protected override void EndEnter(StateData data)
        {
            base.EndEnter(data);

            _character.CharacterModel.Speed = _character.CharacterModel.Basedata.MoveSpeed;
        }
    }
}