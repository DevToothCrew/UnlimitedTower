using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
}
