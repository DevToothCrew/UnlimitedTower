using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelAnimation : MonoBehaviour {
    public GameObject[] aniObject;
    public int startIndex;
    public float animationDelay;

    private void OnEnable()
    {
        StartCoroutine(AnimationStart());
    }

    IEnumerator AnimationStart()
    {
        int index = startIndex;
        for (; ; )
        {
            index++;
            for (int i = 0; i < aniObject.Length; i++)
            {
                aniObject[i].SetActive(false);
            }
            aniObject[index % aniObject.Length].SetActive(true);
            yield return new WaitForSeconds(animationDelay);
        }
    }
}
