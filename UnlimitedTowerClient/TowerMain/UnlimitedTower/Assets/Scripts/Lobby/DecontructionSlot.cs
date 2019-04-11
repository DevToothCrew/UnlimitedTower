using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecontructionSlot : MonoBehaviour {
    public GameObject FrameUnitInfo;
    public Image imageUnitGrade;
    public Image imageUnit;
    public Text textUnitLevel;
    public Text textStr;
    public Text textDex;
    public Text textInt;

    public GameObject FrameItemInfo;
    public Image imageItemGrade;
    public Image imageItem;
    public Text textItemLevel;
    public Image imageStat;
    public Text textStat;

    public Image imageEmptySlot;

    public void updateSlot(int unit_idx)
    {
        if (unit_idx > 0)
        {
            if (SubViewDeconstruction.Inst.GetDeconstructionType() == DECONSTRUCTION_TYPE.SERVANT || SubViewDeconstruction.Inst.GetDeconstructionType() == DECONSTRUCTION_TYPE.MONSTER)
            {
                FrameUnitInfo.SetActive(true);
                FrameItemInfo.SetActive(false);
                imageEmptySlot.enabled = false;
                this.GetComponent<Button>().interactable = true;
                
                PartyInfoVC party_info = PartyInfoVC.Inst;

                if (party_info.selectedMenu == PartyInfoVC.menu_type.SERVANT)
                {
                    UserServantData s_info = UserDataManager.Inst.GetServantInfo(unit_idx);
                    if (s_info == null)
                    {
                        Debug.Log("Invalid Servant ID : " + s_info.id);
                    }
                    imageUnitGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)s_info.grade);
                    imageUnit.sprite = CSVData.Inst.GetServantData(s_info.id).servantIcon;

                    textUnitLevel.text = string.Format("{0}", s_info.level);

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
                    imageUnitGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)m_info.grade);
                    imageUnit.sprite = CSVData.Inst.GetMonsterData(m_info.id).monsterIcon;

                    textUnitLevel.text = string.Format("{0}", m_info.level);

                    textStr.text = string.Format("{0}", m_info.status.basicStr);
                    textDex.text = string.Format("{0}", m_info.status.basicDex);
                    textInt.text = string.Format("{0}", m_info.status.basicInt);
                }
            }
            else//Equip
            {
                FrameUnitInfo.SetActive(false);
                FrameItemInfo.SetActive(true);
                imageEmptySlot.enabled = false;
                this.GetComponent<Button>().interactable = true;

                UserEquipmentData e_info = UserDataManager.Inst.GetEquipmentInfo(unit_idx);
                if (e_info == null)
                {
                    Debug.Log("Invalid Equip ID : " + e_info.id);
                }
                imageItemGrade.sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)e_info.grade);
                imageItem.sprite = CSVData.Inst.GetEquipmentData(e_info.id).equipmentIcon;

                textItemLevel.text = string.Format("{0}", e_info.upgrade);

                imageStat.sprite = CSVData.Inst.GetSpriteOptionType(e_info.optionType);
                textStat.text = string.Format("{0}", e_info.value);
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
