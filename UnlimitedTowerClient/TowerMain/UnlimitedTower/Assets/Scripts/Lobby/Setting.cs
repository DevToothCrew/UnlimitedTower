using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour {
    public Slider volumSlider;
    public Toggle volumToggle;
    
    public void VolumChange()
    {
        PlayerSetting.Inst.SoundSlider(volumSlider.value);
    }

    public void VolumMute()
    {
        PlayerSetting.Inst.SoundToggle(volumToggle.isOn);
    }

    public void OnEnable()
    {
        volumSlider.value = PlayerSetting.Inst.volum;
        volumToggle.isOn = PlayerSetting.Inst.volumMute;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    #region BattleSetting
    public GameObject pauseChice;
    public GameObject gameExit;
    public GameObject gameSetting;

    public void Cancle()
    {
        pauseChice.SetActive(true);
        gameExit.SetActive(false);
        gameSetting.SetActive(false);
    }

    public void GameExit()
    {
        pauseChice.SetActive(false);
        gameExit.SetActive(true);
    }
    
    public void GameSetting()
    {
        pauseChice.SetActive(false);
        gameSetting.SetActive(true);
    }

    #endregion
}
