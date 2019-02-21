using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPopup : MonoBehaviour {

    // static화 하기(싱글톤NO)
    public static StageInfoPopup instance;
    private void Awake()
    {
        instance = this;
    }

    // 이미지 슬롯들
    public List<stageui_charslot> charslotList;

    // 캐릭터윈도우
    public Text charLv;
    public Text charName;
    public Image charJobImg;

    public List<StagewindowItemSlot> stagewindowItemList; 

    public Text STR;
    public Text DEX;
    public Text INT;
    public Text ATK;
    public Text DEF;
    public Text SPD;
    public Text CRIPERCENT;
    public Text CRIDMG;

    public Image weaponIamage;


    // FSM //

    // 현재 디스플레이되고있는 팀넘버 
    public int curDisplayTeam;
    public void SetDisplayTeamnum(int teamNum)
    {
        // 캐릭터 창 업데이트
        for (int i = 0; i < charslotList.Count; i++)
        {
            int formationIndex = i;

            // 배치되어있다면, 이미지로 띄워주기
            // 서번트
            if (formationIndex <= 4)
            {
                if (UserDataManager.Inst.GetServantIsPlaced(formationIndex) == true)
                {
                    UserServantData servantdata = GameDataManager.instance.getServantPlacedAt_nullPossible(curDisplayTeam, formationIndex);
                    charslotList[i].ToServant(servantdata);
                }
                else
                {
                    charslotList[i].ToEmpty();
                }
            }
            // 몬스터
            else
            {
                if (UserDataManager.Inst.GetMonsterIsPlaced(formationIndex) == true)
                {
                    UserMonsterData servantdata = GameDataManager.instance.getMonsterPlacedAt_nullPossible(curDisplayTeam, formationIndex);
                    charslotList[i].ToMonster(servantdata);
                }
                else
                {
                    charslotList[i].ToEmpty();
                }
            }
        }

        // 메인서번트 정보 띄우기
        SetFormationNum(Etc.MainServantFormationNum);
    }
    
    // 현재 팀에서 선택된 자리넘버
    public int selectedFormationNum;
    public void SetFormationNum(int formNum)
    {
        // 일단 모두 디셀렉트
        DeselectAll();

        // 아니 왜 어디는 몬스터먼저고 어디는 서번트먼저고 그리고 참조하는 값도 다 다르고 이게뭐야

        // 해당칸이 어딘지에 따라 -> 몬스터
        if (formNum >= 5)
        {
            if (UserDataManager.Inst.GetMonsterIsPlaced(formNum) == false)
            {
                return;
            }

            UserMonsterData monsterdata = GameDataManager.instance.getMonsterPlacedAt_nullPossible(curDisplayTeam, formNum);
            UpdateCharacterWindow(monsterdata);
        }
        // -> 서번트
        else
        {
            if (UserDataManager.Inst.GetServantIsPlaced(formNum) == false)
            {
                return;
            }

            UserServantData servantdata = GameDataManager.instance.getServantPlacedAt_nullPossible(curDisplayTeam, formNum);
            UpdateCharacterWindow(servantdata);
        }

        charslotList[formNum].selectedObj.SetActive(true);
    }
    public void UpdateCharacterWindow(UserServantData servantdata)
    {

        STR.text = Etc.instance.Getstr(servantdata).ToString();
        DEX.text = Etc.instance.Getdex(servantdata).ToString();
        INT.text = Etc.instance.Getint(servantdata).ToString();
        ATK.text = Etc.instance.Getatk(servantdata).ToString();
        DEF.text = Etc.instance.GetDef(servantdata).ToString();
        SPD.text = Etc.instance.GetSpd(servantdata).ToString();
        CRIPERCENT.text = Etc.instance.GetCriticalProb(servantdata).ToString();
        CRIDMG.text = Etc.instance.Getstr(servantdata).ToString();

        // 캐릭터 프리팹
        SubCamera.instance.Register(servantdata);

        // 아이템칸 켜기
        foreach (var item in stagewindowItemList)
        {
            item.gameObject.SetActive(true);
        }


        /* 착용중인 아이템 등록 */
        foreach (var item in stagewindowItemList)
        {
            item.Deregister();
        }
        // 아이템창 초기화
        List<UserMountItemData> mountitemList = UserDataManager.Inst.GetMountItemList().FindAll((rowdata) => { return rowdata.isMounted && rowdata.mountServantIndex == servantdata.index; });
        // 아이템창 등록
        foreach (var item in mountitemList)
        {
            MountItemEntity.Param param = ErdManager.instance.getmountitemEntityTable_nullPossible(item.mountitemNum);
            switch (param.mountitemType)
            {
                case MountitemType.Weapon:
                    stagewindowItemList[0].RegisterItem(item);
                    break;

                case MountitemType.Defense:
                    stagewindowItemList[1].RegisterItem(item);
                    break;
                    
                case MountitemType.Accessory:
                    stagewindowItemList[2].RegisterItem(item);
                    break;
            }
        }
    }
    public void UpdateCharacterWindow(UserMonsterData monsterdata)
    {
        STR.text = Etc.instance.Getstr(monsterdata).ToString();
        DEX.text = Etc.instance.Getdex(monsterdata).ToString();
        INT.text = Etc.instance.Getint(monsterdata).ToString();
        ATK.text = Etc.instance.Getatk(monsterdata).ToString();
        DEF.text = Etc.instance.GetDef(monsterdata).ToString();
        SPD.text = Etc.instance.GetSpd(monsterdata).ToString();
        CRIPERCENT.text = Etc.instance.GetCriticalProb(monsterdata).ToString();
        CRIDMG.text = Etc.instance.Getstr(monsterdata).ToString();

        // 프리팹
        SubCamera.instance.Register(monsterdata);

        // 아이템칸 끄기
        foreach (var item in stagewindowItemList)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void DeselectAll()
    {
        foreach (stageui_charslot item in charslotList)
        {
            item.selectedObj.SetActive(false);
        }
    }



    private void OnEnable()
    {
        SetDisplayTeamnum(curDisplayTeam);
    }
    public void TeambtnOnclick(int teamNum)
    {
        this.curDisplayTeam = teamNum;
        SetDisplayTeamnum(curDisplayTeam);
    }


    
}
