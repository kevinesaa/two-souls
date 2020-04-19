using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform Target { get; set; }
    public Vector3 DistanceFromTarget;
    public static CameraController INSTANCE { get;private set; }
    
    private void Awake()
    {
        if (INSTANCE == null) 
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(this);
        }
    }


    private void LateUpdate()
    {
        if (Target != null && DistanceFromTarget != null)
        {
            transform.position = Target.position - DistanceFromTarget;
        }
    }
}
