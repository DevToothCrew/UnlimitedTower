using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class GachaBoxParticle : MonoBehaviour {
    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private float maxRadius;

    [SerializeField]
    private float minRadius;

    [SerializeField]
    private float targetRadius;

    [SerializeField]
    private AnimationCurve speedDeltaGraph;

    [SerializeField]
    private AnimationCurve scaleDeltaGraph;

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

    }

	// Update is called once per frame
	void Update () {
        UpdateAnimation?.Invoke();
    }

    void UpdateSummonAnimation()
    {
        transform.localPosition = transform.localPosition + summonVelocity * Time.deltaTime;
        transform.localScale = scale;
    }

    void UpdateDispersionAnimation()
    {
        transform.localPosition = transform.localPosition + dispersionVelocity * Time.deltaTime;
        transform.localScale = scale;
    }

    public void BeginSummon()
    {
        UpdateAnimation = UpdateSummonAnimation;
    }

    public void BeginDispersion()
    {
        UpdateAnimation = UpdateDispersionAnimation;
    }

}
