using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class GachaBoxParticleController : MonoBehaviour
{

    public GameObject particlePrefab;

    //pre saved transforms
    private Vector3 orinCubePosition;
    private Quaternion orinCubeRotation;
    private Vector3 orinCubeScale;

    //post effect
    private PostProcessingBehaviour ppb;
    private BloomModel.Settings bloomSettings;

    //object
    public GameObject Cube;

    public float maxDelayTime;
    public int particleCount;

    public float maxPlayTime;
    public List<GameObject> particles = new List<GameObject>();

    private Action callback = null;

    public GameObject gachaList;
    public GameObject skipButton;
    public GameObject explainEOS;
    public GameObject explainUTG;
    public bool isGachaStart = false;

    private void Awake()
    {
        ppb = GetComponentInChildren<PostProcessingBehaviour>();

        bloomSettings = ppb.profile.bloom.settings;

        orinCubePosition = Cube.transform.localPosition;
        orinCubeRotation = Cube.transform.localRotation;
        orinCubeScale = Cube.transform.localScale;
    }

    #region Summon 

    IEnumerator SummonTranslation()
    {
        float timer = maxPlayTime;

        Quaternion nextRotation = Quaternion.Euler(20.0f, 0, 0) * Cube.transform.rotation;
        Vector3 nextPosition = Vector3.down * 0.1f;
        Vector3 nextScale = Vector3.one * 0.8f;

        while ((timer -= Time.unscaledDeltaTime) > 0)
        {
            Cube.transform.rotation = Quaternion.Lerp(Cube.transform.rotation, nextRotation, Time.unscaledDeltaTime * 0.2f);
            Cube.transform.localScale = Vector3.Lerp(Cube.transform.localScale, nextScale, Time.unscaledDeltaTime * 0.2f);
            Cube.transform.localPosition = Vector3.Lerp(Cube.transform.localPosition, nextPosition, Time.unscaledDeltaTime * 0.2f);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator SummonBlooming()
    {
        float timer = maxPlayTime;
        float nextIntensity = 0.0f;

        while ((timer -= Time.unscaledDeltaTime) > 0)
        {
            if (Mathf.Abs(bloomSettings.bloom.intensity - nextIntensity) < 0.05f)
            {
                nextIntensity = Mathf.Max(0.0f, bloomSettings.bloom.intensity + UnityEngine.Random.Range(-0.8f, 1.2f));
            }

            bloomSettings.bloom.intensity = Mathf.Lerp(bloomSettings.bloom.intensity, nextIntensity, Time.unscaledDeltaTime * 16.0f);

            //set dirty
            ppb.profile.bloom.settings = bloomSettings;
            yield return new WaitForEndOfFrame();
        }
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
                var particle = go?.GetComponent<GachaBoxParticle>();
                if (particle != null)
                {
                    allParticleReady = allParticleReady && particle.isFinishedSummonAnimation;
                }
            }
        }

        StopAllCoroutines();
        BeginDispersionAnimation();
    }

    #endregion

    #region Dispersion
    IEnumerator BoxUpAnimation()
    {
        while (true)
        {
            Cube.transform.localRotation = Quaternion.Lerp(Cube.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, 45.0f), Time.unscaledDeltaTime * 8.0f);
            Cube.transform.localScale = Vector3.Lerp(Cube.transform.localScale, Vector3.one * 2.0f, Time.unscaledDeltaTime * 8.0f);

            bloomSettings.bloom.intensity = Mathf.Lerp(bloomSettings.bloom.intensity, 2.5f, Time.unscaledDeltaTime * 8.0f);
            //set dirty
            ppb.profile.bloom.settings = bloomSettings;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    IEnumerator GenerateParticle(float time)
    {
        var go = Instantiate(particlePrefab, transform);
        particles.Add(go);
        yield return new WaitForSecondsRealtime(time);

        go.GetComponent<GachaBoxParticle>()?.BeginSummon();
    }

    IEnumerator ParticleWatcher()
    {
        FinishAnimation();
        while (particles.Count > particleCount / 10)
        {
            particles.RemoveAll(go => go == null);
            yield return new WaitForEndOfFrame();
        }
    }


    public void BeginSummonAnimation(System.Action callback)
    {
        gachaList.SetActive(false);
        explainEOS.SetActive(false);
        explainUTG.SetActive(false);
        skipButton.SetActive(true);
        isGachaStart = true;

        StopAllCoroutines();
        particles?.Clear();

        Cube.transform.localPosition = orinCubePosition;
        Cube.transform.localRotation = orinCubeRotation ;
        Cube.transform.localScale = orinCubeScale;

        for (int i = 0; i < particleCount; i++)
        {
            StartCoroutine(GenerateParticle(UnityEngine.Random.Range(0, maxDelayTime)));
        }
        
        StartCoroutine(SummonBlooming());
        StartCoroutine(SummonTranslation());
        StartCoroutine(ConditionChecker());

        this.callback += callback;
    }

    public void BeginDispersionAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(BoxUpAnimation());
        StartCoroutine(ParticleWatcher());

        particles.ForEach(go => go?.GetComponent<GachaBoxParticle>()?.BeginDispersion());
    }

    public void FinishAnimation()
    {
        gachaList.SetActive(true);
        explainEOS.SetActive(true);
        explainUTG.SetActive(true);
        skipButton.SetActive(false);
        isGachaStart = false;
        StopAllCoroutines();
        callback?.Invoke();
    }

    private void OnDestroy()
    {
        bloomSettings.bloom.intensity = 0.0f;
        ppb.profile.bloom.settings = bloomSettings;
    }
}
