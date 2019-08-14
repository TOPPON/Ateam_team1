using UnityEngine;
using System.Collections;

//外部アセットのため無視
#pragma warning disable 0618

[ExecuteInEditMode]
public class ScaleParticles : MonoBehaviour {
    void Update () {
        GetComponent<ParticleSystem>().startSize = transform.lossyScale.magnitude;
    }
}