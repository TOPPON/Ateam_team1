using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class TitleScene : BaseScene
    {
        [SerializeField]
        Vector3 _cameraRotSpeed = Vector3.zero;

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
            yield return Common.LoadAsync("Prefabs/Title/TitleUiMain", 
                (data) =>
                {
                    _baseUi = Instantiate<GameObject>(data as GameObject).GetComponent<BaseUi>();
                    _baseUi.gameObject.transform.SetParent(_mainCanvas.transform, false);
                });

            while (_baseUi.IsInitialize == false)
            {
                yield return null;
            }

            EndInitilaize();
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            _mainCamera.transform.Rotate(_cameraRotSpeed);
        }
    
        //---------------------------------------------------
        // ChangedSceneStart
        //---------------------------------------------------
        override protected void ChangedSceneStart()
        {

        }

    }
}