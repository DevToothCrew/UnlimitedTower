﻿using System;
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


    public Text uiComment;
    public Image uiInside;

    private long lastId = 0;
    private long currId = 0;

    private float uiInsideFullWidth;
    private float uiInsideNextWidth;
    private float uiInsideCurrWidth;

    private Queue<Description> wrkQ = new Queue<Description>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            StartCoroutine(wrkFlow());
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private IEnumerator wrkFake1()
    {
        yield return new WaitForSeconds(1.0f);
        SetProgress(0.1f, "Step1");
        yield return new WaitForSeconds(1.0f);
        SetProgress(0.2f, "Step2");
        yield return new WaitForSeconds(1.0f);
        SetProgress(0.3f, "Step3");
        yield return new WaitForSeconds(1.0f);
        SetProgress(0.4f, "Step4");
        yield return new WaitForSeconds(1.0f);
        SetProgress(0.5f, "Step5");
    }
    private IEnumerator wrkFake2()
    {
        yield return new WaitForSeconds(2.0f);
        SetProgress(0.2f, "Step1");
        yield return new WaitForSeconds(2.0f);
        SetProgress(0.4f, "Step2");
        yield return new WaitForSeconds(2.0f);
        SetProgress(0.6f, "Step3");
        yield return new WaitForSeconds(2.0f);
        SetProgress(0.8f, "Step4");
        yield return new WaitForSeconds(2.0f);
        SetProgress(0.9f, "Step5");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BeginScene(wrkFake1(), () => { Debug.Log("WRK FAKE 1 IS DONE!"); });
        if (Input.GetKeyDown(KeyCode.Alpha2)) BeginScene(wrkFake2(), () => { Debug.Log("WRK FAKE 2 IS DONE!"); });

        // scale update
        uiInsideCurrWidth = Mathf.Lerp(uiInsideCurrWidth, uiInsideNextWidth, 0.1f);
        uiInside?.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiInsideCurrWidth);
    }

    private IEnumerator wrkFlow()
    {
        uiInsideFullWidth = uiInside.rectTransform.rect.width;

        while (this != null)
        {
            if (wrkQ.Count > 0)
            {
                var desc = wrkQ.Dequeue();

                gameObject.SetActivateWithAnimation(true);

                uiInsideCurrWidth = 0.0f;
                SetProgress(0.0f, desc.startComment);

                if (desc.OnProgressAsync == null) {
                    yield return new WaitUntil(() => desc.predicate());
                }
                else
                {
                    yield return desc.OnProgressAsync;
                }

                SetProgress(1.0f, desc.finishedComment);
                yield return new WaitForSeconds(1.0f);

                desc.OnSuccess?.Invoke();
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
        uiInsideNextWidth = uiInsideFullWidth * t;
        uiComment.text = cmnt;
    }

}
