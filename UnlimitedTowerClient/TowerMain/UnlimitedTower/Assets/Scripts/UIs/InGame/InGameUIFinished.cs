using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIFinished : MonoBehaviour {

    public Image clearTitleImage;
    public Image failedTitleImage;

    [SerializeField]
    private AnimationCurve blurPlayAnimationCurve;
    [SerializeField]
    private float blurPlayDuration;
        
    [SerializeField]
    private AnimationCurve clearTitlePlayAnimationCurve;
    [SerializeField]
    private float clearTitlePlayDuration;

    [SerializeField]
    private AnimationCurve failedTitlePlayAnimationCurve;
    [SerializeField]
    private float failedTitlePlayDuration;

    [SerializeField]
    private Material blurMaterial;

    private readonly float _BLUR_MAXIMUM_RANGE = 3.0f;
    private readonly float _DARKNESS_MAXIMUM = 0.9f;
    private float blurPlayedTime = 0.0f;
    private float clearTitlePlayedTime = 0.0f;
    private float failedTitlePlayedTime = 0.0f;
    private Color clearTitleOriginColor = Color.white;
    private Color failedTitleOriginColor = Color.white;
    private Color clearTitleTargetColor = Color.white;
    private Color failedTitleTargetColor = Color.white;

    public enum FinishedType { Clear, Failed, None }
    public FinishedType finishedType { get; set; }

	// Update is called once per frame
	void Update () {

        //Animation about blur / darkness
        {
            blurPlayedTime += Time.deltaTime / Mathf.Max(float.Epsilon, blurPlayDuration);
            float t = blurPlayAnimationCurve.Evaluate(blurPlayedTime);

            blurMaterial.SetFloat("_BlurRange", _BLUR_MAXIMUM_RANGE * t);
            blurMaterial.SetFloat("_Darkness", 1.0f - _DARKNESS_MAXIMUM * t);
        }

        switch(finishedType)
        {
            case FinishedType.Clear: //Animation about clean title
                {
                    clearTitlePlayedTime += Time.deltaTime / Mathf.Max(float.Epsilon, clearTitlePlayDuration);
                    float t = clearTitlePlayAnimationCurve.Evaluate(clearTitlePlayedTime);

                    clearTitleImage.color = Color.Lerp(clearTitleOriginColor, clearTitleTargetColor, t);
                }
                break;
            case FinishedType.Failed:

                //Animation about failed title
                {
                    failedTitlePlayedTime += Time.deltaTime / Mathf.Max(float.Epsilon, failedTitlePlayDuration);
                    float t = failedTitlePlayAnimationCurve.Evaluate(failedTitlePlayedTime);

                    failedTitleImage.color = Color.Lerp(failedTitleOriginColor, failedTitleTargetColor, t);
                }
                break;
            default: break;
        }
    }

    private void OnEnable()
    {
        blurPlayedTime = 0.0f;
        clearTitlePlayedTime = 0.0f;
        failedTitlePlayedTime = 0.0f;

        clearTitleTargetColor = clearTitleImage.color;
        failedTitleTargetColor = failedTitleImage.color;

        clearTitleOriginColor = clearTitleImage.color;
        failedTitleOriginColor = failedTitleImage.color;

        clearTitleTargetColor.a = 0.0f;
        failedTitleTargetColor.a = 0.0f;
    }
}
