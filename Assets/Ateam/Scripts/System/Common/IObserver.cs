using UnityEngine;
using System.Collections;

namespace Ateam
{
    public interface IObserver
    {
        void OnNotify(string eventName, Hashtable hashTable);
    }
}