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

    public void Popup(servantData result)
    {
        SetActiveButtons(true);

        //resultName.text = result.servant.name;
        //resultType.text = result.servant.islegend ? "Legendary" : "Normal";
        resultStrStat.text = result.servant.status.basic_str.ToString();
        resultIntStat.text = result.servant.status.basic_int.ToString();
        resultDexStat.text = result.servant.status.basic_dex.ToString();

        gameObject.SetActivateWithAnimation(true);
    }

    public void Popup(monsterData result)
    {
        SetActiveButtons(true);

        //resultName.text = result.monster.name;
        resultType.text = result.monster.grade.ToString();
        resultStrStat.text = result.monster.status.basic_str.ToString();
        resultIntStat.text = result.monster.status.basic_int.ToString();
        resultDexStat.text = result.monster.status.basic_dex.ToString();

        gameObject.SetActivateWithAnimation(true);
    }

    public void Popup(itemData result)
    {
        SetActiveButtons(true);

        gameObject.SetActivateWithAnimation(true);
        throw new System.NotImplementedException();
    }
}
