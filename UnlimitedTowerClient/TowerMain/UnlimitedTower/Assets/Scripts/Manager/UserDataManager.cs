﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataManager : MonoSingleton<UserDataManager> {

    // TODO : Test Flag
    public bool UserLoginFlag;
    public bool CreatePlayerFlag;
    public SCENE_STATE sceneState = SCENE_STATE.None;

    public Character heroChar;
    public Dictionary<int, Character> servantDic = new Dictionary<int, Character>();
    public Dictionary<int, Character> monsterDic = new Dictionary<int, Character>();

    public UserInfo userInfo = new UserInfo();

    public Dictionary<int, Servant> newServantDic = new Dictionary<int, Servant>();
    public Dictionary<int, Monster> newMonsterDic = new Dictionary<int, Monster>();
    public Dictionary<int, Item>    itemDic       = new Dictionary<int, Item>();

    //public Dictionary<int, int> itemDic = new 




    // 포메이션 : 캐릭터 인덱스 ->가 들어가는 딕셔너리
    public Dictionary<int, int> formationDic = new Dictionary<int, int>();

    public Dictionary<int, int> oldFormationDic = new Dictionary<int, int>();

    // 유저의 캐릭터 키값들 담는 리스트
    public List<int> userCharsKeyList = new List<int>();

    // 포메이션 들어가는 순서
    public List<int> formationOrderList = new List<int>()
    {
      7, 6, 8, 5, 9, 2, 1, 3, 0, 4 
    };

    public int TestCharNum = 20;

    private bool testInitFlag = false;


    // TODO : 확실히 필요없다고 판단되면 삭제할것
    //public GameObject LobbyBackGround;
    //public GameObject StageBackGround;
    // public GameObject StageList;

    public int servantIndex = 0;
    public int monsterIndex = 0;

    public void Awake()
    {
        InitFlag();
        // TODO : 확실히 필요없다고 판단되면 삭제할것
        //LobbyBackGround.SetActive(true);
        //StageBackGround.SetActive(false);
        //StageList.SetActive(false);
    }
    public void Test_InitCharacter()
    {
        if(testInitFlag == false)
        {
            //TODO :  나의 캐릭터(무조건 존재하는 값)이라고 가정
            heroChar = new Character(CHAR_TYPE.SERVANT);
            CreateHero();
            for (int i = 0; i < TestCharNum; i++)
            {
                CreateServant();
                CreateMonster();
            }
            Create10NumberMonster();
            testInitFlag = true;
            oldFormationDic = formationDic;
        }  
    }
    void CreateServant()
    {
        Character newChar = new Character(UserDataManager.Inst.GetCharacterIndex(), GACHA_TYPE.Servant);
        SetServant(newChar);
        AddNewCharImage(newChar, CHAR_TYPE.SERVANT);

    }
    void CreateMonster()
    {
        Character newChar = new Character(UserDataManager.Inst.GetMonsterIndex(), GACHA_TYPE.Monster);
        SetMonster(newChar);
        AddNewCharImage(newChar, CHAR_TYPE.MONSTER);
    }
    void Create10NumberMonster()
    {
        Character newChar;
        for (int i = 0; i < 3; i++)
        {
            newChar = new Character(CHARACTER_NUM.Mst_Death);
            SetMonster(newChar);
            AddNewCharImage(newChar, CHAR_TYPE.MONSTER);
        }
        
        for (int i = 3; i < 6; i++)
        {
            newChar = new Character(CHARACTER_NUM.Mst_Robot);
            SetMonster(newChar);
            AddNewCharImage(newChar, CHAR_TYPE.MONSTER);
        }
     
        for (int i = 6; i < 10; i++)
        {
            newChar = new Character(CHARACTER_NUM.Mst_ShadowCat);
            SetMonster(newChar);
            AddNewCharImage(newChar, CHAR_TYPE.MONSTER);
        }


    }

    public void InitFlag()
    {
        Debug.Log("InitFlag");
        UserLoginFlag = false;
        CreatePlayerFlag = false;
 
    }
    private void CreateHero()
    {
        if(formationDic.ContainsKey(DEFINE.HERO_FORMATION_NUM) == false)
        {
            // 히어로의 인덱스는 0~2
            formationDic.Add(DEFINE.HERO_FORMATION_NUM, 0);
            string path = "UI/CharaterImage/" + heroChar.Name;
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
    // 저장을 안했기 때문에 히어로 혼자 플레이한다.
    private void ResetFormation()
    {

    }
    public void SetOldFormation()
    {
        oldFormationDic = formationDic;
    }

    public void SetUserLoginFlag(bool flag)
    {
        UserLoginFlag = flag;
    }

    public void SetCreatePlayerFlag(bool flag)
    {
        CreatePlayerFlag = flag;
    }

    public bool CheckEnterLobby()
    {
        // TEST
        //if(UserLoginFlag == true && CreatePlayerFlag == true)
        //{
        //    return true;
        //}

        //return false;

        return true;
    }


   
    public void ChangeSceneState(SCENE_STATE state)
    {
        sceneState = state;

        //TODO : Test Code
        //switch (state)
        //{
        //    case SCENE_STATE.Lobby:
        //        StageBackGround.SetActive(false);
        //        StageList.SetActive(false);
        //        LobbyBackGround.SetActive(true);
        //        break;
        //    case SCENE_STATE.Stage:
        //        LobbyBackGround.SetActive(false);
        //        StageBackGround.SetActive(true);
        //        StageList.SetActive(true);
        //        break;
        //}
    }

    public int GetCharacterIndex()
    {
        return servantIndex;
    }
    public int GetMonsterIndex()
    {
        return monsterIndex;
    }


    // TODO : Test Code if deleted
    public void SetChar(Dictionary<int, Character> getCharcterDic)
    {
        servantDic = getCharcterDic;
    }

    // 새로운 캐릭터를 dic에 저장한다.
    public void SetServant(Character newChar)
    {
#if UNITY_EDITOR
        servantDic.Add(servantIndex, newChar);
        Debug.Log("서번트 인덱스 : " + servantIndex);
#else
         servantDic.Add((int)newChar.UniqueIndex-1, newChar);
#endif
        servantIndex += 1;
    }

    public void SetMonster(Character newChar)
    {
#if UNITY_EDITOR
        monsterDic.Add(monsterIndex, newChar);
        Debug.Log("몬스터 인덱스 : " + monsterIndex);
#else
          monsterDic.Add((int)newChar.UniqueIndex - 1, newChar);
#endif
        monsterIndex += 1;
    }

    public void  LoadUserData()
    {
        string path = "UI/CharaterImage/" + heroChar.Name;
        LoadCharImage(path, DEFINE.HERO_FORMATION_NUM, null);
        LoadCharData(CharacterListManager.Inst.ServantContentList, ref servantDic, CHAR_TYPE.SERVANT);
        LoadCharData(CharacterListManager.Inst.MonsterContentList, ref monsterDic, CHAR_TYPE.MONSTER);
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
                instance.GetComponent<CharContent>().CharDicKey = dic.Key;
                if (charType == CHAR_TYPE.SERVANT)
                {
                    instance.GetComponent<CharContent>().CharType = CHAR_TYPE.SERVANT;
                }
                else
                {
                    instance.GetComponent<CharContent>().CharType = CHAR_TYPE.MONSTER;
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




    public void AddNewCharImage(Character newChar, CHAR_TYPE charType)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/CharContent") as GameObject);

        if (instance.transform.GetChild(0))
        {
            if (charType == CHAR_TYPE.SERVANT)
            {
                instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + newChar.Name);
                //instance.GetComponent<CharContent>().CharDicKey = servantIndex - 1;
                instance.GetComponent<CharContent>().CharDicKey = (int)newChar.UniqueIndex;
                instance.transform.SetParent(CharacterListManager.Inst.ServantContentList.transform.transform);
                //instance.transform.SetParent(LobbyManager.Inst.ServantContentList.transform.transform);
                instance.GetComponent<CharContent>().CharType = CHAR_TYPE.SERVANT;
            }
            else
            {
                instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/MonsterImage/" + newChar.Name);
                //instance.GetComponent<CharContent>().CharDicKey = monsterIndex - 1;
                instance.GetComponent<CharContent>().CharDicKey = (int)newChar.UniqueIndex;
                instance.transform.SetParent(CharacterListManager.Inst.MonsterContentList.transform.transform); 
                 //instance.transform.SetParent(LobbyManager.Inst.MonsterContentList.transform.transform);
                 instance.GetComponent<CharContent>().CharType = CHAR_TYPE.MONSTER;
            }          
        } 
    }

    public void RemoveUserInfo()
    {
        Debug.Log("Remove UserInfo");
        servantDic.Clear();
        servantIndex = 0;
        monsterDic.Clear();
        monsterIndex = 0;

          //userInfo 
         //   heroChar;

    userCharsKeyList.Clear();
 
    }





#region NewGetData Func


    public void Login(UserLoginData userLoginData)
    {

        if (ParseUserInfo(userLoginData.userinfo) == false)
        {
            Debug.Log("Invalid ParseUserInfo Info");
            // 재 로그인 시켜야함
        }

        if (ParseServantList(userLoginData.servant_list) == false)
        {
            Debug.Log("Invalid ParseServantList Info");
            // 재 로그인 시켜야함
        }
        // TODO : 작업 예정
        if (ParseItemList(userLoginData.item_list) == false)
        {
            Debug.Log("Invalid ParseItemList Info");
            // 재 로그인 시켜야함
        }
        if (ParseMonsterList(userLoginData.monster_list) == false)
        {
            Debug.Log("Invalid ParseMonsterList Info");
        }

     

        // TODO : Party 편성 정보도 추가
        formationDic.Add(DEFINE.HERO_FORMATION_NUM, 0);
        string path = "UI/CharaterImage/" + heroChar.Name;
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
        }

        return true;
    }
    
    public Servant ParseServant(int getServantIndex, servantInfo getServantInfo)
    {
        Servant servant = new Servant();

        servant.index = getServantIndex;
        servant.state = getServantInfo.state;
        servant.exp = getServantInfo.exp;
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        servant.level = DEFINE.GetLevelForExp(getServantInfo.exp);
        servant.job = getServantInfo.job;

        // TODO : 추후 Appear의 값에 따른 리소스가 저장되어야함
        servant.head = getServantInfo.appear.head;
        servant.hair = getServantInfo.appear.hair;
        servant.body = getServantInfo.appear.body;

        servant.status = ParseStatus(getServantInfo.status);
        if(servant.status == null)
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
        }

        return true;
    }

    public Monster ParseMonster(int getMonsterIndex, monsterInfo getMonsterInfo)
    {
        Monster monster = new Monster();

        monster.index = getMonsterIndex;
        monster.state = getMonsterInfo.state;
        monster.exp = getMonsterInfo.exp;
        // TODO : 추후 Servant Exp에 따른 Level 공식을 추가해 레벨 적용 필요
        monster.level = DEFINE.GetLevelForExp(getMonsterInfo.exp);
        monster.type = getMonsterInfo.type;
        monster.grade = getMonsterInfo.grade;
        monster.upgrade = getMonsterInfo.upgrade;

        // TODO : 추후 Appear + Type 의 값에 따른 리소스가 저장되어야함
        // monster.appear = ???

        monster.status = ParseStatus(getMonsterInfo.status);
        if(monster.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        return monster;
    }
    bool ParseItemList(List<itemData> getItemList)
    {
        itemDic = new Dictionary<int, Item>();
        
        for(int i=0; i<getItemList.Count; i++)
        {
            Item item = ParseItem(getItemList[i].index, getItemList[i].item);
        }

        return true;
    }

    public Item ParseItem(int getItemIndex, itemInfo getItemInfo)
    {
        Item item = new Item();

        item.index = getItemIndex;
        item.state = getItemInfo.state;
        item.id = getItemInfo.id;
        item.slot = getItemInfo.slot;
        item.tier = getItemInfo.tier;
        item.job = getItemInfo.job;
        item.grade = getItemInfo.grade;
        item.upgrade = getItemInfo.upgrade;
        item.atk = getItemInfo.atk;
        item.def = getItemInfo.def;

        item.status = ParseStatus(getItemInfo.status);
        if(item.status == null)
        {
            Debug.Log("Invalid Status Info");
            return null;
        }

        return item;   
    }


    //public Character AddServant(cservantinfo servantinfo)
    //{
    //    Character newChar = new Character(servantinfo);
    //    SetServant(newChar);
    //    // 만약 여기서 인자로 받는다도 해도
    //    // 씬이 전환 됬을 때 이 값을 복구할 수 있겠는가?
    //    AddNewCharImage(newChar, CHAR_TYPE.SERVANT);

    //    return newChar;
    //}

    //public Character AddMonster(cmonsterinfo monsterinfo)
    //{
    //    Character newChar = new Character(monsterinfo);
    //    SetMonster(newChar);
    //    AddNewCharImage(newChar, CHAR_TYPE.MONSTER);

    //    return newChar;
    //}
    //public void AddItem(cmonsterinfo monsterinfo)
    //{
    //    Character newChar = new Character(monsterinfo);
    //    SetServant(newChar);
    //    AddNewCharImage(newChar, CHAR_TYPE.MONSTER);
    //}




    #endregion
}
