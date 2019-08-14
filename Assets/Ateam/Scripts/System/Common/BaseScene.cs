using UnityEngine;
using System.Collections;

namespace Ateam
{
    abstract public class BaseScene : BaseCoroutineMonobehaviour
    {
        [SerializeField]
        protected Canvas _mainCanvas;

        [SerializeField]
        protected Camera _mainCamera = null;

        protected BaseUi _baseUi = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            ApplicationManager.Instance.GameSceneManager.CurrentScene = this;
            StartCoroutine(InitializeCoroutine());
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            base.Release();
        }
            
        //---------------------------------------------------
        // ChangedSceneStart
        //---------------------------------------------------
        virtual protected void ChangedSceneStart()
        {
            
        }
    }
}