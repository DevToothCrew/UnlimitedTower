﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour {
    
    // FSM
    public enum SlotType
    {
        locked,
        servant,
        monster
    }
    public SlotType slottype;
    public UserServantData servant;
    public UserMonsterData monster;
    
    // UI
    [SerializeField] GameObject Lockobj;

    [SerializeField] Image MonsterGradeBG;
    [SerializeField] Image lefttopimage;
    [SerializeField] Image charimage;
    [SerializeField] Text leveltext;
    [SerializeField] Text EnforceCountText;




    public void Register(UserServantData servant)
    {
        slottype = SlotType.servant;
        this.servant = servant;


        InitializeUI();

        charimage.gameObject.SetActive(true);
        charimage.sprite = ErdManager.instance.GetServantIconSprite(servant.isLegend, servant.body, servant.jobNum);
        lefttopimage.gameObject.SetActive(true);
        lefttopimage.sprite = ErdManager.instance.JobIcons[servant.jobNum];
        leveltext.gameObject.SetActive(true);
        leveltext.text = "Lv." + servant.level;
    }
    public void Register(UserMonsterData monster)
    {
        slottype = SlotType.monster;
        this.monster = monster;



        InitializeUI();

        charimage.gameObject.SetActive(true);
        charimage.sprite = ErdManager.instance.getMonsterImage(monster.monsterNum,monster.monsterTypeNum);
        lefttopimage.gameObject.SetActive(true);
        lefttopimage.sprite = ErdManager.instance.TypeIcons[monster.monsterTypeNum];
        leveltext.gameObject.SetActive(true);
        leveltext.text = "Lv." + monster.level;
        EnforceCountText.gameObject.SetActive(true);
        EnforceCountText.text = monster.enforceNum > 0 ? "+" + monster.enforceNum : "";
        MonsterGradeBG.gameObject.SetActive(true);
        MonsterGradeBG.sprite = ErdManager.instance.monstergradeIcons[monster.gradeNum];
    }
    public void LockSlot()
    {
        slottype = SlotType.locked;


        InitializeUI();
        Lockobj.SetActive(true);
    }

    public void InitializeUI()
    {
        MonsterGradeBG.gameObject.SetActive(false);
        Lockobj.gameObject.SetActive(false);
        lefttopimage.gameObject.SetActive(false);
        charimage.gameObject.SetActive(false);
        leveltext.gameObject.SetActive(false);
        EnforceCountText.gameObject.SetActive(false);


    }

}
