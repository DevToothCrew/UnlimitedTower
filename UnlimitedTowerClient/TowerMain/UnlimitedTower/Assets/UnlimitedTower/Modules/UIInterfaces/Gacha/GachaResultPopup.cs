using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultPopup : MonoBehaviour {

    public static GachaResultPopup Instance = null;

    [SerializeField]
    private Image resultImage;

    [SerializeField]
    private Text resultObjectName;

    [SerializeField]
    private Text resultExplanation;

    [SerializeField]
    private Text resultStrStat;

    [SerializeField]
    private Text resultDexStat;

    [SerializeField]
    private Text resultIntStat;

    public UTObject resultObject
    { 
        set
        {
            if(resultImage != null)
            {
                resultImage.sprite = Resources.Load<Sprite>("UI/CharaterImage/" + value.name);
            }
            else
            {
                Debug.LogWarning("[GachaResultPopup.resultObject] missing resultImage UI.");
            }

            if(resultObjectName != null)
            {
                resultObjectName.text = value.name;
            }
            else
            {
                Debug.LogWarning("[GachaResultPopup.resultObject] missing resultObjectName UI.");
            }

            if (resultExplanation != null)
            {
                resultExplanation.text = "empty";
            }
            else
            {
                Debug.LogWarning("[GachaResultPopup.resultObject] missing resultExplanation UI.");
            }

            if (value is UTUnit)
            {
                var unit = value as UTUnit;

                if(unit.status != null)
                {
                    if (resultStrStat != null)
                    {
                        resultStrStat.text = "STR  " + unit.status.basicStr.ToString();
                    }

                    if (resultDexStat != null)
                    {
                        resultDexStat.text = "DEX  " + unit.status.basicDex.ToString();
                    }

                    if (resultIntStat != null)
                    {
                        resultIntStat.text = "INT  " + unit.status.basicInt.ToString();
                    }
                }
                else
                {
                    Debug.LogWarning("[GachaResultPopup.resultObject] " + unit.name + " has no status by some reason.");
                }
            }
        }
    }


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
}
