using System.Collections;
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
    public GameObject Test10GachaButton;

    private bool reGachaflag = false;
    private bool fadeOutFlag = false;


    // 가챠 결과 마지막에 깜빡이는 부분
    IEnumerator FADE_OUT()
    {
        if (fadeOutFlag == false)
        {
            Debug.Log("Start Fade Out ");
            do
            {
                yield return null;
            }
            while (GachaImageAnimator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
         GachaImageAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

            Debug.Log("End Fade Out ");
            GachaImageAnimator.SetBool("Play", false);
            fadeOutFlag = true;

            int gachaIndex = 1;
            UTUMSProvider.Instance.RequestGacha(gachaIndex);
        }

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

    public void ReTryGacha()
    {
        // TODO : 정상적으로 작동하지 않음
        LightEffectCircle04Animator.SetBool("Play", true);
        BlackHoleAnimator.SetBool("Play", true);
        PurpleCircleAnimator.SetBool("Play", true);

        StopAllCoroutines();
        StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE04");

        GachaButton.SetActive(false);
        ExitButton.SetActive(false);

    }

    public void SetServantGachaImage(UserServantData getServant)
    {
        Sprite sprite = null;
        SetGachaResultInfo(getServant.status);

        CharNameText.text = getServant.name;
        sprite = Resources.Load<Sprite>("UI/CharaterImage/" + getServant.name);
        charImage.GetComponent<Image>().sprite = sprite;
        fadeOutFlag = false;
    }

    public void SetMonsterGachaImage(UserMonsterData getMonster)
    {
        Sprite sprite = null;

        // TODO : ID를 이용해 CSV에서 Name을 가져오는 것 추가 필요
        CharNameText.text = getMonster.id.ToString();

        // TODO : ID를 이용해 CSV에서 Name을 가져오는 것 추가 필요
        sprite = Resources.Load<Sprite>("UI/MonsterImage/" + getMonster.id);
        charImage.GetComponent<Image>().sprite = sprite;
        fadeOutFlag = false;
    }

    public void SetItemGachaImage(UserEquipmentData getEquipment)
    {
        Sprite sprite = null;

        // 아이템 가챠는 아이콘 보여주는게 다름
        //SetGachaResultInfo(getItem.value);
        // TODO : 아이템 이름 등이 확정되면 수정필요.

        CharNameText.text = getEquipment.index.ToString();
        charImage.GetComponent<Image>().sprite = sprite;
        fadeOutFlag = false;
    }

    private void SetGachaResultInfo(Status getStatus)
    {
        GachaResultPopup.SetActive(true);

        StatusStrText.text = getStatus.basicStr.ToString();
        StatusDexText.text = getStatus.basicDex.ToString();
        StatusIntText.text = getStatus.basicInt.ToString();
    }

    public void OnClickGachaOKButton()
    {
        fadeOutFlag = false;
        LightEffectCircle04Animator.SetBool("Play", false);

        StopAllCoroutines();
        StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE04");

        GachaButton.SetActive(true);
        ExitButton.SetActive(true);
        GachaResultPopup.SetActive(false);

        PurpleCircleAnimator.SetBool("Play", false);
        BlackHoleAnimator.SetBool("Play", false);
    }

    public void OnClickCheckButton()
    {
        fadeOutFlag = false;

        StopAllCoroutines();
        LightEffectCircle04Animator.SetBool("Play", false);
        PurpleCircleAnimator.SetBool("Play", false);
        BlackHoleAnimator.SetBool("Play", false);

        GachaButton.SetActive(true);
        ExitButton.SetActive(true);
        GachaResultPopup.SetActive(false);
    }
}



