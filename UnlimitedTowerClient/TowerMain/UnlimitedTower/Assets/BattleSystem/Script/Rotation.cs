using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;
    public float speed;
    
    private void FixedUpdate()
    {
        transform.Rotate(rotateX ? speed : 0, rotateY ? speed : 0, rotateZ ? speed : 0);
    }
}
