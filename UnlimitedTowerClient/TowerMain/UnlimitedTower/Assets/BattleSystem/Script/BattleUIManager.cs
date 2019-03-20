using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleUIManager : MonoSingleton<BattleUIManager>
{
    public Animator attackButtonani;
    public Animator defenceButtonani;
    public Animator turnEndButtonani;
    private GameObject heroTargetObject;
    public GameObject heroTargetBorder;
    private Image heroImage;
    public Image heroTargetImage;
    private GameObject monsterTargetObject;
    public GameObject monsterTargetBorder;
    private Image monsterImage;
    public Image monsterTargetImage;
    public Sprite defenceImage;
    public Sprite blank;
    private Animator targetAni;

    public void Awake()
    {
        attackButtonani = GameObject.Find("Attack Action").GetComponent<Animator>();
        defenceButtonani = GameObject.Find("Defence Action").GetComponent<Animator>();
        turnEndButtonani = GameObject.Find("TurnEnd Action").GetComponent<Animator>();
        heroTargetObject = GameObject.Find("HeroSelect");
        heroImage = heroTargetObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        heroTargetImage = heroTargetObject.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        heroTargetBorder = heroTargetObject.transform.GetChild(1).GetChild(1).gameObject;
        monsterTargetObject = GameObject.Find("MonsterSelect");
        monsterImage = monsterTargetObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        monsterTargetImage = monsterTargetObject.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        monsterTargetBorder = monsterTargetObject.transform.GetChild(1).GetChild(1).gameObject;
        targetAni = GameObject.Find("Target Select").GetComponent<Animator>();
        // 히어로 이미지 설정
        // heroImage.sprite = Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" + CharacterCSVData.Inst.monsterDataDic[HeroCode.Value].inGameIconName);
        monsterImage.sprite = Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" + CharacterCSVData.Inst.monsterDataDic[BattleSystem.Inst.GetMyState(5).id].inGameIconName);
        attackButtonani.SetBool("isOff", true);
        attackButtonani.SetBool("isOn", false);
        defenceButtonani.SetBool("isOff", true);
        defenceButtonani.SetBool("isOn", false);
        heroTargetBorder.SetActive(false);
        monsterTargetBorder.SetActive(false);

    }

    public void Start()
    {
        if (!BattleSystem.Inst.characterisPlace[5])
        {
            monsterTargetObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AutoCheck();
        }
    }

    public void SetHeroTargetImage(int TargetCode)
    {
        heroTargetObject.SetActive(true);
        heroTargetImage.sprite = Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" + CharacterCSVData.Inst.monsterDataDic[TargetCode].inGameIconName);
        heroTargetBorder.SetActive(false);
        attackButtonani.SetBool("isOff", true);
        attackButtonani.SetBool("isOn", false);
        defenceButtonani.SetBool("isOff", true);
        defenceButtonani.SetBool("isOn", false);
    }

    public void SetMonsterTargetImage(int TargetCode)
    {
        monsterTargetObject.SetActive(true);
        monsterTargetImage.sprite = Resources.Load<Sprite>("BattleUI/Character Portrait Image/Monster/" + CharacterCSVData.Inst.monsterDataDic[TargetCode].inGameIconName);
        monsterTargetBorder.SetActive(false);
        attackButtonani.SetBool("isOff", true);
        attackButtonani.SetBool("isOn", false);
        defenceButtonani.SetBool("isOff", true);
        defenceButtonani.SetBool("isOn", false);
    }

    public void BattleStartAction()
    {
        targetAni.SetTrigger("Off");
        SelectSystem.Inst.actionState = SelectSystem.ActionState.Non;
        attackButtonani.SetBool("isOn", false);
        attackButtonani.SetBool("isOff", true);
        defenceButtonani.SetBool("isOn", false);
        defenceButtonani.SetBool("isOff", true);
        turnEndButtonani.SetBool("isOn", false);
        turnEndButtonani.SetBool("isOff", true);
    }

    public void BattleEndAction()
    {
        targetAni.SetTrigger("On");
        SelectSystem.Inst.actionState = SelectSystem.ActionState.Non;
        turnEndButtonani.SetBool("isOn", true);
        turnEndButtonani.SetBool("isOff", false);
        
        if (BattleSystem.Inst.characterisPlace[5])
        {
            if (BattleSystem.Inst.characterControl[5].nowHp <= 0)
                monsterTargetObject.SetActive(false);
        }
    }

    public void HeroTargetChoice()
    {
        if (SelectSystem.Inst.actionState != SelectSystem.ActionState.HeroSelected)
        {
            if (SelectSystem.Inst.actionState != SelectSystem.ActionState.MonsterSelected)
            {
                monsterTargetBorder.SetActive(false);
            }
            SelectSystem.Inst.actionState = SelectSystem.ActionState.HeroSelected;
            heroTargetBorder.SetActive(true);
            monsterTargetBorder.SetActive(false);
            attackButtonani.SetBool("isOn", true);
            attackButtonani.SetBool("isOff", false);
            defenceButtonani.SetBool("isOn", true);
            defenceButtonani.SetBool("isOff", false);
        }
        else
        {
            SelectSystem.Inst.actionState = SelectSystem.ActionState.Non;
            heroTargetBorder.SetActive(false);
            attackButtonani.SetBool("isOn", false);
            attackButtonani.SetBool("isOff", true);
            defenceButtonani.SetBool("isOn", false);
            defenceButtonani.SetBool("isOff", true);
        }
    }

    public void MonsterTargetChoice()
    {
        if (SelectSystem.Inst.actionState != SelectSystem.ActionState.MonsterSelected)
        {
            SelectSystem.Inst.actionState = SelectSystem.ActionState.MonsterSelected;
            monsterTargetBorder.SetActive(true);
            heroTargetBorder.SetActive(false);
            attackButtonani.SetBool("isOn", true);
            attackButtonani.SetBool("isOff", false);
            defenceButtonani.SetBool("isOn", true);
            defenceButtonani.SetBool("isOff", false);
        }
        else
        {
            SelectSystem.Inst.actionState = SelectSystem.ActionState.Non;
            monsterTargetBorder.SetActive(false);
            attackButtonani.SetBool("isOn", false);
            attackButtonani.SetBool("isOff", true);
            defenceButtonani.SetBool("isOn", false);
            defenceButtonani.SetBool("isOff", true);
        }
    }

    public void ResetTargetImage()
    {
        heroTargetImage.sprite = blank;
        monsterTargetImage.sprite = blank;
    }

    public void BattleLeave()
    {
        // 나가기 버튼
        BattleSystem.Inst.delayImage.SetActive(false);
        PacketManager.Inst.RequestLoginWithScatter();
    }

    public void TimeScaleX10()
    {
        // 배속 올리거나 줄이시려면 Speed를 바꿔주세요!
        int Speed = 10;
        if (BattleSystem.Inst.TimeScale != Speed)
        {
            BattleSystem.Inst.TimeScale = Speed;
            Time.timeScale = Speed;
        }
        else
        {
            BattleSystem.Inst.TimeScale = 1;
            Time.timeScale = 1;
        }
    }

    public void AutoCheck()
    {
        BattleSystem battleSystem = BattleSystem.Inst;

        if (battleSystem.isAuto)
        {
            battleSystem.isAuto = false;
        }
        else
        {
            if (!battleSystem.isBattleStart)
            {
                battleSystem.AutoTargeting();
                battleSystem.TurnEnd();
                battleSystem.isAuto = true;
            }
        }
    }
    
}
