using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager> {
    public List<AudioClip> soundList = new List<AudioClip>();
    public new AudioSource audio;

    public void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public void SoundPlay(int SoundIndex)
    {
        audio.PlayOneShot(soundList[SoundIndex]);
    }
}
