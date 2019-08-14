using UnityEngine;
using System.Collections;

namespace Ateam
{
    public class ParticleBase : MonoBehaviour
    {
        ParticleSystem _system;
        
        //---------------------------------------------------
        // Start
        //---------------------------------------------------
        void Awake()
        {
            _system = GetComponent<ParticleSystem>();
        }
        
        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            if (_system.IsAlive() == false)
            {
                gameObject.SetActive(false);
            }
        }
    }
}