using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager> {
    public List<AudioClip> soundList = new List<AudioClip>();
    
    public void SoundPlay(int SoundIndex, bool isLoop = false)
    {
        StartCoroutine(SoundPlayCo(SoundIndex, isLoop));
    }

    IEnumerator SoundPlayCo(int SoundIndex, bool isLoop = false)
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = soundList[SoundIndex];
        audio.loop = isLoop;
        audio.Play();

        if (!isLoop)
        {
            for (; ; )
            {
                yield return new WaitForSeconds(0.1f);
                if (audio.isPlaying == false)
                {
                    Destroy(audio);
                    break;
                }
            }
        }
    }
}
