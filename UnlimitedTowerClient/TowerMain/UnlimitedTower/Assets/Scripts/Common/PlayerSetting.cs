using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetting : MonoSingleton<PlayerSetting>
{
    public float volum = 1.0f;
    public bool volumMute = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneLoadStart;
    }

    private void SceneLoadStart(Scene scene, LoadSceneMode mode)
    {
        AudioListener.volume = volum;
    }

    public void SoundSlider(float volum)
    {
        this.volum = volum;

        if (volumMute)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = volum;
        }
    }

    public void SoundToggle(bool isMute)
    {
        volumMute = isMute;
        if (volumMute)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = volum;
        }
    }
}
