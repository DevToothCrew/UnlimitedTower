using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaInfoPopup : MonoSingleton<GachaInfoPopup> {

    public GameObject GachaResultInfoPopup;
    public Text GachaResultInfoText;

    void Awake () {
        GachaResultInfoPopup.SetActive(false);
	}

    public void GachaStart()
    {
        //PacketManager.Inst.Request_Gacha();
    }

    public void SetGachaResultInfoPopup(Character getChar)
    {
        GachaResultInfoPopup.SetActive(true);
        // 캐릭터 정보 보여주기
        GachaResultInfoText.text = "Level : " + getChar.Level + "\n"
            + "Name : " + getChar.Name + "\n"
            + "Str : " + getChar.Str + "\n"
            + "Dex : " + getChar.Dex + "\n"
            + "Int : " + getChar.Int + "\n"
            + "Job : " + getChar.Job + "\n"
            + "Appear : " + getChar.Appear + "\n"
            + "MaxHP : " + getChar.GetMaxHp() + "\n"
            + "Damage : " + getChar.GetDamage() + "\n"
            + "Avoid : " + getChar.GetAvoid() + "\n"
            + "Speed : " + getChar.GetSpeed();


      //  UserDataManager.Inst.AddNewCharImage(getChar.Name);

    }

    public void OnClickCloseResultInfoPopup()
    {
        GachaResultInfoPopup.SetActive(false);
        // 정보 초기화     
    }
}
