using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class BootScene : BaseScene
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
        // Update
        //---------------------------------------------------
        override protected void ChangedSceneStart()
        {

        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            if (ApplicationManager.Instance.IsInitialize)
            {
                ApplicationManager.Instance.GameSceneManager.ChangeScene(Define.Scenes.Title);
            }
        }

    }
}