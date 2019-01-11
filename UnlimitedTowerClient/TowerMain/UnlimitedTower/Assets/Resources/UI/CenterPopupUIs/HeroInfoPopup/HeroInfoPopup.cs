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

    [SerializeField] Image WeaponImage;              // 무기칸
    [SerializeField] Image DefenseImage;             // 방어구칸
    [SerializeField] Image AccesoryImage;            // 악세서리칸

    //
    [SerializeField] GameObject CharCamera;
    [SerializeField] Vector3 CharPos;

        
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
        powertext.text = ((int)Etc.instance.getatk(servant)).ToString();
        hptext.text = (int)Etc.instance.getHP(Etc.instance.getstr(servant)) + "";
        deftext.text = ((int)Etc.instance.getDef(Etc.instance.getdex(servant))).ToString();
        criProbtext.text = ((int)Etc.instance.getCriticalProb(Etc.instance.getwis(servant))).ToString();
        criValuetext.text = "";
        Speedtext.text = ErdManager.instance.getServantJobEntityTable_nullPossible(servant.jobNum).speed.ToString();
        strtext.text = ((int)Etc.instance.getstr(servant)).ToString();
        dextext.text = ((int)Etc.instance.getdex(servant)).ToString();
        wistext.text = ((int)Etc.instance.getwis(servant)).ToString();
        
        // 레벨 이름
        levelText.text = "lv." + servant.level;
        nameText.text = servant.name;

        // 캐릭터 카메라
        CharCamera.SetActive(true);
        _CharObj = Instantiate(ErdManager.instance.ServantBodyPrefabs[servant.charNum]);
        _CharObj.transform.position = CharPos;
        _CharObj.transform.eulerAngles = new Vector3(0,-160,0);

        // 무기칸
        foreach (var mountitem in UserDataManager.Inst.mountitemDic.Values)
        {
            // DB에없는 아이템일경우 continue
            MountItemEntity.Param param = ErdManager.instance.getmountitemEntityTable_nullPossible(mountitem.mountitemNum);
            if (param == null)
            {
                continue;
            }

            // 현재 해당서번트가 착용한 무기가맞다면 등록해주기
            if (mountitem.isMounted && mountitem.mountServantIndex == servant.index && param.mountitemType == MountitemType.Weapon)
            {
                // 해당 무기칸 슬롯에다가 이 무기자체를 등록해줘야함.
                WeaponImage.sprite = ErdManager.instance.MountitemSprite[mountitem.mountitemNum];
                break;
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

            // 현재 해당서번트가 착용한 방어구마 맞다면 등록해주기 
            if (mountitem.isMounted && mountitem.mountServantIndex == servant.index && param.mountitemType == MountitemType.Defense)
            {
                // 해당 방어구슬롯자체에 이 방어구를 등록해줘야함.
                WeaponImage.sprite = ErdManager.instance.MountitemSprite[mountitem.mountitemNum];
                break;
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

            // 현재 해당서번트가 착용한 방어구마 맞다면 등록해주기 
            if (mountitem.isMounted && mountitem.mountServantIndex == servant.index && param.mountitemType == MountitemType.Accesory)
            {
                // 해당 악세서리슬롯 자체에 이 악세서리를 등록해줘야함.
                WeaponImage.sprite = ErdManager.instance.MountitemSprite[mountitem.mountitemNum];
                break;
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

        // 캐릭터 카메라
        CharCamera.SetActive(false);
        if (_CharObj != null)
        {
            Destroy(_CharObj);
        }
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
