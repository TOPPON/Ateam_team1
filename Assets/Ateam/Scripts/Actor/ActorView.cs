using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class ActorView : BaseMonoBehaviour, IObserver
    {
        GameObject _avatar = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
    
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
    
        }

        //---------------------------------------------------
        // SetAvatar
        //---------------------------------------------------
        public void SetAvatar(string path)
        {
            if (path != null)
            {
                //今回はavatarの必要性がないのでGameObject
                StartCoroutine(LoadAvatar(path));
            }
        }

        //---------------------------------------------------
        // LoadAvatar
        //---------------------------------------------------
        IEnumerator LoadAvatar(string path)
        {
            yield return Common.LoadAsync(path, (obj)=>{

                _avatar = Instantiate(obj as GameObject);
                _avatar.transform.SetParent(gameObject.transform, false);
            });   
        }

        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        virtual public void OnNotify(string eventName, Hashtable hashTable)
        {

        }
    }
}