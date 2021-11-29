using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//카메라 타겟
public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        transform.position = target.position + offset;
    }
}
