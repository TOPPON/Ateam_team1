using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class ApplicationManager : MonoBehaviour 
    {
        private static ApplicationManager _instance = null;
        public static ApplicationManager Instance 
        { 
            get { return _instance; }
        }

        private GameSceneManager _gameSceneManager = null;
        public GameSceneManager GameSceneManager
        {
            get { return _gameSceneManager; }
        }

        private ActorManager _actorManager = null;
        public ActorManager ActorManager
        {
            get { return _actorManager; }
        }

        private Master _master = null;
        public Master Master
        {
            get { return _master; }
        }

        private CameraManager _cameraManager = null;
        public CameraManager CameraManager
        {
            get { return _cameraManager; }
        }

        bool _isInitialize = false;
        public bool IsInitialize
        {
            get { return _isInitialize; }
        }

        BattleSystem _battleSystem;
        public BattleSystem Battlesystem
        {
            get { return _battleSystem; }
            set { _battleSystem = value; }
        }

        EffectManager _effectManager;
        public EffectManager EffectManager
        {
            get { return _effectManager; }
            set { _effectManager = value; }
        }

        GameObject _ncmbSetting;

        //---------------------------------------------------
        // OnLaunchApplication
        //---------------------------------------------------
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLaunchApplication()
        {
            Application.targetFrameRate = Define.Config.FPS;

            if (_instance == null)
            {
                GameObject go = new GameObject("ApplicationManager");
                _instance = go.AddComponent<ApplicationManager>();

                DontDestroyOnLoad(go);
                _instance.Initialize();
            }

        }    

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize()
        {
            StartCoroutine(InitializeCoroutine());
        }

        //---------------------------------------------------
        // InitializeCoroutin
        //---------------------------------------------------
        IEnumerator InitializeCoroutine()
        {    
            if (_gameSceneManager == null)
            {
                _gameSceneManager = gameObject.AddComponent<GameSceneManager>();
                _gameSceneManager.Initialize();
            }

            if (_actorManager == null)
            {
                _actorManager = new ActorManager();
            }

            if (_master == null)
            {
                _master = gameObject.AddComponent<Master>();

                while (_master.IsInitialize == false)
                {
                    yield return null;
                }
            }

            if (_cameraManager == null)
            {
                _cameraManager = gameObject.AddComponent<CameraManager>();
            }

            if (_effectManager == null)
            {
                _effectManager = gameObject.AddComponent<EffectManager>();
            }

            if (_ncmbSetting == null)
            {
                yield return Common.LoadAsync("Prefabs/Settings/NCMBSetting", (obj) =>
                    {
                        _ncmbSetting = GameObject.Instantiate(obj as GameObject) as GameObject; 
                    });
            }

            if (Master.CheckDataAll() == false)
            {
                GameSceneManager.ChangeScene(Define.Scenes.DANGER);
            }

            QualitySettings.SetQualityLevel(0);

            _isInitialize = true;

            yield return null;
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        public void Release()
        {
            if (_gameSceneManager != null)
            {
                _gameSceneManager.Release();
                _gameSceneManager = null;
            }

            if (_actorManager != null)
            {
                _actorManager.Release();
                _actorManager = null;
            }

            if (_master != null)
            {
                _master = null;
            }

            if (_effectManager != null)
            {
                _effectManager = null;
            }
        }
    }
}
