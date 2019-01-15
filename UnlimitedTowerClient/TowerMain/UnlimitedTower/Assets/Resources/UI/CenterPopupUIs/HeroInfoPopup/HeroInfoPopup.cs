using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPopup : MonoBehaviour {
    

    // UI
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

    //
    [SerializeField] public Heroinfo_InvenPannel heroinfo_invenpannel;

    // 
    [SerializeField] GameObject CharCamera;
    [SerializeField] Vector3 CharPos;

    // 싱글톤 X(어차피 씬에 1개만 존재하는것이 보장된 상태 이므로)
    // Static로 선언해 다른 스크립트에서 참조하기편하게하기위한 코드
    public static HeroInfoPopup instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
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



    [Space(15)]
    [Header("DEBUG DATAS")]
    public UserServantData servant;
    public bool registered = false;
    public GameObject _CharObj;
    public void to_registered(UserServantData servant)
    {
        if (registered)
        {
            return;
        }
        registered = true;

        
        this.servant = servant;

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
        foreach (var mountitem in UserDataManager.Inst.mountitemDic.Values)
        {
            // DB에없는 아이템일경우 continue
            MountItemEntity.Param param = ErdManager.instance.getmountitemEntityTable_nullPossible(mountitem.mountitemNum);
            if (param == null)
            {
                continue;
            }
            
        }

        // 방어구칸
        foreach (var mountitem in UserDataManager.Inst.mountitemDic.Values)
        {
            //
            MountItemEntity.Param param = ErdManager.instance.getmountitemEntityTable_nullPossible(mountitem.mountitemNum);
            if (param == null)
            {
                continue;
            }
            
        }

        // 악세서리칸
        foreach (var mountitem in UserDataManager.Inst.mountitemDic.Values)
        {
            //
            MountItemEntity.Param param = ErdManager.instance.getmountitemEntityTable_nullPossible(mountitem.mountitemNum);
            if (param == null)
            {
                continue;
            }
            
        }
        
    }
    public void to_deregistered()
    {
        if (!registered)
        {
            return;
        }
        registered = false;
        SubCamera.instance.Deregister();
    }


    private void OnEnable()
    {
        if (registered == false)
        {
            UserServantData servantdata = UserDataManager.Inst.ServantList.Find((rowdata) => { return rowdata.isMainHero; });
            to_registered(servantdata);
        }
        
    }
    private void OnDisable()
    {
        to_deregistered();
    }


    
    //
    public void register(UserServantData servant)
    {
        registered = true;
        
    }
    


}
