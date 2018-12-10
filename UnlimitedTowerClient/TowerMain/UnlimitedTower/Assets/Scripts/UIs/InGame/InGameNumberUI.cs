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
    private MeshRenderer rdrr;

    private MaterialPropertyBlock mtrlBlock = null;

    private void Awake()
    {
        rdrr = GetComponentInChildren<MeshRenderer>();
        numberMeshTransform = rdrr.transform;

        mtrlBlock = new MaterialPropertyBlock();
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
            mtrlBlock.SetFloat("_Alpha", alphaProgress);
            rdrr.SetPropertyBlock(mtrlBlock);

            currAnimationTime += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    #region interfaces

    public void SetTexture(Texture2D tex)
    {
        if (rdrr != null)
        {
            rdrr.material.mainTexture = tex;
        }
    }
    #endregion
}
