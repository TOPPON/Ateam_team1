using UnityEngine;
using System.Collections;

namespace Ateam
{
    public abstract class BaseMonoBehaviour : MonoBehaviour
    {
        //---------------------------------------------------
        // Start
        //---------------------------------------------------
        void Awake()
        {
            Initialize();
        }
        
        //---------------------------------------------------
        // OnDestroy
        //---------------------------------------------------
        void OnDestroy()
        {
            Release();
        }
    
        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        abstract protected void Initialize();

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        abstract protected void Release();
    }
}