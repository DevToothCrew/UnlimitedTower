using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameNumberUI : MonoBehaviour
{
    private readonly float _COMPLETE_THRESHOLD = 1.00f;

    public AnimationCurve animationAlphaCurve;
    public AnimationCurve animationTransformCurve;

    public float animationDuration = 1.0f;
    public float animationMoveUpAmount = 1.0f;

    // animation times
    private float currAnimationTime = 0.0f;

    // Number Mesh (child) transform information
    private Transform numberMeshTransform;

    // Number Mesh (child) shader information
    private TextMesh textMesh;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        numberMeshTransform = textMesh.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Clamp01(currAnimationTime / animationDuration);

        if (t < _COMPLETE_THRESHOLD)
        {
            float alphaProgress = animationAlphaCurve.Evaluate(t);
            float transformProgress = animationTransformCurve.Evaluate(t);

            // update transfrom
            numberMeshTransform.localPosition = Vector3.up * animationMoveUpAmount * transformProgress;

            // update Shader
            Color currColor = textMesh.color;
            currColor.a = alphaProgress;
            textMesh.color = currColor;

            currAnimationTime += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    #region interfaces

    public void SetValue(int val)
    {
        textMesh.color = Mathf.Sign(val) < 0 ? new Color(1.0f, 0.0f, 0.0f) : new Color(1.0f, 1.0f, 0.0f);
        textMesh.text = val.ToString();
    }
    #endregion
}
