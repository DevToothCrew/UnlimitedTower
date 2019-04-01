using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecontructionSlot : MonoBehaviour {
    public Image imageGrade;
    public Image imageCharacter;
    public Text textLevel;
    public Text textStr;
    public Text textDex;
    public Text textInt;
    public Image imageEmptySlot;

    public void updateSlot(int unit_idx)
    {
        if (unit_idx > 0)
        {
            this.GetComponent<Button>().interactable = true;
            imageEmptySlot.enabled = false;
            PartyInfoVC party_info = PartyInfoVC.Inst;

            if (party_info.selectedMenu == PartyInfoVC.menu_type.SERVANT)
            {
                UserServantData s_info = UserDataManager.Inst.GetServantInfo(unit_idx);
                if (s_info == null)
                {
                    Debug.Log("Invalid Servant ID : " + s_info.id);
                }
                imageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)s_info.grade);
                imageCharacter.sprite = CSVData.Inst.GetServantData(s_info.id).servantIcon;

                textLevel.text = string.Format("{0}", s_info.level);

                textStr.text = string.Format("{0}", s_info.status.basicStr);
                textDex.text = string.Format("{0}", s_info.status.basicDex);
                textInt.text = string.Format("{0}", s_info.status.basicInt);
            }
            else if (party_info.selectedMenu == PartyInfoVC.menu_type.MONSTER)
            {
                UserMonsterData m_info = UserDataManager.Inst.GetMonsterInfo(unit_idx);
                if (m_info == null)
                {
                    Debug.Log("Invalid Monster ID : " + m_info.id);
                }
                imageGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)m_info.grade);
                imageCharacter.sprite = CSVData.Inst.GetMonsterData(m_info.id).monsterIcon;

                textLevel.text = string.Format("{0}", m_info.level);

                textStr.text = string.Format("{0}", m_info.status.basicStr);
                textDex.text = string.Format("{0}", m_info.status.basicDex);
                textInt.text = string.Format("{0}", m_info.status.basicInt);
            }
        }
        else
        {
            this.GetComponent<Button>().interactable = false;
            imageEmptySlot.enabled = true;
        }
       
    }

    public void OnClickSlot(int btn_idx)
    {
        SubViewDeconstruction.Inst.OnClickCancelSlot(btn_idx);
    }
}
