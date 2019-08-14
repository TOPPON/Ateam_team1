using UnityEngine;
using System.Collections;

namespace Ateam
{
    abstract public class BaseCoroutineMonobehaviour : BaseMonoBehaviour
    {
        private bool _isInitialize = false;
        public bool IsInitialize { get { return _isInitialize; } }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            StartCoroutine(InitializeCoroutine());
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {

        }

        //---------------------------------------------------
        // InitializeCoroutine
        //---------------------------------------------------
        abstract protected IEnumerator InitializeCoroutine();

        //---------------------------------------------------
        // InitializeCoroutine
        //---------------------------------------------------
        protected void EndInitilaize()
        {
            _isInitialize = true;
        }
    }
}