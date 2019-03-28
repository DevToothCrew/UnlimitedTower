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

    [SerializeField]
    private Button retryButton;

    [SerializeField]
    private Button exitButton;
    
    private void OnEnable()
    {
        SetActiveButtons(false);
        Instance = Instance ?? this;
    }

    private void OnDisable()
    {
        SetActiveButtons(false);
        Instance = null;
    }

    public void Retry()
    {
        SetActiveButtons(false);

        gameObject.SetActivateWithAnimation(false);
        GachaManager.Instance.ExecuteGacha();
    }

    public void Exit()
    {
        SetActiveButtons(false);

        gameObject.SetActivateWithAnimation(false);
        GachaManager.Instance.CloseGachaResult();
    }

    public void SetActiveButtons(bool value)
    {
        retryButton.interactable = value;
        exitButton.interactable = value;
    }

    public void Popup(UserServantData result)
    {
        SetActiveButtons(true);

        DBServantData servantData = CSVData.Inst.GetServantData(result.id);

        resultName.text = servantData.name;
        resultType.text = result.grade.ToString();
        resultStrStat.text = result.status.basicStr.ToString();
        resultIntStat.text = result.status.basicInt.ToString();
        resultDexStat.text = result.status.basicDex.ToString();
    }

    public void Popup(UserMonsterData result)
    {
        SetActiveButtons(true);

        //resultName.text = result.name;
        resultType.text = result.grade.ToString();
        resultStrStat.text = result.status.basicStr.ToString();
        resultIntStat.text = result.status.basicInt.ToString();
        resultDexStat.text = result.status.basicDex.ToString();
    }

    public void Popup(UserEquipmentData result)
    {
        SetActiveButtons(true);
        throw new System.NotImplementedException();
    }

    public static void PopupAlert(object result)
    {
        Debug.Log(result);

        if(result is UserServantData)
        {
            Instance.gameObject.SetActivateWithAnimation(true);
            Instance.Popup(result as UserServantData);
        }

        else if (result is UserMonsterData)
        {
            Instance.gameObject.SetActivateWithAnimation(true);
            Instance.Popup(result as UserMonsterData);
        }

        else if (result is UserEquipmentData)
        {
            Instance.gameObject.SetActivateWithAnimation(true);
            Instance.Popup(result as UserEquipmentData);
        }

        else
        {
            throw new System.Exception("Unexpected type result object : " + result);
        }
    }
}
