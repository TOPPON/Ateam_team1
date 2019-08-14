using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class BattleScene : BaseScene
    {
        [SerializeField]
        BattleSystem _battlesystem = null;

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
            while (ApplicationManager.Instance.IsInitialize == false)
            {
                yield return null;
            }

            ApplicationManager.Instance.CameraManager.MainCamera = _mainCamera;
            EffectPreload();

            yield return Common.LoadAsync("Prefabs/Battle/BattleUiMain", 
                (data) =>
                {
                    _baseUi = Instantiate<GameObject>(data as GameObject).GetComponent<BaseUi>();
                    _baseUi.gameObject.transform.SetParent(_mainCanvas.transform, false);
                });

            while (_baseUi.IsInitialize == false)
            {
                yield return null;
            }

            while (_battlesystem.IsInitialize == false)
            {
                yield return null;
            }

            _battlesystem.BattleModel.AddListerner(_baseUi.GetComponent<BattleUiMain>());
            BattleUiMain battleUiMain = (BattleUiMain) _baseUi;
            battleUiMain.SetTeamName(_battlesystem.TeamNameList);

            EndInitilaize();
        }
    
        //---------------------------------------------------
        // ChangedSceneStart
        //---------------------------------------------------
        override protected void ChangedSceneStart()
        {

        }

        //---------------------------------------------------
        // ChangedSceneStart
        //---------------------------------------------------
        void EffectPreload()
        {
            EffectData list = ApplicationManager.Instance.Master.EffectData;

            for(int i = 0; i < list.GetLength(); i++)
            {
                ApplicationManager.Instance.EffectManager.PreLoad(list.GetData(i).PrefabPath);
            }
                
        }

    }
}