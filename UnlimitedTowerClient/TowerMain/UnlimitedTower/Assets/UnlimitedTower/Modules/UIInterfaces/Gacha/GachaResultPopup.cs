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

    public void Retry()
    {
        gameObject.SetActivateWithAnimation(false);
        GachaManager.Instance.ExecuteGacha();
    }

    public void Exit()
    {
        gameObject.SetActivateWithAnimation(false);
        GachaManager.Instance.CloseGachaResult();
    }

    public void Popup(servantData result)
    {
        resultName.text = result.servant.name;
        resultType.text = result.servant.isLegend ? "Legendary" : "Normal";
        resultStrStat.text = result.servant.status.basic_str.ToString();
        resultIntStat.text = result.servant.status.basic_int.ToString();
        resultDexStat.text = result.servant.status.basic_dex.ToString();

        gameObject.SetActivateWithAnimation(true);
    }

    public void Popup(monsterData result)
    {
        resultName.text = result.monster.name;
        resultType.text = result.monster.grade.ToString();
        resultStrStat.text = result.monster.status.basic_str.ToString();
        resultIntStat.text = result.monster.status.basic_int.ToString();
        resultDexStat.text = result.monster.status.basic_dex.ToString();

        gameObject.SetActivateWithAnimation(true);
    }

    public void Popup(itemData result)
    {
        gameObject.SetActivateWithAnimation(true);
        throw new System.NotImplementedException();
    }
}
