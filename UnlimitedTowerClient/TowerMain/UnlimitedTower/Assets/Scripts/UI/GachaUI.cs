using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoSingleton<GachaUI>
{

    public Animator gachaImages;

    public Animator lightEffectCircle01Animator;
    public Animator lightEffectCircle02Animator;
    public Animator lightEffectCircle03Animator;
    public Animator lightEffectCircle04Animator;

    public Animator purpleCircleAnimator;
    public Animator blackHoleAnimator;

    public GameObject blackHole;
    public GameObject lightEffectCircle;

    public GameObject gachaResult;
    public GameObject charImage;

    public Text charNameText;
    public Text statusStrText;
    public Text statusDexText;
    public Text statusIntText;

    public GameObject gachaButton;
    public GameObject exitButton;




    IEnumerator FADE_OUT()
    {
        do
        {
            yield return null;
        }
        while (gachaImages.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
     gachaImages.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        gachaImages.SetBool("Play", false);
        yield break;
    }

    #region LightCircles Wave Coroutine
    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE01()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      lightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        gachaImages.SetBool("Play", true);
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
    #endregion




    #region LightCircles FadeIn Coroutine
    IEnumerator FADE_IN_LIGHT_EFFECT_CIRCLE01()
    {
        do
        {
            yield return null;
        }
        while (lightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") &&
      lightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        yield break;
    }

    IEnumerator FADE_IN_LIGHT_EFFECT_CIRCLE02()
    {

        do
        {
            yield return null;
        }
        while (lightEffectCircle02Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") &&
      lightEffectCircle02Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        lightEffectCircle01Animator.SetBool("Play", false);
        yield return StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE01");
    }
    IEnumerator FADE_IN_LIGHT_EFFECT_CIRCLE03()
    {

        do
        {
            yield return null;
        }
        while (lightEffectCircle03Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") &&
      lightEffectCircle03Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        lightEffectCircle02Animator.SetBool("Play", false);
        yield return StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE02");
    }
    IEnumerator FADE_IN_LIGHT_EFFECT_CIRCLE04()
    {

       do
        {
            yield return null;
        }
        while (lightEffectCircle04Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") &&
      lightEffectCircle04Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        lightEffectCircle03Animator.SetBool("Play", false);
        yield return StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE03");
    }
    #endregion


    public void GoGacha()
    {

        lightEffectCircle04Animator.SetBool("Play", true);
        blackHoleAnimator.SetBool("Play", true);
        purpleCircleAnimator.SetBool("Play", true);
        StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE04");

        gachaButton.SetActive(false);
        exitButton.SetActive(false);
    }
    public void ReGacha()
    {
        OnClickCheckGacha();
        //GoGacha();
    }

    public void SetGachaReult(Character newChar)
    {
        gachaResult.SetActive(true);
        // 캐릭터 정보 보여주기

        charNameText.text = newChar.Name;
        statusStrText.text = newChar.Str.ToString();
        statusDexText.text = newChar.Dex.ToString();
        statusIntText.text = newChar.Int.ToString();


        // 포메이션에 추가하는 것이기 때문에 함수 이를 수정 요망.
        UserDataManager.Inst.AddNewChar(newChar.Name);

        Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + newChar.Name);
        charImage.GetComponent<Image>().sprite = sprite;
    }
    public void ShowGachaResult()
    {
        PacketManager.Inst.Request_Gacha();

       // Character newChar = new Character(UserDataManager.Inst.GetCharacterIndex() + 1);
        //Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + newChar.Name);
        //charImage.GetComponent<Image>().sprite = sprite;
        //UserDataManager.Inst.SetCharacter(newChar);
        //UserDataManager.Inst.AddNewChar(newChar.Name);
    }



    public void OnClickCheckGacha()
    {
        lightEffectCircle04Animator.SetBool("Play", false);
        StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE04");

        gachaButton.SetActive(true);
        exitButton.SetActive(true);
        gachaResult.SetActive(false);


        purpleCircleAnimator.SetBool("Play", false);
        blackHoleAnimator.SetBool("Play", false);
    }  
}



