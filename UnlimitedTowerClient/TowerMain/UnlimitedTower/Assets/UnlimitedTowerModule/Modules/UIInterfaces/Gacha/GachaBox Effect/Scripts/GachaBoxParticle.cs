using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class GachaBoxParticle : MonoBehaviour {
    [SerializeField]
    private Color color;

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private float maxRadius;

    [SerializeField]
    private float minRadius;

    [SerializeField]
    private float targetRadius;

    [SerializeField]
    private AnimationCurve alphaDeltaCurve;

    [SerializeField]
    private AnimationCurve speedDeltaGraph;

    [SerializeField]
    private AnimationCurve scaleDeltaGraph;

    private MaterialPropertyBlock materialPropertyBlock;

    private Vector3 preGenDir;

    private float preGenEndDist;
    private float preGenStartDist;
    private float preGenDispersionSpeed;

    private Action UpdateAnimation;

    private float dispersionProgress
    {
        get
        {
            return (transform.localPosition.magnitude - preGenEndDist) / (preGenStartDist - preGenEndDist);
        }
    }

    private Vector3 dispersionVelocity
    {
        get
        {
            return preGenDir * preGenDispersionSpeed;
        }
    }

    private float summonProgress
    {
        get
        {
            return 1.0f - dispersionProgress;
        }
    }

    private float alpha
    {
        get
        {
            return alphaDeltaCurve.Evaluate(summonProgress);
        }
    }

    private Vector3 summonVelocity
    {
        get
        {
            return -preGenDir * maxSpeed * speedDeltaGraph.Evaluate(summonProgress);
        }
    }

    private Vector3 scale
    {
        get
        {
            return Vector3.one * scaleDeltaGraph.Evaluate(summonProgress);
        }
    }

    public bool isFinishedSummonAnimation
    {
        get
        {
            return Mathf.Abs(dispersionProgress) < 0.05f;
        }
    }


    private void Awake()
    {
        //gen pre random val
        preGenEndDist = Random.Range(0, targetRadius);
        preGenStartDist = Random.Range(minRadius, maxRadius);
        preGenDispersionSpeed = maxSpeed * Random.Range(0.1f, 1.0f);

        float rad = Random.Range(0, Mathf.PI * 2.0f);
        preGenDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0.0f);

        //set position
        transform.localPosition = preGenDir * (preGenStartDist);

        //get materialblock
        materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, 0.0f));
        transform.GetComponentInChildren<MeshRenderer>()?.SetPropertyBlock(materialPropertyBlock);

    }

	// Update is called once per frame
	void Update () {
        UpdateAnimation?.Invoke();
    }

    void UpdateSummonAnimation()
    {
        transform.localPosition = transform.localPosition + summonVelocity * Time.deltaTime;
        transform.localScale = scale;
        materialPropertyBlock.SetColor("_TintColor", Color.Lerp(color * 0.0f, color, alpha));
        transform.GetComponentInChildren<MeshRenderer>()?.SetPropertyBlock(materialPropertyBlock);
    }

    void UpdateDispersionAnimation()
    {
        transform.localPosition = transform.localPosition + dispersionVelocity * Time.deltaTime;
        transform.localScale = scale;
        materialPropertyBlock.SetColor("_TintColor", Color.Lerp(color * 0.0f, color, alpha));
        transform.GetComponentInChildren<MeshRenderer>()?.SetPropertyBlock(materialPropertyBlock);
    }

    void Destroyer()
    {
        if (dispersionProgress > 0.99f)
            Destroy(gameObject);
    }

    public void BeginSummon()
    {
        UpdateAnimation = UpdateSummonAnimation;
    }

    public void BeginDispersion()
    {
        UpdateAnimation = UpdateDispersionAnimation;
        UpdateAnimation += Destroyer;
    }

}
