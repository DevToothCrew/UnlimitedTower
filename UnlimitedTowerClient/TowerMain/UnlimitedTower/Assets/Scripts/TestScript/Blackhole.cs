using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour {

   private RectTransform rectTransform;
    private Vector3 rotationAxis = new Vector3(0.0f, 0.0f, 1.0f);
    public float RotationSpeed = 800.0f;
	// Use this for initialization
	void Start () {
        rectTransform = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        //rectTransform.Rotate(new Vector3(0.0f, 0.0f, Time.deltaTime*800.0f));
        rectTransform.Rotate(rotationAxis, Time.deltaTime * RotationSpeed);

    }
}
