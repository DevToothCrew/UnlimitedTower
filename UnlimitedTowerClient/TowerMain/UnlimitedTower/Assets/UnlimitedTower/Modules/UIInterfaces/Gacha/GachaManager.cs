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
    
    public float glowingFxPanelAlpha = 0.0f;

    private void Awake()
    {
        // add event listener to get datas from server
        // this interface can be changed
        UTEventPoolInterface.AddEventListener("gacha", (result) =>
        {
            if(result is UTFailedData)
            {
                Debug.Log("Failed");
            }
            else
            {
                var gacha = result as UTPlayerManager.UTGachaData;
            }
        });
    }

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

    public void ExecuteGacha(bool withoutAnimation = false)
    {
        //GachaResultPopup.Instance.gameObject.SetActivateWithAnimation(true);

        GetComponent<Animator>()?.SetTrigger("Particle");
        particleController.BeginSummonAnimation(()=>
        {
            // Request to server
            // this interface can be changed
            UTUMSProvider.Instance.RequestGacha();
            GetComponent<Animator>()?.SetTrigger("Glowing");
        });
    }

    public void CloseGachaResult()
    {
        GetComponent<Animator>()?.SetTrigger("Invisible");
    }

    public void ExitGachaScene()
    {
    }

}
