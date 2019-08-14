using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class TitleUiMain : BaseUi
    {
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
        // InitializeCoroutine
        //---------------------------------------------------
        override protected IEnumerator InitializeCoroutine()
        {
            yield return null;

            EndInitilaize();
        }

        //---------------------------------------------------
        // OnClickedStartButton
        //---------------------------------------------------
        public void OnClickedStartButton()
        {
            ApplicationManager.Instance.GameSceneManager.ChangeScene(Define.Scenes.BATTLE);
        }
    }
}