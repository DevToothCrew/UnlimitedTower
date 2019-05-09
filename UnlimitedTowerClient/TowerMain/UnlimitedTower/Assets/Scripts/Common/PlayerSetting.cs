using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetting : MonoSingleton<PlayerSetting> {
    public float volum = 1.0f;

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
        AudioListener.volume = volum;
    }
}
