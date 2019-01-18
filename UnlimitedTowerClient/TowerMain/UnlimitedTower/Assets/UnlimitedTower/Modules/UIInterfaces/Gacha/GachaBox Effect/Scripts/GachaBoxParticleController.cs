using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class GachaBoxParticleController : MonoBehaviour
{

    public GameObject particlePrefab;


    //post effect
    private PostProcessingBehaviour ppb;
    private BloomModel.Settings bloomSettings;

    //object
    public GameObject Cube;

    public float maxDelayTime;
    public int particleCount;

    public float maxPlayTime;
    public List<GameObject> particles = new List<GameObject>();

    private void Awake()
    {
        ppb = GetComponentInChildren<PostProcessingBehaviour>();

        bloomSettings = ppb.profile.bloom.settings;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BeginSummonAnimation();
	}

    IEnumerator Translation()
    {
        float timer = maxPlayTime;

        Quaternion nextRotation = Quaternion.Euler(20.0f, 0, 0) * Cube.transform.rotation;
        Vector3 nextPosition = Vector3.down * 0.1f;
        Vector3 nextScale = Vector3.one * 0.8f;

        while ((timer -= Time.deltaTime) > 0)
        {
            Cube.transform.rotation = Quaternion.Lerp(Cube.transform.rotation, nextRotation, Time.deltaTime * 0.2f);
            Cube.transform.localScale = Vector3.Lerp(Cube.transform.localScale, nextScale, Time.deltaTime * 0.2f);
            Cube.transform.localPosition = Vector3.Lerp(Cube.transform.localPosition, nextPosition, Time.deltaTime * 0.2f);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Blooming()
    {
        float timer = maxPlayTime;
        float nextIntensity = 0.0f;

        while ((timer -= Time.deltaTime) > 0)
        {
            if (Mathf.Abs(bloomSettings.bloom.intensity - nextIntensity) < 0.05f)
            {
                nextIntensity = Mathf.Max(0.0f, bloomSettings.bloom.intensity + Random.Range(-0.8f, 1.2f));
            }

            bloomSettings.bloom.intensity = Mathf.Lerp(bloomSettings.bloom.intensity, nextIntensity, Time.deltaTime * 16.0f);

            //set dirty
            ppb.profile.bloom.settings = bloomSettings;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator GenerateParticle(float time)
    {
        var go = Instantiate(particlePrefab, transform);
        particles.Add(go);
        yield return new WaitForSeconds(time);

        go.GetComponent<GachaBoxParticle>()?.BeginSummon();
    }

    IEnumerator ConditionChecker()
    {
        bool allParticleReady = false;

        while (!allParticleReady)
        {
            yield return new WaitForEndOfFrame();

            allParticleReady = true;
            foreach (var go in particles)
            {
                var particle = go.GetComponent<GachaBoxParticle>();
                if (particle != null)
                {
                    allParticleReady = allParticleReady && particle.isFinishedSummonAnimation;
                }
            }
        }

        StopAllCoroutines();
        BeginDispersionAnimation();
    }

    public void BeginSummonAnimation()
    {
        for (int i = 0; i < particleCount; i++)
        {
            var randTime = Random.Range(0, maxDelayTime);
            StartCoroutine(GenerateParticle(randTime));
        }

        StartCoroutine(Blooming());
        StartCoroutine(Translation());
        StartCoroutine(ConditionChecker());
    }

    public void BeginDispersionAnimation()
    {
        foreach(var go in particles)
        {
            var particle = go.GetComponent<GachaBoxParticle>();
            if (particle != null)
            {
                particle.BeginDispersion();
            }
        }
    }

    private void OnDestroy()
    {
        bloomSettings.bloom.intensity = 0.0f;
        ppb.profile.bloom.settings = bloomSettings;
    }
}
