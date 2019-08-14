using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public class Observable
    {
        List<IObserver> _observerList = new List<IObserver>();

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        public void Release()
        {
            RemoveAllObserver();
            _observerList = null;
        }

        //---------------------------------------------------
        // AddObserver
        //---------------------------------------------------
        public void AddObserver(IObserver observer)
        {
            _observerList.Add(observer);
        }

        //---------------------------------------------------
        // RemoveObserver
        //---------------------------------------------------
        public void RemoveObserver(IObserver observer)
        {
            _observerList.Remove(observer);
        }

        //---------------------------------------------------
        // RemoveAllObserver
        //---------------------------------------------------
        public void RemoveAllObserver()
        {
            _observerList.Clear();
        }

        //---------------------------------------------------
        // pushEvent
        //---------------------------------------------------
        public void PushEvent(string eventName, Hashtable hashTable)
        {
            int len = _observerList.Count;

            for (int i = 0; i < len; i++)
            {
                _observerList[i].OnNotify(eventName, hashTable);
            }
        }
    }
}