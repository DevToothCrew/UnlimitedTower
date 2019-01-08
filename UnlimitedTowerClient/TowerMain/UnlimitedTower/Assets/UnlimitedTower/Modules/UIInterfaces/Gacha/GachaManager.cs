using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaManager : MonoBehaviour {

    static private GachaManager Instance = null;

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
        // Request to server
        // this interface can be changed
        UTUMSProvider.Instance.RequestGacha();
    }

    public void ExitGachaScene()
    {

    }

}
