using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public class EffectManager : MonoBehaviour
    {
        struct EffectData
        {
            public EffectData(string path)
            {
                this._cacheList     = new List<GameObject>();
                _prefabPath         = path;
                _originalPrefab     = null;
            }

            public List<GameObject> _cacheList;
            public string _prefabPath;
            public GameObject _originalPrefab;
        }

        Dictionary<string, EffectData> _effectList = new Dictionary<string, EffectData>();

        public delegate void playeDelegate(GameObject obj);

        //---------------------------------------------------
        // Play
        //---------------------------------------------------
        public void Play(string prefabPath, Vector3 pos, Vector3 rot, playeDelegate callback = null)
        {
            if (_effectList.ContainsKey(prefabPath) == false)
            {
                StartCoroutine(LoadCoroutine(prefabPath, pos, rot, callback));
            }
            else
            {
                bool isCacheUse     = false;
                EffectData data     = _effectList[prefabPath];

                for(int i = 0; i < data._cacheList.Count; i++)
                {
                    GameObject go = data._cacheList[i];
                    if (go.activeSelf == false)
                    {
                        isCacheUse = true;
                        go.transform.position = pos;

                        go.transform.Rotate(rot);
                        go.SetActive(true);

                        if (callback != null)
                        {
                            callback(go);
                        }

                        break;
                    }
                }

                if (isCacheUse == false)
                {
                    GameObject go = Instantiate(data._originalPrefab);
                    go.transform.position = pos;
                    go.transform.Rotate(rot);
                    data._cacheList.Add(go);

                    if (callback != null)
                    {
                        callback(go);
                    }
                }
            }
        }

        //---------------------------------------------------
        // LoadCoroutine
        //---------------------------------------------------
        IEnumerator LoadCoroutine(string path, Vector3 pos, Vector3 rot, playeDelegate callback)
        {
            yield return Common.LoadAsync(path, (obj) =>
                {
                    EffectData data         = new EffectData(path);
                    data._originalPrefab    = obj as GameObject;
                    GameObject go           = GameObject.Instantiate(data._originalPrefab);

                    data._cacheList.Add(go);
                    go.transform.position   = pos;
                    go.transform.Rotate(rot);

                    _effectList.Add(path, data);

                    if (callback != null)
                    {
                        callback(go);
                    }
                });
        }

        //---------------------------------------------------
        // PreLoad
        //---------------------------------------------------
        public void PreLoad(string prefabPath)
        {
            if (_effectList.ContainsKey(prefabPath))
            {
                return;
            }

            StartCoroutine(LoadCoroutine(prefabPath, Vector3.zero, Vector3.zero, (obj)=>{

                obj.SetActive(false);
            }));
        }

        //---------------------------------------------------
        // RemoveAll
        //---------------------------------------------------
        public void RemoveAll()
        {
            foreach (var data in _effectList)
            {
                for (int i = 0; i < data.Value._cacheList.Count; i++)
                {
                    Destroy(data.Value._cacheList[i]);
                }

                data.Value._cacheList.Clear();
            }
        }
     
    }
}