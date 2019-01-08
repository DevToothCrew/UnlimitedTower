using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballRotate : MonoBehaviour {
	void FixedUpdate () {
        transform.Rotate(2, 0, 0);
	}
}
