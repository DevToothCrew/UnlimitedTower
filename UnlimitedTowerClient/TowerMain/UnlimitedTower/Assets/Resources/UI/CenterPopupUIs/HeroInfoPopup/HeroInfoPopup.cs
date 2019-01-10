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



    // 서번트 등록된상태, 등록안된상태 
    [Space(15)]
    [Header("DEBUG DATAS")]
    public UserServantData servant;
    public bool registered = false;
    private void OnEnable()
    {
        // 등록 되어있다면, 그 서번트 띄워줌
        if (registered)
        {
            register(servant);
        }
        // 등록 안되어 있다면, 메인서번트를 띄워준다.
        else
        {
            foreach (UserServantData item in UserDataManager.Inst.servantDic.Values)
            {
                if (item.isMainHero == true)
                {
                    register(item);
                    break;
                }
            }
            
        }
    }
    private void OnDisable()
    {
        registered = false;
    }

    //
    public void register(UserServantData servant)
    {
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
        levelText.text = servant.level + "";
        nameText.text = servant.name;


        // 무기칸
        foreach (var mountitem in UserDataManager.Inst.mountitemDic.Values)
        {
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









        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }
        
    }
    
}
