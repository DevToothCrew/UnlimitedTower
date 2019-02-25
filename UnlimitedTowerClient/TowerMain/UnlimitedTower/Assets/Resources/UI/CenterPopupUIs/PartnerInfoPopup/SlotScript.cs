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
    [SerializeField] GameObject Lockobj;

    [SerializeField] Image MonsterGradeBG;
    [SerializeField] Image lefttopimage;
    [SerializeField] Image charimage;
    [SerializeField] Text leveltext;
    [SerializeField] Text EnforceCountText;

    [SerializeField] GameObject sellCheckedObj;
    

    public void Register(UserServantData servant)
    {
        slottype = SlotType.servant;
        this.servant = servant;


        InitializeUI();

        charimage.gameObject.SetActive(true);

        bool isLegend = servant.isLegend;
        int bodyNum = servant.body;
        int jobNum = servant.jobNum;
        
        // Servant Image
        //charimage.sprite = ErdManager.instance.GetServantIconSprite(isLegend, bodyNum, jobNum);
        lefttopimage.gameObject.SetActive(true);
        
        // Servant Job Icon
        //lefttopimage.sprite = ErdManager.instance.JobIcons[servant.jobNum];
        leveltext.gameObject.SetActive(true);
        leveltext.text = "Lv." + servant.level;


        // 해당 서번트가 판매리스트에 포함되어있다면
        if (PopupUIsManager.instance.partnerInfoPopup.sellingServantList.Contains(servant) &&
            PopupUIsManager.instance.partnerInfoPopup.modeType == PartnerInfoPopup.MODE_TYPE.SELLING)
        {
            sellCheckedObj.SetActive(true);
        }

        //
        PartnerInfoPopup.servantListChanged += servantListChanged;
        PartnerInfoPopup.modeChanged += modeChanged;
        modeChanged();


    }
    public void Register(UserMonsterData monster)
    {
        slottype = SlotType.monster;
        this.monster = monster;



        InitializeUI();

        charimage.gameObject.SetActive(true);
        
        // Monster Image
        //charimage.sprite = ErdManager.instance.getMonsterImage(monster.monsterNum,monster.monsterTypeNum);
        lefttopimage.gameObject.SetActive(true);
        
        // Monster Type Icon
        //lefttopimage.sprite = ErdManager.instance.TypeIcons[monster.monsterTypeNum];
        leveltext.gameObject.SetActive(true);
        leveltext.text = "Lv." + monster.level;
        EnforceCountText.gameObject.SetActive(true);
        EnforceCountText.text = monster.enforceNum > 0 ? "+" + monster.enforceNum : "";
        MonsterGradeBG.gameObject.SetActive(true);
        
        // Monster Grade Icon
        //MonsterGradeBG.sprite = ErdManager.instance.monstergradeIcons[monster.gradeNum];

        // 해당 몬스터가 판매리스트에 포함되어있다면
        if (PopupUIsManager.instance.partnerInfoPopup.sellingMonsterList.Contains(monster) &&
            PopupUIsManager.instance.partnerInfoPopup.modeType == PartnerInfoPopup.MODE_TYPE.SELLING)
        {
            sellCheckedObj.SetActive(true);
        }

        //
        PartnerInfoPopup.monsterListChanged += monsterListChanged;
        PartnerInfoPopup.modeChanged += modeChanged;
        modeChanged();
    }
    public void LockSlot()
    {
        switch (slottype)
        {
            case SlotType.locked:
                break;
            case SlotType.servant:
                PartnerInfoPopup.servantListChanged -= servantListChanged;
                PartnerInfoPopup.modeChanged -= modeChanged;
                break;
            case SlotType.monster:
                PartnerInfoPopup.monsterListChanged -= monsterListChanged;
                PartnerInfoPopup.modeChanged -= modeChanged;
                break;
        }
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
        sellCheckedObj.SetActive(false);

    }


    // 
    public void monsterListChanged(UserMonsterData monsterdata)
    {
        // 셀링모드가 아니라면, 끄고 return
        if (PopupUIsManager.instance.partnerInfoPopup.modeType == PartnerInfoPopup.MODE_TYPE.JUST_DISPLAY)
        {
            sellCheckedObj.SetActive(false);
            return;
        }

        // 내몬스터가 아니라면, return
        if (monsterdata != monster)
        {
            return;
        }


        //
        sellCheckedObj.SetActive(PopupUIsManager.instance.partnerInfoPopup.sellingMonsterList.Contains(monster));
    }
    public void servantListChanged(UserServantData servantdata)
    {
        // 셀링모드가 아니라면, 끄고 return
        if (PopupUIsManager.instance.partnerInfoPopup.modeType == PartnerInfoPopup.MODE_TYPE.JUST_DISPLAY)
        {
            sellCheckedObj.SetActive(false);
            return;
        }

        // 내몬스터가 아니라면, return
        if (servantdata != servant)
        {
            return;
        }


        //
        sellCheckedObj.SetActive(PopupUIsManager.instance.partnerInfoPopup.sellingServantList.Contains(servant));
    }
    //
    public void modeChanged()
    {
        // 셀링모드가 아니면,  꺼주기
        if (PopupUIsManager.instance.partnerInfoPopup.modeType == PartnerInfoPopup.MODE_TYPE.JUST_DISPLAY)
        {
            sellCheckedObj.SetActive(false);
        }
        
    }



    //
    public void OnClick()
    {

        switch (PopupUIsManager.instance.partnerInfoPopup.modeType)
        {
            // 디스플레이모드일때
            case PartnerInfoPopup.MODE_TYPE.JUST_DISPLAY:
                {
                    // 
                    switch (slottype)
                    {
                        case SlotType.locked:
                            {

                            }
                            break;
                        case SlotType.servant:
                            {
                                PopupUIsManager.instance.partnerInfoPopup.gameObject.SetActivateWithAnimation(false);
                                PopupUIsManager.instance.heroInfoPopup.ToRegistered(servant);
                            }
                            break;
                        case SlotType.monster:
                            {
                                PopupUIsManager.instance.partnerInfoPopup.gameObject.SetActivateWithAnimation(false);
                                PopupUIsManager.instance.heroInfoPopup.ToRegistered(monster);
                            }
                            break;
                    }
                }
                break;

                // 파는 모드일때
            case PartnerInfoPopup.MODE_TYPE.SELLING:
                {
                    switch (slottype)
                    {
                        case SlotType.servant:
                            {
                                PopupUIsManager.instance.partnerInfoPopup.OnClickInSellingmode(servant);
                            }
                            break;
                        case SlotType.monster:
                            {
                                PopupUIsManager.instance.partnerInfoPopup.OnClickInSellingmode(monster);
                            }
                            break;
                    }
                }
                break;
        }
        
    }
}
