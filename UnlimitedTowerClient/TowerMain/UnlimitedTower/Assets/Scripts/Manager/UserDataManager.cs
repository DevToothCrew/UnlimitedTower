using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataManager : MonoSingleton<UserDataManager> {

    // TODO : userLoginFlag : 배틀씬에서 단독으로 실행할 때 쓰는 용도
    public bool userLoginFlag;
    public bool createPlayerFlag;
    public SCENE_STATE sceneState = SCENE_STATE.None;

    public UserInfo userInfo = new UserInfo();

    public Dictionary<int, Servant> newServantDic = new Dictionary<int, Servant>();
    public Dictionary<int, Monster> newMonsterDic = new Dictionary<int, Monster>();
    public Dictionary<int, Item>    itemDic       = new Dictionary<int, Item>();

    // 포메이션 : 캐릭터 인덱스 ->가 들어가는 딕셔너리
    public Dictionary<int, int> formationDic = new Dictionary<int, int>();


    //add by canie
    public Dictionary<int, Party> partyDic = new Dictionary<int, Party>();

    //TODO :현재 활성화된 파티 인덱스 : 1 부터 시작한다.
    // 현재는 테스트 용으로 1을 사용한다.
    public int usingPartyNum = 1;

    // 포메이션 들어가는 순서
    public List<int> formationOrderList = new List<int>()
    {
      7, 6, 8, 5, 9, 2, 1, 3, 0, 4 
    };


    // TODO : 테스트 서번트+몬스터 수
    public readonly int  TestCharNum = 10;
    private bool testInitFlag = false;


    public void Awake()
    {
        InitFlag();
        // TODO : 확실히 필요없다고 판단되면 삭제할것
        //LobbyBackGround.SetActive(true);
        //StageBackGround.SetActive(false);
        //StageList.SetActive(false);
    }



    #region  Editor
    public void Test_InitCharacter()
    {
        if (testInitFlag == false)
        {
            //TODO :  나의 캐릭터(무조건 존재하는 값)이라고 가정
            //userInfo.userHero = new Servant();
            CreateHero();
            for (int i = 0; i < TestCharNum; i++)
            {
                CreateServant(i + 1);
                CreateMonster(i + 1);
            }
            testInitFlag = true;

            partyDic[usingPartyNum] = new Party();
            for(int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
            {
                partyDic[usingPartyNum].characterList[i] = new PartyCharacterInfo(formationOrderList[i], 0, 0);
            }

        }
    }


    public Servant CreateServant(int getIndex)
    {
       Servant servant = new Servant();
       servant.index = getIndex;
       newServantDic.Add(getIndex, servant);
       AddServantImage(servant);

        return servant;
    }
    public Monster CreateMonster(int getIndex)
    {
        Monster monster = new Monster();
        monster.index = getIndex;
        newMonsterDic.Add(getIndex, monster);
        AddMonsterImage(monster);

        return monster;
    }
    public int GetMonsterCount()
    {
        return newMonsterDic.Count;
    }
    public int GetServantCount()
    {
        return newServantDic.Count;
    }
    #endregion
    public void InitFlag()
    {
        Debug.Log("InitFlag");
        userLoginFlag = false;
        createPlayerFlag = false;
 
    }
    private void CreateHero()
    {
        if(formationDic.ContainsKey(DEFINE.HERO_FORMATION_NUM) == false)
        {
            // 히어로의 인덱스는 0~2
            formationDic.Add(DEFINE.HERO_FORMATION_NUM, 0);
            string path = "UI/CharaterImage/" + userInfo.userHero.name;
            LoadCharImage(path, DEFINE.HERO_FORMATION_NUM, null);
        }
    }

    private void LoadCharImage(string imageFath, int deckNum, GameObject original)
    {
        //GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
        GameObject deck = FormationManager.Inst.Decks[deckNum];
        Sprite sprite = Resources.Load<Sprite>(imageFath);

        if (deck.GetComponent<FormationDeck>())
        {
            deck.GetComponent<FormationDeck>().LinkedChar = original;
            deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;


            FormationManager.Inst.DeckTexts[deckNum].SetActive(false);
            if(original == null)
            {
                Debug.Log("히로 로딩 완료");
            }
            //deck.GetComponent<FormationDeck>().ShowEmptyText(false);
        }
        else
        {
            Debug.Log("Do not exist FormatiionDeck Component");
        }    
    }

    public void SetUserLoginFlag(bool flag)
    {
        userLoginFlag = flag;
    }

    public void SetCreatePlayerFlag(bool flag)
    {
        createPlayerFlag = flag;
    }

    public void  LoadUserData()
    {
        //string path = "UI/CharaterImage/" + heroChar.Name;
        //LoadCharImage(path, DEFINE.HERO_FORMATION_NUM, null);
        //LoadCharData(CharacterListManager.Inst.ServantContentList, ref servantDic, CHAR_TYPE.SERVANT);
        //LoadCharData(CharacterListManager.Inst.MonsterContentList, ref monsterDic, CHAR_TYPE.MONSTER);
    }


    public void LoadCharData(GameObject charContentList,
        ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    {
        string imageFath = null;
        if(charType == CHAR_TYPE.SERVANT)
        {
            imageFath = "UI/CharaterImage/";
        }
        else
        {
            imageFath = "UI/MonsterImage/";
        }

        // 캐릭터 개수만큼 캐릭터 목록을 다시 불러온다.
        foreach (KeyValuePair<int, Character> dic in charDic)
        {          
            var instance = Instantiate(Resources.Load("Prefabs/CharContent") as GameObject);
            // 이걸 이미지로 검사해야하는가?
            if (instance.transform.GetChild(0).GetComponent<Image>())
            {         
                instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(imageFath + charDic[dic.Key].Name);
                instance.transform.SetParent(charContentList.transform.transform);
                instance.GetComponent<CharContent>().charDicKey = dic.Key;
                if (charType == CHAR_TYPE.SERVANT)
                {
                    instance.GetComponent<CharContent>().charType = CHAR_TYPE.SERVANT;
                }
                else
                {
                    instance.GetComponent<CharContent>().charType = CHAR_TYPE.MONSTER;
                }

                if (charDic[dic.Key].OnFormation)
                {
                    Color color = instance.transform.GetChild(0).GetComponent<Image>().color;
                    color.r = color.g = color.b = 0.35f;
                    instance.transform.GetChild(0).GetComponent<Image>().color = color;
                    instance.transform.GetChild(1).gameObject.SetActive(true);

                    //  포메이션 세팅.
                    if (charDic[dic.Key].FormationIndex != -1)
                    {
                        int deckNum = charDic[dic.Key].FormationIndex;
                        LoadCharImage(imageFath + charDic[dic.Key].Name, deckNum, instance);                 
                    }
                }
            }
        }
    }


    public void AddServantImage(Servant getServant)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/CharContent") as GameObject);

        if (instance.transform.GetChild(0))
        {
            instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + getServant.name);
            instance.GetComponent<CharContent>().charDicKey = getServant.index;
            instance.transform.SetParent(CharacterListManager.Inst.ServantContentList.transform.transform);
            instance.GetComponent<CharContent>().charType = CHAR_TYPE.SERVANT;

        }
    }
    public void AddMonsterImage(Monster getMonster)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/CharContent") as GameObject);

        if (instance.transform.GetChild(0))
        {
            instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/MonsterImage/" + getMonster.name);
            instance.GetComponent<CharContent>().charDicKey = getMonster.index;
            //instance.transform.SetParent(CharacterListManager.Inst.MonsterContentList.transform.transform);
            instance.transform.SetParent(CharacterListManager.Inst.ServantContentList.transform.transform);
            instance.GetComponent<CharContent>().charType = CHAR_TYPE.MONSTER;
        }    
    }





