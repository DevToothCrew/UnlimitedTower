using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIFinished : MonoBehaviour {

    public GameObject clearUIGO;
    public GameObject failedUIGO;

    [SerializeField]
    private AnimationCurve blurPlayAnimationCurve;
    [SerializeField]
    private float blurPlayDuration;
    private float blurPlayedTime = 0.0f;

    [SerializeField]
    private AnimationCurve UIPlayAnimationCurve;
    [SerializeField]
    private float UIPlayDuration;
    private float UIPlayedTime = 0.0f;

    [SerializeField]
    private Material blurMaterial;

    private readonly float _BLUR_MAXIMUM_RANGE = 3.0f;
    private readonly float _DARKNESS_MAXIMUM = 0.9f;

    public enum FinishedType { Clear, Failed, None }
    public FinishedType finishedType { get; set; }

    static private InGameUIFinished instance = null;

    private void Awake()
    {
        instance = instance ?? this;
        instance.gameObject.SetActive(false);

        BattleManager.Inst.onBattleClear.AddListener(() => SetupAndPlay(FinishedType.Clear));
        BattleManager.Inst.onBattleFailed.AddListener(() => SetupAndPlay(FinishedType.Failed));
    }

    // Update is called once per frame
    void Update () {

        //Animation about blur / darkness
        {
            blurPlayedTime += Time.deltaTime / Mathf.Max(float.Epsilon, blurPlayDuration);
            float t = blurPlayAnimationCurve.Evaluate(blurPlayedTime);

            blurMaterial.SetFloat("_BlurRange", _BLUR_MAXIMUM_RANGE * t);
            blurMaterial.SetFloat("_Darkness", 1.0f - _DARKNESS_MAXIMUM * t);
        }

        {
            UIPlayedTime += Time.deltaTime / Mathf.Max(float.Epsilon, UIPlayDuration);
            float t = UIPlayAnimationCurve.Evaluate(UIPlayedTime);

            switch (finishedType)
            {
                case FinishedType.Clear:
                    //Animation about clean title
                    foreach (var cvs in clearUIGO.GetComponentsInChildren<CanvasRenderer>())
                        cvs.SetAlpha(t);
                    break;
                case FinishedType.Failed:
                    //Animation about failed title
                    foreach (var cvs in failedUIGO.GetComponentsInChildren<CanvasRenderer>())
                        cvs.SetAlpha(t);
                    break;
                default: break;
            }
        }
    }

    private void OnEnable()
    {
        blurPlayedTime = 0.0f;
        UIPlayedTime = 0.0f;

        foreach (var cvs in clearUIGO.GetComponentsInChildren<CanvasRenderer>())
            cvs.SetAlpha(0.0f);

        foreach (var cvs in failedUIGO.GetComponentsInChildren<CanvasRenderer>())
            cvs.SetAlpha(0.0f);

        blurMaterial.SetFloat("_BlurRange", 0.0f);
        blurMaterial.SetFloat("_Darkness", 1.0f);

        clearUIGO.gameObject.SetActive(finishedType == FinishedType.Clear);
        failedUIGO.gameObject.SetActive(finishedType == FinishedType.Failed);
    }

    private void OnDestroy()
    {
        if(instance == this) instance = null;
    }

    public void Clear()
    {
        finishedType = FinishedType.None;
        gameObject.SetActive(false);
    }

    static public void SetupAndPlay(FinishedType type)
    {
        instance.finishedType = type;
        instance.gameObject.SetActive(true);
    }

}
