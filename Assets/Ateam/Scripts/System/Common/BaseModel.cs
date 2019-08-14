using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class BaseModel
    {
        protected Observable _observable = new Observable();

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        virtual public void Release()
        {
            if (_observable != null)
            {
                _observable.Release();
                _observable = null;
            }
        }

        //---------------------------------------------------
        // AddListerner
        //---------------------------------------------------
        public void AddListerner(IObserver observer)
        {
            _observable.AddObserver(observer);
        }

        //---------------------------------------------------
        // RemoveListerner
        //---------------------------------------------------
        public void RemoveListerner(IObserver observer)
        {
            _observable.RemoveObserver(observer);
        }
    }
}