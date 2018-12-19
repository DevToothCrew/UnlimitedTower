﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

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
    private GACHA_RESULT_TYPE gachaResultType;

#if UNITY_EDITOR
    public int TestGachaNum = 1;
#endif



    public void Test_10_GoGacha()
    {
        Debug.Log("Test_10Gacha");

        // TODO : 현재 임시로 TestDB에서 캐릭터 정보 가져와서

        for(int i=0; i<10; i++)
        {
            //Character newChar = new Character(UserDataManager.Inst.GetCharacterIndex() + 1, GACHA_TYPE.Servant);
            // 가챠의 결과로 나온 캐릭터 정보를 저장한다.
            //UserDataManager.Inst.SetServant(newChar);
            //UserDataManager.Inst.AddNewCharImage(newChar, CHAR_TYPE.SERVANT);
        }
     
    }

    // 가챠 결과 마지막에 깜빡이는 부분
    IEnumerator FADE_OUT()
    {
        if(fadeOutFlag == false)
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


    public void OnClickReGacha()
    {
        reGachaflag = true ;
        fadeOutFlag = false;
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
            OnClickExecuteGacha((int)gachaResultType);
        }
    }

    public void SetGachaReult(Character newChar, GACHA_RESULT_TYPE gachaResultType)
    {
        GachaResultPopup.SetActive(true);
        Sprite sprite = null;

        if  (gachaResultType == GACHA_RESULT_TYPE.Item)
        {
            sprite = null;
        }
        else
        {
            CharNameText.text = newChar.Name;
            StatusStrText.text = newChar.Str.ToString();
            StatusDexText.text = newChar.Dex.ToString();
            StatusIntText.text = newChar.Int.ToString();

           
            if (gachaResultType == GACHA_RESULT_TYPE.Servant)
            {
                sprite = Resources.Load<Sprite>("UI/CharaterImage/" + newChar.Name);
            }
            else if (gachaResultType == GACHA_RESULT_TYPE.Monster)
            {
                sprite = Resources.Load<Sprite>("UI/MonsterImage/" + newChar.Name);
            }
        }
        charImage.GetComponent<Image>().sprite = sprite;
    }
    // TODO :Old Code
    public void SetGachaCharacterResult(string name, Status status, GACHA_RESULT_TYPE gachaType)
    {
        GachaResultPopup.SetActive(true);
        Sprite sprite = null;

        CharNameText.text = name;
        StatusStrText.text = status.basicStr.ToString();
        StatusDexText.text = status.basicDex.ToString();
        StatusIntText.text = status.basicInt.ToString();

        if (gachaType == GACHA_RESULT_TYPE.Servant)
        {
            sprite = Resources.Load<Sprite>("UI/CharaterImage/" +name);
        }
        else if (gachaType == GACHA_RESULT_TYPE.Monster)
        {
            sprite = Resources.Load<Sprite>("UI/MonsterImage/" + name);
        }

        charImage.GetComponent<Image>().sprite = sprite;
    }

    public void SetGachaResult_Servant(Servant getServant)
    {  
        Sprite sprite = null;
        SetGachaResultInfo(getServant.status);

        CharNameText.text = getServant.name;    
        sprite = Resources.Load<Sprite>("UI/CharaterImage/" + getServant.name);       
        charImage.GetComponent<Image>().sprite = sprite;

        fadeOutFlag = false;
    }

    public void SetGachaResult_Monster(Monster getMonster)
    {
        Sprite sprite = null;
        SetGachaResultInfo(getMonster.status);

        CharNameText.text = getMonster.name;       
        sprite = Resources.Load<Sprite>("UI/MonsterImage/" + getMonster.name);
        charImage.GetComponent<Image>().sprite = sprite;

        fadeOutFlag = false;
    }

    public void SetGacharResult_Item(Item getItem)
    {
        Sprite sprite = null;
        SetGachaResultInfo(getItem.status);
        // TODO : 아이템 이름 등이 확정되면 수정필요.
        CharNameText.text = "Item";
        charImage.GetComponent<Image>().sprite = sprite;

        fadeOutFlag = false;
    }


    public void SetNewGachaResult(object getGachaItem, GACHA_RESULT_TYPE gachaResultType)
    {
        GachaResultPopup.SetActive(true);
        Sprite sprite = null;

        switch (gachaResultType)
        {
            case GACHA_RESULT_TYPE.Servant:
                {
                    Servant servant = (Servant)getGachaItem;
                    SetGachaResultInfo(servant.status);
                    sprite = Resources.Load<Sprite>("UI/CharaterImage/" + servant.name);
                    break;
                }
            case GACHA_RESULT_TYPE.Monster:
                {
                    Monster monster = (Monster)getGachaItem;
                    SetGachaResultInfo(monster.status);
                    sprite = Resources.Load<Sprite>("UI/MonsterImage/" + monster.name);
                    break;
                }
            case GACHA_RESULT_TYPE.Item:
                {
                    Item item = (Item)getGachaItem;
                    SetGachaResultInfo(item.status);
                    break;
                }
            default:
                Debug.Log("Error : SetNewGachaResult");
                break;
        }

    }
    private void SetGachaResultInfo(Status getStatus)
    {
        GachaResultPopup.SetActive(true);

        StatusStrText.text = getStatus.basicStr.ToString();
        StatusDexText.text = getStatus.basicDex.ToString();
        StatusIntText.text = getStatus.basicInt.ToString();
    }






    public void ShowGachaResult()
    {
        reGachaflag = false;

#if UNITY_EDITOR
        //// ### 가챠 결과
        TestGachaNum++;
        if (TestGachaNum % 2 == 1) gachaResultType = GACHA_RESULT_TYPE.Monster;
        else gachaResultType = GACHA_RESULT_TYPE.Servant;

        PacketManager.Inst.Request_GachaResult(this.gachaResultType);
#else
        // 
#endif
        // 가챠 결과를 보여준다


    }

    // 가차 멈춤
    public void OnClickCheckGacha()
    {
        fadeOutFlag = false;
        LightEffectCircle04Animator.SetBool("Play", false);
        StartCoroutine("FADE_IN_LIGHT_EFFECT_CIRCLE04");

        GachaButton.SetActive(true);
        ExitButton.SetActive(true);
        GachaResultPopup.SetActive(false);


        PurpleCircleAnimator.SetBool("Play", false);
        BlackHoleAnimator.SetBool("Play", false);
    }


    // 가챠 시작
    public void OnClickExecuteGacha(int gachaType)
    {
        //Test : Send Gacha to Server
        // ### 가챠 시작 패킷을 보낸다.

#if UNITY_EDITOR

#else
        // Remove Code 
        //PacketManager.Inst.Request_ExecuteGacha();
         PacketManager.Inst.Request_Gacha();
#endif

        LightEffectCircle04Animator.SetBool("Play", true);
        BlackHoleAnimator.SetBool("Play", true);
        PurpleCircleAnimator.SetBool("Play", true);
        StartCoroutine("WAVE_LIGHT_EFFECT_CIRCLE04");

        GachaButton.SetActive(false);
        ExitButton.SetActive(false);
    }

}



