using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaImage : MonoSingleton<GachaImage>
{

    public Animator GachaImageAnimator;

    public Animator LightEffectCircle01Animator;
    public Animator LightEffectCircle02Animator;
    public Animator LightEffectCircle03Animator;
    public Animator LightEffectCircle04Animator;

    public Animator PurpleCircleAnimator;
    public Animator BlackHoleAnimator;

    public GameObject BlackHole;
    public GameObject GachaResultPopup;
    public GameObject charImage;

    public Text CharNameText;
    public Text StatusStrText;
    public Text StatusDexText;
    public Text StatusIntText;

    public GameObject GachaButton;
    public GameObject ExitButton;

    private bool reGachaflag = false;


    IEnumerator FADE_OUT()
    {
        do
        {
            yield return null;
        }
        while (GachaImageAnimator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
     GachaImageAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        GachaImageAnimator.SetBool("Play", false);
        yield break;
    }

    #region LightCircles Wave Coroutine
    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE01()
    {
        do
        {
            yield return null;
        }
        while (LightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      LightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        GachaImageAnimator.SetBool("Play", true);
        yield return StartCoroutine("FADE_OUT");
    }

    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE02()
    {
        do
        {
            yield return null;
        }
        while (LightEffectCircle02Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      LightEffectCircle02Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        LightEffectCircle01Animator.SetBool("Play", true);
        yield return StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE01");
    }
    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE03()
    {
        do
        {
            yield return null;
        }
        while (LightEffectCircle03Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      LightEffectCircle03Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        LightEffectCircle02Animator.SetBool("Play", true);
         yield return StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE02");
    }
    IEnumerator WAVE_LIGHT_EFFECT_CIRCLE04()
    {
        do
        {
            yield return null;
        }
        while (LightEffectCircle04Animator.GetCurrentAnimatorStateInfo(0).IsName("Wave") &&
      LightEffectCircle04Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        LightEffectCircle03Animator.SetBool("Play", true);
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
        while (LightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") &&
      LightEffectCircle01Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        // 가챠 결과창에서 바로 가챠 시도할 때 사용하는 함수
        ReTryGacha();
        yield break;
    }

    IEnumerator FADE_IN_LIGHT_EFFECT_CIRCLE02()
    {
        do
        {
            yield return null;
        }
        while (LightEffectCircle02Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") &&
      LightEffectCircle02Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        LightEffectCircle01Animator.SetBool("Play", false);
        yield return StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE01");
    }
    IEnumerator FADE_IN_LIGHT_EFFECT_CIRCLE03()
    {
        do
        {
            yield return null;
        }
        while (LightEffectCircle03Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") &&
      LightEffectCircle03Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        LightEffectCircle02Animator.SetBool("Play", false);
        yield return StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE02");
    }
    IEnumerator FADE_IN_LIGHT_EFFECT_CIRCLE04()
    {
       do
        {
            yield return null;
        }
        while (LightEffectCircle04Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") &&
      LightEffectCircle04Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);


        LightEffectCircle03Animator.SetBool("Play", false);
        yield return StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE03");
    }
    #endregion


    public void ReGacha()
    {
        reGachaflag = true ;

        GachaResultPopup.SetActive(false);
        LightEffectCircle04Animator.SetBool("Play", false);
        StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE04");

        PurpleCircleAnimator.SetBool("Play", false);
        BlackHoleAnimator.SetBool("Play", false);
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
        GachaResultPopup.SetActive(true);
 
        CharNameText.text = newChar.Name;
        StatusStrText.text = newChar.Str.ToString();
        StatusDexText.text = newChar.Dex.ToString();
        StatusIntText.text = newChar.Int.ToString();


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
        LightEffectCircle04Animator.SetBool("Play", false);
        StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE04");

        GachaButton.SetActive(true);
        ExitButton.SetActive(true);
        GachaResultPopup.SetActive(false);


        PurpleCircleAnimator.SetBool("Play", false);
        BlackHoleAnimator.SetBool("Play", false);
    }


    public void GoGacha()
    {
        LightEffectCircle04Animator.SetBool("Play", true);
        BlackHoleAnimator.SetBool("Play", true);
        PurpleCircleAnimator.SetBool("Play", true);
        StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE04");

        GachaButton.SetActive(false);
        ExitButton.SetActive(false);
    }

}



