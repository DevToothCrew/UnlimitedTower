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
    private Button backButton;

    [SerializeField]
    private Button startButton;

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
        backButton.interactable = value;
        startButton.interactable = value;
    }

    public void ExecuteGacha(bool withoutAnimation = false)
    {
        SetButtonActivate(false);

        GetComponent<Animator>()?.SetTrigger("Particle");
        particleController.BeginSummonAnimation(()=>
        {
            StartGacha(1);
            GetComponent<Animator>()?.SetTrigger("Glowing");
        });
    }

    public void CloseGachaResult()
    {
        SetButtonActivate(true);
        GetComponent<Animator>()?.SetTrigger("Invisible");
    }

    public void ExitGachaScene()
    {
        GetComponent<Animator>()?.SetTrigger("Invisible");
    }

    public void StartGacha(int gachaIndex)
    {
        // bool 처리를 통해 Gacha 처리가 필요할수도

        // GachaIndex에 따른 필요 EOS를 체크한다.

        // 현재는 1EOS로 박아두기

        if(UserDataManager.Inst.GetUserEOS() < 10000)
        {
            return;
        }

#if UNITY_EDITOR
        Cheat.Inst.GetGachaResultData(1);
#else
        PacketManager.Inst.RequestGacha(gachaIndex);
#endif
    }

}