#region NewGetData Func


    public void Login(UserLoginData getUserLoginData)
    {
        if (ParseUserInfo(getUserLoginData.userinfo) == false)
        {
            Debug.Log("Invalid ParseUserInfo Info");
            // 재 로그인 시켜야함
        }

        if (ParseServantList(getUserLoginData.servant_list) == false)
        {
            Debug.Log("Invalid ParseServantList Info");
            // 재 로그인 시켜야함
        }

        if (ParseMonsterList(getUserLoginData.monster_list) == false)
        {
            Debug.Log("Invalid ParseMonsterList Info");
            // 재 로그인 시켜야함
        }

        if (ParseItemList(getUserLoginData.item_list) == false)
        {
            Debug.Log("Invalid ParseItemList Info");
            // 재 로그인 시켜야함
        }

        //add by canie
        if (ParsePartyList(getUserLoginData.party_list) ==  false)
        {
            Debug.Log("invalid ParsePartyList info");
            // 재 로그인 시켜야함
        }

        // TODO : Party 편성 정보도 추가
        formationDic = new Dictionary<int, int>();
        formationDic.Add(DEFINE.HERO_FORMATION_NUM, 0);

        string path = "UI/CharaterImage/" + userInfo.userHero.name;
        LoadCharImage(path, DEFINE.HERO_FORMATION_NUM, null);
    }

    public bool ParseUserInfo(userData getUserData)
    {
        userInfo = new UserInfo();

        userInfo.userName = getUserData.user;
        userInfo.userEOS = 0;
        // TODO : EOS는 따로 구하는 코드 추가 필요?
        // userInfo.userEOS = ??
        userInfo.userMoney = getUserData.game_money;
        sceneState = (SCENE_STATE)getUserData.state;

        userInfo.userHero = ParseServant(0, getUserData.hero);
        if (userInfo.userHero == null)
        {
            Debug.Log("Invalid UserHero Info");
            return false;
        }

        return true;
    }

    public bool ParseServantList(List<servantData> getServantList)
    {
        newServantDic = new Dictionary<int, Servant>();
        
        for (int i = 0; i < getServantList.Count; i++)
        {
            Servant servant = ParseServant(getServantList[i].index, getServantList[i].servant);
            if(servant == null)
            {
                Debug.Log("Invalid Servant Info");
                return false;
            }

            newServantDic.Add(servant.index, servant);
            //TODO : 이미지 추가할 수 있는 적당한 곳이 생기면 옮길것.
            AddServantImage(servant);
        }

        return true;
    }
    
    public Servant ParseServant(int getServantIndex, servantInfo getServantInfo)
    {
        if (getServantInfo == null)
        {
            return null;
        }

        Servant servant = new Servant();

        servant.job         = getServantInfo.job;

        servant.index       = getServantIndex;
        servant.state       = getServantInfo.state;
        servant.exp         = getServantInfo.exp;
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        servant.level        = DEFINE.GetLevelForExp(getServantInfo.exp);
        servant.job          = getServantInfo.job;

        // TODO : 추후 Appear의 값에 따른 리소스가 저장되어야함
        servant.head        = getServantInfo.appear.head;
        servant.hair        = getServantInfo.appear.hair;
        servant.body        = getServantInfo.appear.body;

        servant.status      = ParseStatus(getServantInfo.status);

        //TODO : 임시 코드 
        CHARACTER_NUM servantNum = servant.job + CHARACTER_NUM.Hero_Novice_1001;
        servant.name        = servantNum.ToString();

        if (servant.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        servant.equipmentList = getServantInfo.equip_slot;

        return servant;
    }

    public Status ParseStatus(statusInfo getStatusInfo)
    {
        if(getStatusInfo == null)
        {
            return null;
        }

        Status status = new Status();

        status.basicStr = getStatusInfo.basic_str;
        status.basicDex = getStatusInfo.basic_dex;
        status.basicInt = getStatusInfo.basic_int;

        status.plusStr = getStatusInfo.plus_str;
        status.plusDex = getStatusInfo.plus_dex;
        status.plusInt = getStatusInfo.plus_int;

        return status;
    }

    public bool ParseMonsterList(List<monsterData> getMonsterList)
    {
        newMonsterDic = new Dictionary<int, Monster>();

        for(int i = 0; i < getMonsterList.Count; i++)
        {
            Monster monster = ParseMonster(getMonsterList[i].index, getMonsterList[i].monster);
            if(monster == null)
            {
                Debug.Log("Invalid Monster Info");
                return false;
            }

            newMonsterDic.Add(monster.index, monster);
            //TODO : 이미지 추가할 수 있는 적당한 곳이 생기면 옮길것.
            AddMonsterImage(monster);
        }

        return true;
    }

    public Monster ParseMonster(int getMonsterIndex, monsterInfo getMonsterInfo)
    {
        if (getMonsterInfo == null)
        {
            return null;
        }

        Monster monster = new Monster();

        monster.look        = getMonsterInfo.look;

        monster.index       = getMonsterIndex;
        monster.state       = getMonsterInfo.state;
        monster.exp = getMonsterInfo.exp;
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        monster.level       = DEFINE.GetLevelForExp(getMonsterInfo.exp);
        monster.type        = getMonsterInfo.type;
        monster.grade       = getMonsterInfo.grade;
        monster.upgrade     = getMonsterInfo.upgrade;

        // TODO : 추후 Appear + Type 의 값에 따른 리소스가 저장되어야함
        // monster.appear = ???

        monster.status      = ParseStatus(getMonsterInfo.status);

        //TODO : 임시 코드 
        CHARACTER_NUM monsterNum = monster.look + CHARACTER_NUM.Mst_Cat;
        monster.name = monsterNum.ToString();

        if (monster.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        return monster;
    }

    public bool ParseItemList(List<itemData> getItemList)
    {
        itemDic = new Dictionary<int, Item>();
        
        for(int i=0; i<getItemList.Count; i++)
        {
            Item item = ParseItem(getItemList[i].index, getItemList[i].item);
            if(item == null)
            {
                Debug.Log("Invalid Item Info");
                return false;
            }
            itemDic.Add(item.index, item);
        }

        return true;
    }

    public Item ParseItem(int getItemIndex, itemInfo getItemInfo)
    {
        if(getItemInfo == null)
        {
            return null;
        }

        Item item = new Item();

        item.index = getItemIndex;

        item.state      = getItemInfo.state;
        item.id         = getItemInfo.id;
        item.slot       = getItemInfo.slot;
        item.tier       = getItemInfo.tier;
        item.job        = getItemInfo.job;
        item.grade      = getItemInfo.grade;
        item.upgrade    = getItemInfo.upgrade;
        item.atk        = getItemInfo.atk;
        item.def        = getItemInfo.def;

        item.status = ParseStatus(getItemInfo.status);
        if(item.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        return item;   
    }
    //add by canie
    public bool ParsePartyList(List<partyData> getPartyList)
    {
        partyDic = new Dictionary<int, Party>();

        Debug.Log("ParsePartyList : " + getPartyList.Count);

        for (int i = 0; i < getPartyList.Count; i++)
        {
            Debug.Log("getPartyList : " + getPartyList[i].index);
            Party party = ParseParty(getPartyList[i].index, getPartyList[i]);
            if (party == null)
            {
                Debug.Log("Invalid party Info");
                return false;
            }
            partyDic.Add(party.partyIndex, party);
            Debug.Log("party.partyIndex : " + party.partyIndex);
        }
        return true;
    }

    public Party ParseParty(int getPartyIndex,partyData getParty)
    {
        if (getParty == null)
        {
            return null;
        }
        Party party = new Party();
        party.partyIndex = getPartyIndex;
        party.state = getParty.state;

        Debug.Log("party Count : " + getParty.party.Count);

        for (int i=0; i< getParty.party.Count; ++i)
        {
            PartyCharacterInfo partyInfo = new PartyCharacterInfo();

            Debug.Log("partyInfo : " + partyInfo.type + ", " + partyInfo.index);

            if (i == 0 )
            {
                partyInfo.type = (int)CHAR_TYPE.HERO;
            }
            else if ( i < 5 )
            {
                partyInfo.type = (int)CHAR_TYPE.SERVANT;
            }
            else
            {
                partyInfo.type = (int)CHAR_TYPE.MONSTER;
            }

            partyInfo.partyLocation = formationOrderList[i];
            partyInfo.index = getParty.party[i];

            party.characterList.Add(i, partyInfo);
        }
        return party;
    }

    // TODO : 가챠에서 받아오는 목록
    public void SetServant(Servant getServant)
    {
        if(newServantDic.ContainsKey(getServant.index) == false)
        {
            newServantDic.Add(getServant.index, getServant);
        }
        else
        {
            Debug.Log("SetServant : being servert");
        }
    }
    public void SetMonster(Monster getMonster)
    {
        if (newServantDic.ContainsKey(getMonster.index) == false)
        {
            newMonsterDic.Add(getMonster.index, getMonster);
        }
        else
        {
            Debug.Log("SetMonster : being monster");
        }
    }
    public void SetItem(Item getItem)
    {
        if (itemDic.ContainsKey(getItem.index) == false)
        {
            itemDic.Add(getItem.index, getItem);
        }
        else
        {
            Debug.Log("SetItem : being item");
        }
    }

    #endregion

}



