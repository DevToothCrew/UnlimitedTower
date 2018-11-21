using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaEffect : MonoBehaviour {

    public Animator fadeOut;

    public Animator lightEffectCircle01;
    public Animator lightEffectCircle02;
    public Animator lightEffectCircle03;
    public Animator lightEffectCircle04;

    public Animator blackHoleAnimator;

    public GameObject blackHole;
    public GameObject lightEffectCircle;

    public bool MouseClick = false;

    private void Awake()
    {
        blackHole.GetComponent<RectTransform>().localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
    }
 
    private void OnEnable()
    {
        // 애니메이션 기본값 설정.
        MouseClick = false;
        lightEffectCircle01.SetBool("Play", false);
        lightEffectCircle02.SetBool("Play", false);
        lightEffectCircle03.SetBool("Play", false);
        lightEffectCircle04.SetBool("Play", false);

        //for(int i=0; i<4; i++)
        //{
        //    if (lightEffectCircle.transform.GetChild(i).GetComponent<RectTransform>())
        //    {
        //        Debug.Log("LightEffect : " + lightEffectCircle.transform.GetChild(i).name);
        //        lightEffectCircle.transform.GetChild(i).GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //    }
          
        //}    
        if (blackHole.GetComponent<RectTransform>())
        {

            Debug.Log("블랙홀 각도 : " + blackHole.GetComponent<RectTransform>().eulerAngles.ToString());
            Debug.Log("블랙홀 각도 : " + blackHole.GetComponent<RectTransform>().eulerAngles.ToString());

           // blackHole.GetComponent<RectTransform>().localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

        }


    }

    IEnumerator FADE_OUT()
    {
        do
        {
            yield return null;
        }
        while (fadeOut.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
     fadeOut.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        //lightEffectCircle01.SetBool("Play", false);

        yield break;
    }

    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE01()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle01.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle01.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        //lightEffectCircle01.SetBool("Play", false);

        fadeOut.SetBool("Play", true);
        yield return StartCoroutine("FADE_OUT");
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


    //public void Update()
    //{
    //    if(Input.GetMouseButtonDown(0))
    //    {
    //        GoGacha();
    //    }
    //}


    public void GoGacha()
    {
        
            MouseClick = true;
            lightEffectCircle04.SetBool("Play", true);
            blackHoleAnimator.SetBool("Play", true);
            StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE04");
        
    }
	
	
}
