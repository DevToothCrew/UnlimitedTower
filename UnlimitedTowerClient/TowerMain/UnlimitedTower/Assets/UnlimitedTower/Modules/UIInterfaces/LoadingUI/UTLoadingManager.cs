using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UTLoadingManager : MonoBehaviour {

    static public UTLoadingManager Instance = null;

    public class Description
    {
        public long id = 0;

        public string startComment = "Loading...";
        public string finishedComment = "Done ..!";

        public IEnumerator OnProgressAsync = null;
        public Action OnSuccess = null;

        public Func<bool> predicate = null;

        public bool isLoading = true;
    }

    public float gageRadius = 110f;
    public float gageAlpha = 1.0f;

    public Text uiComment;
    public Image uiInside;
    public UTLoadingParticle uiParticles;
    public RectTransform uiGageFx;

    private long lastId = 0;
    private long currId = 0;

    private float uiInsideFullGage;
    private float uiInsideNextGage;
    private float uiInsideCurrGage;

    private Queue<Description> wrkQ = new Queue<Description>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            StartCoroutine(wrkFlow());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        // scale update
        uiInsideCurrGage = Mathf.Lerp(uiInsideCurrGage, uiInsideNextGage, 0.1f);
        uiInside.material.SetFloat("uProgress", uiInsideCurrGage);
        uiInside.material.SetFloat("uAlpha", gageAlpha);

        float rad = Mathf.PI * 2.0f * uiInsideCurrGage;

        uiGageFx.anchoredPosition =
            Vector2.left * gageRadius * Mathf.Sin(rad) +
            Vector2.up * gageRadius * Mathf.Cos(rad);
    }

    private IEnumerator wrkFlow()
    {
        uiInsideFullGage = 1.0f;

        while (this != null)
        {
            if (wrkQ.Count > 0)
            {
                var desc = wrkQ.Dequeue();

                gameObject.SetActivateWithAnimation(true);
                uiParticles.Begin();

                uiInsideCurrGage = 0.0f;
                SetProgress(0.0f, desc.startComment);

                if (desc.OnProgressAsync == null) {
                    yield return new WaitUntil(() => desc.predicate());
                }
                else
                {
                    yield return desc.OnProgressAsync;
                }

                SetProgress(1.0f, desc.finishedComment);
                yield return new WaitForSecondsRealtime(1.0f);

                desc.OnSuccess?.Invoke();

                uiParticles.End();
                gameObject.SetActivateWithAnimation(false);
            }
            else
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="maximumWaitTime"> </param>
    public long BeginScene(Description desc)
    {
        desc.id += lastId;
        wrkQ.Enqueue(desc);
        return desc.id;
    }

    public long BeginScene(IEnumerator OnProgressAsync = null, Action OnSuccess = null)
    {
        Description desc = new Description
        {
            OnProgressAsync = OnProgressAsync,
            OnSuccess = OnSuccess,
        };

        return BeginScene(desc);
    }

    public long BeginScene(Func<bool> predicate)
    {
        Description desc = new Description
        {
            predicate = predicate,
        };

        return BeginScene(desc);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="cmnt"></param>
    public void SetProgress(float t, string cmnt = "Loading ...")
    {
        uiInsideNextGage = uiInsideFullGage * t;
        uiComment.text = cmnt;
    }

}
