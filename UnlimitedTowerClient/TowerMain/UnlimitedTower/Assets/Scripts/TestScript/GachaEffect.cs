using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaEffect : MonoBehaviour {

    public Animator fadeOut;

    public Animator lightEffectCircle01Animator;
    public Animator lightEffectCircle02Animator;
    public Animator lightEffectCircle03Animator;
    public Animator lightEffectCircle04Animator;

    public Animator purpleCircleAnimator;
    public Animator blackHoleAnimator;

    public GameObject blackHole;
    public GameObject lightEffectCircle;


    private void Awake()
    {
        blackHole.GetComponent<RectTransform>().localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
    }
 


    IEnumerator FADE_OUT()
    {
        do
        {
            yield return null;
        }
        while (fadeOut.GetCurrentAnimatorStateInfo(0).IsName("TestFadeOut") &&
     fadeOut.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        fadeOut.SetBool("Play", false);
        yield break;
    }

    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE01()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        fadeOut.SetBool("Play", true);
        yield return StartCoroutine("FADE_OUT");
    }

    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE02()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle02Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle02Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        //lightEffectCircle02.SetBool("Play", false);


        //yield break;
        lightEffectCircle01Animator.SetBool("Play", true);
        yield return StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE01");
    }
    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE03()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle03Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle03Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);




        //yield break;
        lightEffectCircle02Animator.SetBool("Play", true);
         yield return StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE02");
    }
    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE04()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle04Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle04Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        lightEffectCircle03Animator.SetBool("Play", true);
        yield return StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE03");
    }



    public void GoGacha()
    {

        lightEffectCircle04Animator.SetBool("Play", true);
        blackHoleAnimator.SetBool("Play", true);
        purpleCircleAnimator.SetBool("Play", true);
        StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE04");

    }

    public void ShowGachaResult()
    {
        //PacketManager.Inst.Request_Gacha();
        Debug.Log("Gacha Result");

        Character newChar = new Character(UserDataManager.Inst.GetCharacterIndex() + 1);
        UserDataManager.Inst.SetCharacter(newChar);

        UserDataManager.Inst.AddNewChar(newChar.Name);

        // var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
    }
	
	
}
