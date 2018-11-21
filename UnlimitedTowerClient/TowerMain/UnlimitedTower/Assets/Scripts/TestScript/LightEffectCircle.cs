using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffectCircle : MonoBehaviour {



    public Animator lightEffectCircle01;
    public Animator lightEffectCircle02;
    public Animator lightEffectCircle03;
    public Animator lightEffectCircle04;

    public Animator blackHole;

    public bool MouseClick = false;

    void Awake () {
		
	}

    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE01()
    {
        Debug.Log("나도 간다");
        do
        {
            yield return null;
        }
        while (lightEffectCircle01.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle01.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        //lightEffectCircle01.SetBool("Play", false);

        yield break;
    }

    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE02()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle02.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle02.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        //lightEffectCircle02.SetBool("Play", false);


        //yield break;
        lightEffectCircle01.SetBool("Play", true);
        yield return StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE01");
    }
    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE03()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle03.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle03.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


    

        //yield break;
         lightEffectCircle02.SetBool("Play", true);
         yield return StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE02");
    }
    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE04()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle04.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle04.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        lightEffectCircle03.SetBool("Play", true);
        yield return StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE03");
    }


    void WaveLightEffect()
    {
        if(Input.GetMouseButtonDown(0) && !MouseClick)
        {
            MouseClick = true;
            lightEffectCircle04.SetBool("Play", true);
            blackHole.SetBool("Play", true);
            StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE04");
        }
    }
	
	// Update is called once per frame
	void Update () {
        WaveLightEffect();

    }
}
