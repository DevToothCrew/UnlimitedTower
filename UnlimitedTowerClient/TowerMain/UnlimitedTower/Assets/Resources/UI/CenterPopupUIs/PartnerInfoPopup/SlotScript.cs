using System.Collections;
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
    [SerializeField] GameObject BGobj;
    [SerializeField] GameObject Lockobj;

    [SerializeField] Image typeImage;
    [SerializeField] Image charimage;
    [SerializeField] Text leveltext;
    [SerializeField] Text nametext;
    [SerializeField] Text EnforceCountText;


    public void register(UserServantData servant)
    {
        slottype = SlotType.servant;
        this.servant = servant;

        BGobj.gameObject.SetActive(true);
        Lockobj.gameObject.SetActive(false);

        charimage.gameObject.SetActive(true);
        typeImage.gameObject.SetActive(true);
        leveltext.gameObject.SetActive(true);
        leveltext.text = "Lv." + servant.level;
        nametext.gameObject.SetActive(true);
        nametext.text = servant.name;
        EnforceCountText.gameObject.SetActive(false);
    }
    public void register(UserMonsterData monster)
    {
        slottype = SlotType.monster;
        this.monster = monster;

        BGobj.gameObject.SetActive(true);
        Lockobj.gameObject.SetActive(false);

        charimage.gameObject.SetActive(true);
        typeImage.gameObject.SetActive(true);
        leveltext.gameObject.SetActive(true);
        leveltext.text = "Lv." + monster.level;
        nametext.gameObject.SetActive(true);
        nametext.text = monster.name;
        EnforceCountText.gameObject.SetActive(true);
        EnforceCountText.text = "+" + monster.enforceNum;
    }
    public void LockSlot()
    {
        slottype = SlotType.locked;
        BGobj.gameObject.SetActive(false);
        Lockobj.gameObject.SetActive(true);

        charimage.gameObject.SetActive(false);
        typeImage.gameObject.SetActive(false);
        leveltext.gameObject.SetActive(false);
        nametext.gameObject.SetActive(false);
        EnforceCountText.gameObject.SetActive(false);
    }

}
