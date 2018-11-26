using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoSingleton<GachaUI>
{

    public Animator gachaImagesAnimator;

    public Animator lightEffectCircle01Animator;
    public Animator lightEffectCircle02Animator;
    public Animator lightEffectCircle03Animator;
    public Animator lightEffectCircle04Animator;

    public Animator purpleCircleAnimator;
    public Animator blackHoleAnimator;

    public GameObject blackHole;
    public GameObject gachaResultPopup;
    public GameObject charImage;

    public Text charNameText;
    public Text statusStrText;
    public Text statusDexText;
    public Text statusIntText;

    public GameObject gachaButton;
    public GameObject exitButton;

    private bool reGachaflag = false;


    IEnumerator FADE_OUT()
    {
        do
        {
            yield return null;
        }
        while (gachaImagesAnimator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
     gachaImagesAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        gachaImagesAnimator.SetBool("Play", false);
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


        gachaImagesAnimator.SetBool("Play", true);
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

        Debug.Log(" WAVE_LIGHT_EFFECT_CIRCLE04");

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

        ReTryGacha();
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

        Debug.Log("FADE_IN_LIGHT_EFFECT_CIRCLE04");

        lightEffectCircle03Animator.SetBool("Play", false);
        yield return StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE03");
    }
    #endregion


    public void ReGacha()
    {
        reGachaflag = true ;

        gachaResultPopup.SetActive(false);
        lightEffectCircle04Animator.SetBool("Play", false);
        StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE04");

        purpleCircleAnimator.SetBool("Play", false);
        blackHoleAnimator.SetBool("Play", false);
    }

    public void ReTryGacha()
    {
        if (reGachaflag)
        {
            GoGacha();
        }
    }

    public void SetGachaReult(Character newChar)
    {
        gachaResultPopup.SetActive(true);
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
        reGachaflag = false;
        PacketManager.Inst.Request_Gacha();
    }


    public void OnClickCheckGacha()
    {
        lightEffectCircle04Animator.SetBool("Play", false);
        StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE04");

        gachaButton.SetActive(true);
        exitButton.SetActive(true);
        gachaResultPopup.SetActive(false);


        purpleCircleAnimator.SetBool("Play", false);
        blackHoleAnimator.SetBool("Play", false);
    }


    public void GoGacha()
    {
        lightEffectCircle04Animator.SetBool("Play", true);
        blackHoleAnimator.SetBool("Play", true);
        purpleCircleAnimator.SetBool("Play", true);
        StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE04");

        gachaButton.SetActive(false);
        exitButton.SetActive(false);
    }

}



