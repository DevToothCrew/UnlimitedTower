using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserExpBarUpdate : MonoBehaviour {
    private Image bar;

    private void Awake()
    {
        bar = GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        bar.fillAmount = UserDataManager.Inst.userInfo.userEXP / (float)CSVData.Inst.DBExpDataDic[UserDataManager.Inst.userInfo.level].rankExp;
        Debug.Log("User Level : " + UserDataManager.Inst.userInfo.level);
        Debug.Log("User Exp : " + UserDataManager.Inst.userInfo.userEXP);
        Debug.Log("Next Exp : " + CSVData.Inst.DBExpDataDic[UserDataManager.Inst.userInfo.level].rankExp);
    }

}
