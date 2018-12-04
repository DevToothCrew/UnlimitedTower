using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameCharacterStateUI : MonoBehaviour
{

    [Space(20)]
    [Header("Connection with UIs")]
    [Space(10)]
    public Image uiThumbnailImage;
    public Text uiLevelText;
    public Text uiNameText;
    public Image[] uiSlotImages;
    public Image[] uiStatusImages;
    public Text[] uiStatusTexts;

    static private InGameCharacterStateUI instance = null;
    static public InGameCharacterStateUI Inst { get { return instance; } }

    private void Awake()
    {
        instance = instance ?? this;
        gameObject.SetActive(false);
    }

    static public void SetThumbnail(Sprite img)
    {
        Inst.uiThumbnailImage.sprite = img;
    }

    static public void SetLevel(int level)
    {
        Inst.uiLevelText.text = "Lv. " + level.ToString().PadLeft(2, '0');
    }

    static public void SetName(string name)
    {
        Inst.uiNameText.text = name;
    }

    static public void SetHeadSlotImage(int slotIdx, Sprite img)
    {
        if(slotIdx < Inst.uiSlotImages.Length)
        {
            Inst.uiSlotImages[slotIdx].sprite = img;
        }
        else
        {
            Debug.LogWarning("InGameCharacterStateUI.SetHeadSlotImage : out of range in array");
        }
    }

    static public void SetStatusSlot(int slotIdx, Sprite img, string val)
    {
        if(slotIdx < Inst.uiStatusImages.Length)
        {
            Inst.uiStatusImages[slotIdx].sprite = img;
            Inst.uiStatusTexts[slotIdx].text = val;
        }
        else
        {
            Debug.LogWarning("InGameCharacterStateUI.SetStatus(idx, img, val) : out of range in array");
        }
    }

    static public void SetStatusSlot(int slotIdx, string val)
    {
        if (slotIdx < Inst.uiStatusTexts.Length)
        {
            Inst.uiStatusTexts[slotIdx].text = val;
        }
        else
        {
            Debug.LogWarning("InGameCharacterStateUI.SetStatus : out of range in array");
        }
    }

    /// <summary>
    /// defaultly will setup about whole states
    /// </summary>
    /// <param name="status"></param>
    static public void SetStatus(Battle_Character_Status status)
    {
        if(status != null)
        {
            SetLevel(status.character.Level);
            SetName(status.character.Name);
            
            SetStatusSlot(0, status.nowHp + "/" + status.maxHp);
            SetStatusSlot(1, "" + status.speed);
            SetStatusSlot(2, "" + status.avoid);
            SetStatusSlot(3, "" + status.damage);
            //SetStatusSlot(4, "" + status.lived);
            //SetStatusSlot(5, "" + status.partyIndex);
        }
    }
    
}
