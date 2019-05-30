using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoSingleton<GachaManager> {

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
    public int gachaID;
    
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

    public void SkipButton()
    {
        ExecuteGacha(gachaID);
    }

    public void ExecuteGacha(int id)
    {
        // bool 처리를 통해 Gacha 처리가 필요할수도

        // GachaIndex에 따른 필요 EOS를 체크한다.

        // id 11 = EOS x1 / id 12 = EOX x10 / id 21 = UTG x 1 / id 22 = UTG x 10

        DebugLog.Log(false, "Pre ID : " + gachaID + ", Request ID : " + id);
        gachaID = id;

        if (particleController.isGachaStart == true)
        {
            particleController.BeginDispersionAnimation();
        }
        else {
            if (UserDataManager.Inst.CheckInventoryCount() == false)
            {
                return;
            }

            if (gachaID == 11)
            {
                if (UserDataManager.Inst.GetUserEOS() < DEFINE.NeedGachaEOS)
                {
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough EOS");
                    return;
                }
            }
            else if(gachaID == 21)
            {
                if (UserDataManager.Inst.GetUserUTG() < DEFINE.NeedGachaUTG)
                {
                    SimpleErrorPopupVC.Inst.UpdateErrorText("Not Enough UTG");
                    return;
                }
            }

#if UNITY_EDITOR
            {
                Cheat.Inst.RequestGachaCheat(gachaID);
            }
#else
        {
            PacketManager.Inst.RequestGacha(gachaID);
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
