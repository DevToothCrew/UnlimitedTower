using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour {

    static public GachaManager Instance = null;

    [SerializeField]
    private GachaBoxParticleController particleController;

    [SerializeField]
    private RawImage particleFxPanel;

    [SerializeField]
    private RawImage glowingFxPanel;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private GameObject gachaResultPopup;

    public float glowingFxPanelAlpha = 0.0f;
    
    private void Update()
    {
        glowingFxPanel?.material?.SetFloat("_Alpha", glowingFxPanelAlpha);
    }

    private void OnEnable()
    {
        Instance = Instance ?? this;
    }

    private void OnDisable()
    {
        Instance = Instance == this ? null : Instance;
    }

    public void SetButtonActivate(bool value)
    {
        // startButton.interactable = value;
    }

    public void ExecuteGacha(bool withoutAnimation = false)
    {
        // bool 처리를 통해 Gacha 처리가 필요할수도

        // GachaIndex에 따른 필요 EOS를 체크한다.

        // 현재는 1EOS로 박아두기

        if (particleController.isGachaStart == true)
        {
            particleController.BeginDispersionAnimation();
        }
        else {
            UserInventoryInfo inventoryInfo = UserDataManager.Inst.GetUserInventoryInfo();
            if (inventoryInfo == null)
            {
                DebugLog.Log(false, "Invalid Inventory Info");
                return;
            }

            if (inventoryInfo.servantInventory < UserDataManager.Inst.GetServantCount())
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("Servant Inventory is Full");
                return;
            }

            if (inventoryInfo.monsterInventory < UserDataManager.Inst.GetMonsterCount())
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("Monster Inventory is Full");
                return;
            }

            if (inventoryInfo.equipmentInventory < UserDataManager.Inst.GetEquipmentCount())
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("Equipment Inventory is Full");
                return;
            }

            if (inventoryInfo.itemInventory < UserDataManager.Inst.GetItemTotalCount())
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("Item Inventory is Full");
                return;
            }

            if (UserDataManager.Inst.GetUserEOS() < 1)
            {
                SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough EOS");
                return;
            }

#if UNITY_EDITOR
            {
                Cheat.Inst.RequestGachaCheat(1);
            }
#else
        {
            PacketManager.Inst.RequestGacha(1);
        }
#endif
        }
    }

    public void CloseGachaResult()
    {
        gachaResultPopup.SetActive(false);
        SetButtonActivate(true);
        GetComponent<Animator>()?.SetTrigger("Invisible");
    }

    public void ExitGachaScene()
    {
        GetComponent<Animator>()?.SetTrigger("Invisible");
    }

    public void ResultGacha(object getObject)
    {
        SetButtonActivate(false);

        GetComponent<Animator>()?.SetTrigger("Particle");
        particleController.BeginSummonAnimation(() =>
        {
            gachaResultPopup.SetActive(true);
            GetComponent<Animator>()?.SetTrigger("Glowing");
            GachaResultPopup.PopupAlert(getObject);
        });
    }
}
