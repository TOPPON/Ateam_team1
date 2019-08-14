using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ateam
{
    public class RankingScene : BaseScene
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
            yield return Common.LoadAsync("Prefabs/Ranking/RankingUiMain", 
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
        }

        //---------------------------------------------------
        // ChangedSceneStart
        //---------------------------------------------------
        override protected void ChangedSceneStart()
        {

        }
    }
}