using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultPopup : MonoBehaviour {

    public static GachaResultPopup Instance = null;

    [SerializeField]
    private RawImage gradeImage;

    [SerializeField]
    private RawImage resultImage;

    [SerializeField]
    private Text resultName;

    [SerializeField]
    private GameObject characterStatus;

    [SerializeField]
    private Text resultStrStat;

    [SerializeField]
    private Text resultDexStat;

    [SerializeField]
    private Text resultIntStat;

    [SerializeField]
    private GameObject equipmentValue;

    [SerializeField]
    private RawImage valueStatusImage;

    [SerializeField]
    private GameObject valueTypeObject;

    [SerializeField]
    private RawImage valueTypeImage;

    [SerializeField]
    private Text resultValueStat;

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
        if(servantData == null)
        {
            Debug.Log("Invalid Servant ID : " + result.id);
            return;
        }

        resultName.text = servantData.name;
        resultImage.texture = servantData.servantIcon.texture;
        gradeImage.texture = GachaManager.Instance.GetGradeTexture(servantData.grade);

        resultStrStat.text = result.status.basicStr.ToString();
        resultIntStat.text = result.status.basicInt.ToString();
        resultDexStat.text = result.status.basicDex.ToString();
    }

    public void Popup(UserMonsterData result)
    {
        SetActiveButtons(true);

        DBMonsterData monsterData = CSVData.Inst.GetMonsterData(result.id);
        if(monsterData == null)
        {
            Debug.Log("Invalid Monster ID : " + result.id);
            return;
        }

        resultName.text = monsterData.name;
        resultImage.texture = monsterData.monsterIcon.texture;
        gradeImage.texture = GachaManager.Instance.GetGradeTexture((GRADE_TYPE)result.grade);

        resultStrStat.text = result.status.basicStr.ToString();
        resultIntStat.text = result.status.basicInt.ToString();
        resultDexStat.text = result.status.basicDex.ToString();
    }

    public void Popup(UserEquipmentData result)
    {
        SetActiveButtons(true);

        DBEquipmentData equipmentData = CSVData.Inst.GetEquipmentData(result.id);
        if(equipmentData == null)
        {
            Debug.Log("Invalid Equipment ID : " + result.id);
            return;
        }

        resultName.text = equipmentData.tier + "T " + equipmentData.name;
        if(equipmentData.equipmentIcon.texture == null)
        {
            Debug.Log("Invalid Texture : " + equipmentData.name);
        }
        resultImage.texture = equipmentData.equipmentIcon.texture;
        gradeImage.texture = GachaManager.Instance.GetGradeTexture((GRADE_TYPE)result.grade);

        switch(equipmentData.optionType)
        {
            case EQUIPMENT_OPTION_TYPE.ATK:
            case EQUIPMENT_OPTION_TYPE.DEF:
            case EQUIPMENT_OPTION_TYPE.MATK:
            case EQUIPMENT_OPTION_TYPE.MDEF:
            case EQUIPMENT_OPTION_TYPE.HP:
                valueTypeObject.SetActive(true);
                valueStatusImage.texture = GachaManager.Instance.GetEquipmentOptionTexture(EQUIPMENT_OPTION_TYPE.MAX);
                valueTypeImage.texture = GachaManager.Instance.GetEquipmentOptionTexture(equipmentData.optionType);
                break;

            case EQUIPMENT_OPTION_TYPE.STR:
            case EQUIPMENT_OPTION_TYPE.DEX:
            case EQUIPMENT_OPTION_TYPE.INT:
                valueTypeObject.SetActive(false);
                valueStatusImage.texture = GachaManager.Instance.GetEquipmentOptionTexture(equipmentData.optionType);
                break;

            default:
                Debug.Log("Invalid Equipment Data");
                return;
        }
        resultValueStat.text = result.value.ToString();
    }

    public static void PopupAlert(object result)
    {
        Debug.Log(result);

        if(result is UserServantData)
        {
            Instance.characterStatus.SetActive(true);
            Instance.equipmentValue.SetActive(false);

            Instance.gameObject.SetActivateWithAnimation(true);
            Instance.Popup(result as UserServantData);
        }

        else if (result is UserMonsterData)
        {
            Instance.characterStatus.SetActive(true);
            Instance.equipmentValue.SetActive(false);

            Instance.gameObject.SetActivateWithAnimation(true);
            Instance.Popup(result as UserMonsterData);
        }

        else if (result is UserEquipmentData)
        {
            Instance.equipmentValue.SetActive(true);
            Instance.characterStatus.SetActive(false);            

            Instance.gameObject.SetActivateWithAnimation(true);
            Instance.Popup(result as UserEquipmentData);
        }

        else
        {
            throw new System.Exception("Unexpected type result object : " + result);
        }
    }
}
