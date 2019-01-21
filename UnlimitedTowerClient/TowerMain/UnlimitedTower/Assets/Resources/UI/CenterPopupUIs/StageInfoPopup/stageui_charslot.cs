using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stageui_charslot : MonoBehaviour {

    [SerializeField] Image emptyImage;

    [SerializeField] Image charimage;
    [SerializeField] Image MonsterGradeImgage;
    [SerializeField] Text MonsterEnfCountText;
    [SerializeField] Image LefttopImage;
    [SerializeField] Text levelText;

    public GameObject selectedObj;

    //
    [SerializeField] int formationIndex;

    // For Debug
    [SerializeField] UserMonsterData monsterdata;
    [SerializeField] UserServantData servantdata;


    public void ToServant(UserServantData servantdata)
    {
        this.servantdata = servantdata;
        InitializeUI();

        charimage.gameObject.SetActive(true);
        charimage.sprite = ErdManager.instance.GetServantIconSprite(servantdata.isLegend, servantdata.body, servantdata.jobNum);
        LefttopImage.gameObject.SetActive(true);
        LefttopImage.sprite = ErdManager.instance.JobIcons[servantdata.jobNum];
        levelText.gameObject.SetActive(true);
        levelText.text = "Lv." + servantdata.level;
    }
    public void ToMonster(UserMonsterData monsterdata)
    {
        this.monsterdata = monsterdata;
        InitializeUI();

        MonsterGradeImgage.gameObject.SetActive(true);
        MonsterGradeImgage.sprite = ErdManager.instance.monstergradeIcons[monsterdata.gradeNum];
        MonsterEnfCountText.gameObject.SetActive(true);
        MonsterEnfCountText.text = monsterdata.enforceNum > 0 ? "+" + monsterdata.enforceNum : "";
        charimage.gameObject.SetActive(true);
        charimage.sprite = ErdManager.instance.getMonsterImage(monsterdata.monsterNum, monsterdata.monsterTypeNum);
        LefttopImage.gameObject.SetActive(true);
        LefttopImage.sprite = ErdManager.instance.TypeIcons[monsterdata.monsterTypeNum];
        levelText.gameObject.SetActive(true);
        levelText.text = "Lv." + monsterdata.level;

    }
    public void ToEmpty()
    {
        InitializeUI();

        emptyImage.gameObject.SetActive(true);
    }

    // 사용되는  UI초기화
    public void InitializeUI()
    {
        MonsterEnfCountText.gameObject.SetActive(false);
        emptyImage.gameObject.SetActive(false);
        charimage.gameObject.SetActive(false);
        MonsterGradeImgage.gameObject.SetActive(false);
        LefttopImage.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        StageInfoPopup.instance.SetFormationNum(formationIndex);
    }
}
