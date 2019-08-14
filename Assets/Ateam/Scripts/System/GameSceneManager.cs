using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Ateam
{
    public class GameSceneManager : MonoBehaviour
    {
        string _currentSceneName                    = "";
        Coroutine _changeSceneCoroutineInstance     = null;

        BaseScene _currentScene                     = null;
        public BaseScene CurrentScene
        {
            get { return _currentScene; }
            set { _currentScene = value; }
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            _currentSceneName = SceneManager.GetActiveScene().name;
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        public void Release()
        {
            _currentSceneName = null;
        }
    
        //---------------------------------------------------
        // ChangeScene
        //---------------------------------------------------
        public void ChangeScene(string sceneName)
        {
            if (sceneName == _currentSceneName
                || _changeSceneCoroutineInstance != null)
            {
                return;
            }

            _currentSceneName               = sceneName;
            _changeSceneCoroutineInstance   = StartCoroutine(ChangeSceneCoroutine(sceneName));
        }

        //---------------------------------------------------
        // ChangeSceneColoutine
        //---------------------------------------------------
        IEnumerator ChangeSceneCoroutine(string sceneName)
        {
            ApplicationManager.Instance.EffectManager.RemoveAll();
            ApplicationManager.Instance.ActorManager.RemoveAll();

            yield return SceneManager.LoadSceneAsync(sceneName);

            while (CurrentScene == null)
            {
                yield return null;
            }

            while(CurrentScene.IsInitialize == false)
            {
                yield return null;
            }

            yield return Resources.UnloadUnusedAssets();


            _changeSceneCoroutineInstance = null;
            Debug.Log("CHANGE SCENE :" + sceneName);
        }
    }
}