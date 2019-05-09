using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour {
    public GameObject settingBox; 
    public Slider volumSlider;

    public void VolumChange()
    {
        PlayerSetting.Inst.SoundSlider(volumSlider.value);
    }

    public void OnEnable()
    {
        volumSlider.value = PlayerSetting.Inst.volum;
    }
}
