using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPopup : MonoBehaviour {

    /// <summary>
    /// HeroInfoPopup 윈도우 스크립트 입니다. (인스펙터에 HeroInfoPopup 검색시 어떤 오브젝트에 붙어있는것인지 나옴)
    /// 
    /// STATE 1: Registered
    ///     weaponSlot,armorSlot,accessorySlot이 현재 해당윈도우에 연결된 servantdata의 mountItemListChangeEvent에 콜백을 등록해놓은 상태.
    ///     registered == true, servantdata에 UserServantData가 입력된 상태
    ///     
    /// STATE 2. DeRegistered
    ///      UI가 꺼진 상태
    ///      registered == false
    /// </summary>

    // 스탯 UI들
    [SerializeField] Text powertext;        // 공격력
    [SerializeField] Text hptext;           // 생명력
    [SerializeField] Text deftext;          // 방어력
    [SerializeField] Text criProbtext;      // 치명확률
    [SerializeField] Text criValuetext;     // 치명피해
    [SerializeField] Text Speedtext;         // 속도
    [SerializeField] Text strtext;            // 힘
    [SerializeField] Text dextext;            // 민첩
    [SerializeField] Text wistext;            // 지능

    [SerializeField] Text levelText;           // 레벨
    [SerializeField] Text nameText;            // 이름

    // 무기,방어구,악세서리 슬롯
    [SerializeField] HeroinfoItemSlot weaponSlot;
    [SerializeField] HeroinfoItemSlot armorSlot;
    [SerializeField] HeroinfoItemSlot accessorySlot;

    // 인벤토리 창
    [SerializeField] public Heroinfo_InvenPannel heroinfo_invenpannel;
    public void SetHeroinfoPannel(bool active)
    {
        heroinfo_invenpannel.gameObject.SetActive(active);
    }

    // 싱글톤
    public static HeroInfoPopup instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }
        
    }
    private void OnDestroy()
    {
        instance = null;
    }


    
    private void OnDisable()
    {
        ToDeregistered();
    }


    
    // FSM 이동 함수
    // 상태 1: 활성화 상태
    // 상태 2: 비활성화 상태
    [Space(15)]
    [Header("DEBUG DATAS")]
    public UserServantData servantData;
    public UserMonsterData monsterData;
    public UNIT_TYPE unitType;
    public bool isRegistered = false;
    public void ToRegistered(UserServantData servant)
    {
        if (isRegistered)
        {
            return;
        }
        isRegistered = true;
        this.servantData = servant;

        // 능력치 창
        powertext.text = ((int)Etc.instance.Getatk(servant)).ToString();
        hptext.text = (int)Etc.instance.GetHP(servant) + "";
        deftext.text = ((int)Etc.instance.GetDef(servant)).ToString();
        criProbtext.text = ((int)Etc.instance.GetCriticalProb(servant)).ToString();
        criValuetext.text = "";
        Speedtext.text = ErdManager.instance.getServantJobEntityTable_nullPossible(servant.jobNum).speed.ToString();
        strtext.text = ((int)Etc.instance.Getstr(servant)).ToString();
        dextext.text = ((int)Etc.instance.Getdex(servant)).ToString();
        wistext.text = ((int)Etc.instance.Getint(servant)).ToString();
        
        // 레벨 이름
        levelText.text = "lv." + servant.level;
        nameText.text = servant.name;

        // 캐릭터프리팹
        SubCamera.instance.Register(servant);

        // 무기칸
        weaponSlot.ToRegister(servant);

        // 방어구칸
        armorSlot.ToRegister(servant);

        // 악세서리칸
        accessorySlot.ToRegister(servant);

        gameObject.SetActive(true);
    }
    public void ToRegistered(UserMonsterData monster)
    {
        if (isRegistered)
        {
            return;
        }
        isRegistered = true;
        this.monsterData = monster;

        // 능력치 창
        powertext.text = ((int)Etc.instance.Getatk(monsterData)).ToString();
        hptext.text = (int)Etc.instance.GetHP(monsterData) + "";
        deftext.text = ((int)Etc.instance.GetDef(monsterData)).ToString();
        criProbtext.text = ((int)Etc.instance.GetCriticalProb(monsterData)).ToString();
        criValuetext.text = "";
        //Speedtext.text = ErdManager.instance.getServantJobEntityTable_nullPossible(monsterData.jobNum).speed.ToString();
        strtext.text = ((int)Etc.instance.Getstr(monsterData)).ToString();
        dextext.text = ((int)Etc.instance.Getdex(monsterData)).ToString();
        wistext.text = ((int)Etc.instance.Getint(monsterData)).ToString();

        // 레벨 이름
        levelText.text = "lv." + monsterData.level;
        nameText.text = monsterData.name;

        // 캐릭터프리팹
        SubCamera.instance.Register(monsterData);

        gameObject.SetActive(true);
    }
    public void ToDeregistered()
    {
        if (!isRegistered)
        {
            return;
        }
        isRegistered = false;

        switch (unitType)
        {
            case UNIT_TYPE.SERVANT:
                {

                }
                break;
            case UNIT_TYPE.MONSTER:
                {

                }
                break;
        }

        SubCamera.instance.Deregister();
    }


    
    


}
