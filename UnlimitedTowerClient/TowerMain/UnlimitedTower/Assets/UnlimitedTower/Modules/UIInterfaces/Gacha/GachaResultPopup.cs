using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultPopup : MonoBehaviour {

    public static GachaResultPopup Instance = null;

    [SerializeField]
    private RawImage resultImage;

    [SerializeField]
    private Text resultName;

    [SerializeField]
    private Text resultType;

    [SerializeField]
    private Text resultStrStat;

    [SerializeField]
    private Text resultDexStat;

    [SerializeField]
    private Text resultIntStat;
    
    private void OnEnable()
    {
        Instance = Instance ?? this;
    }

    private void OnDisable()
    {
        Instance = null;
    }

    public void Exit()
    {
        gameObject.SetActivateWithAnimation(false);
    }

    public void Popup()
    {
        gameObject.SetActivateWithAnimation(true);
    }

    void SetResultObject(object result)
    {
        //need to be implemented
        throw new System.NotImplementedException();
    }
}
