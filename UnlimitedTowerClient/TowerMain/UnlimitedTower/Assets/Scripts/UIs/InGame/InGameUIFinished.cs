using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIFinished : MonoBehaviour {

    [SerializeField]
    private AnimationCurve blurPlayAnimationCurve;

    [SerializeField]
    private float blurPlayDuration;

    [SerializeField]
    private Material blurMaterial;

    private readonly float _BLUR_MAXIMUM_RANGE = 3.0f;
    private float blurPlayedTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        blurPlayedTime += Time.deltaTime / Mathf.Max(float.Epsilon, blurPlayDuration);
        float t = blurPlayAnimationCurve.Evaluate(blurPlayedTime);

        blurMaterial.SetFloat("_BlurRange", _BLUR_MAXIMUM_RANGE * t);
    }

    private void OnEnable()
    {
        blurPlayedTime = 0.0f;
    }
}
