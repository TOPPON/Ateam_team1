using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void Update ()
    {
        Vector3 target = Camera.main.transform.position;
        target.y = transform.position.y;
        transform.LookAt(target);
    }
}