using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataManager : MonoSingleton<UserDataManager> {

    // TODO : Test Flag
    public bool UserLoginFlag;
    public bool CreatePlayerFlag;
    public SCENE_STATE sceneState = SCENE_STATE.None;


    public cuserauth userInfo = new cuserauth();
    //public int hero_slot;


    public Character heroChar;
    public Dictionary<int, Character> servantDic = new Dictionary<int, Character>();
    public Dictionary<int, Character> monsterDic = new Dictionary<int, Character>();
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
           // heroChar = new Character(CHAR_TYPE.SERVANT);
            //CreateHero();
            //for (int i = 0; i < TestCharNum; i++)
            //{
            //   CreateServant();
            //   CreateMonster();
            //}
            //Create10NumberMonster();
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
        // 클라 싱글 실행.
        //servantDic.Add(servantIndex, newChar);

        servantDic.Add((int)newChar.UniqueIndex-1, newChar);
        servantIndex += 1;
    }
    public void SetMonster(Character newChar)
    {
        // 클라 싱글 실행.
        //monsterDic.Add(monsterIndex, newChar);

        monsterDic.Add((int)newChar.UniqueIndex - 1, newChar);
        monsterIndex += 1;
    }

    public void  LoadUserData()
    {
        string path = "UI/CharaterImage/" + heroChar.Name;
        LoadCharImage(path, DEFINE.HERO_FORMATION_NUM, null);
        // LobbyManager.Inst
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


    public void GetLogin(cuserauth _userInfo)
    {  
        userInfo.a_game_money = _userInfo.a_game_money;
        userInfo.a_hero_slot = _userInfo.a_hero_slot;
        userInfo.a_state = _userInfo.a_state;
       // Debug.Log("list Count : " + _userInfo.a_hero_List.Count);
        
        //for (int i = 0; i < _userInfo.a_hero_List.Count; i++)
        foreach(var hero in _userInfo.a_hero_list)
        {
            Debug.Log("save hero info");
            //userInfo.a_hero_List.Add(_userInfo.a_hero_List[i]);
           userInfo.a_hero_list.Add(hero);


        }
      


        heroChar = new Character(userInfo.a_hero_list[0]);

        Debug.Log("hero staute : " + heroChar.Str + " " + heroChar.Dex + " "
             + heroChar.Int);
        Debug.Log(userInfo.a_game_money);
        Debug.Log(userInfo.a_hero_slot);

        formationDic.Add(DEFINE.HERO_FORMATION_NUM, 0);
        string path = "UI/CharaterImage/" + heroChar.Name;
        LoadCharImage(path, DEFINE.HERO_FORMATION_NUM, null);
    }
    public void LoadAllServant(cservant servant_info)
    {
        servantDic.Clear();
        for (int i = 0; i < servant_info.servant_list.Count; i++)
        {  
            AddServant(servant_info.servant_list[i]);
        }
    }
    public void LoadAllMonster(cmonster monster_info)
    {
        monsterDic.Clear();
        for (int i = 0; i < monster_info.monster_list.Count; i++)
        {
            AddMonster(monster_info.monster_list[i]);
        }
    }



    public Character AddServant(cservantinfo servantinfo)
    {
        Character newChar = new Character(servantinfo);
        SetServant(newChar);
        // 만약 여기서 인자로 받는다도 해도
        // 씬이 전환 됬을 때 이 값을 복구할 수 있겠는가?
        AddNewCharImage(newChar, CHAR_TYPE.SERVANT);

        return newChar;
    }

    public Character AddMonster(cmonsterinfo monsterinfo)
    {
        Character newChar = new Character(monsterinfo);
        SetMonster(newChar);
        AddNewCharImage(newChar, CHAR_TYPE.MONSTER);

        return newChar;
    }
    public void AddItem(cmonsterinfo monsterinfo)
    {
        Character newChar = new Character(monsterinfo);
        SetServant(newChar);
        AddNewCharImage(newChar, CHAR_TYPE.MONSTER);
    }



    #endregion


}
