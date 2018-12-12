using System.Collections;
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



    // 포메이션 : 캐릭터 인덱스 ->가 들어가는 딕셔너리
    public Dictionary<int, int> formationDic = new Dictionary<int, int>();


    // 유저의 캐릭터 키값들 담는 리스트
    public List<int> userCharsKeyList = new List<int>();

    // 포메이션 임시 틀
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

    public int characterIndex = 0;
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
               //CreateMonster();
            }
            //Create10NumberMonster();
            testInitFlag = true;
        }  
    }
    void CreateServant()
    {
        Character newChar = new Character(UserDataManager.Inst.GetCharacterIndex(), GACHA_TYPE.Servant);
        SetServant(newChar);
        AddNewCharImage(newChar.Name, CHAR_TYPE.SERVANT);
    }

    void CreateMonster()
    {
        Character newChar = new Character(UserDataManager.Inst.GetMonsterIndex(), GACHA_TYPE.Monster);
        SetMonster(newChar);
        AddNewCharImage(newChar.Name, CHAR_TYPE.MONSTER);
    }
    void Create10NumberMonster()
    {
        Character newChar;
        for (int i = 0; i < 3; i++)
        {
            newChar = new Character(CHARACTER_NUM.Mst_Death);
            SetMonster(newChar);
            AddNewCharImage(newChar.Name, CHAR_TYPE.MONSTER);
        }
        
        for (int i = 3; i < 6; i++)
        {
            newChar = new Character(CHARACTER_NUM.Mst_Robot);
            SetMonster(newChar);
            AddNewCharImage(newChar.Name, CHAR_TYPE.MONSTER);
        }
     
        for (int i = 6; i < 10; i++)
        {
            newChar = new Character(CHARACTER_NUM.Mst_ShadowCat);
            SetMonster(newChar);
            AddNewCharImage(newChar.Name, CHAR_TYPE.MONSTER);
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
            formationDic.Add(DEFINE.HERO_FORMATION_NUM, heroChar.Index);
            string path = "UI/CharaterImage/" + heroChar.Name;
            LoadCharImage(path, DEFINE.HERO_FORMATION_NUM, null);
        }
    }
    private void LoadCharImage(string imageFath, int deckNum, GameObject original)
    {
        GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
        Sprite sprite = Resources.Load<Sprite>(imageFath);


        if (deck.GetComponent<FormationDeck>())
        {
            deck.GetComponent<FormationDeck>().LinkedChar = original;
            deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;


            FormationManager.Inst.DeckTexts[deckNum].SetActive(false);
            //deck.GetComponent<FormationDeck>().ShowEmptyText(false);
        }
        else
        {
            Debug.Log("Do not exist FormatiionDeck Component");
        }

    
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
        return characterIndex;
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

        servantDic.Add(characterIndex, newChar);
        characterIndex += 1;
    }

    public void SetMonster(Character newChar)
    {

        monsterDic.Add(monsterIndex, newChar);
        monsterIndex += 1;
    }

    public void  LoadUserData()
    {
        string path = "UI/CharaterImage/" + heroChar.Name;
        LoadCharImage(path, DEFINE.HERO_FORMATION_NUM, null);
        LoadCharData(LobbyManager.Inst.ServantContentList, ref servantDic, CHAR_TYPE.SERVANT);
        LoadCharData(LobbyManager.Inst.MonsterContentList, ref monsterDic, CHAR_TYPE.MONSTER);
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




    public void AddNewCharImage(string getChar, CHAR_TYPE charType)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/CharContent") as GameObject);

        if (instance.transform.GetChild(0))
        {
            if (charType == CHAR_TYPE.SERVANT)
            {
                instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + getChar);
                instance.GetComponent<CharContent>().CharDicKey = characterIndex - 1;
                instance.transform.SetParent(LobbyManager.Inst.ServantContentList.transform.transform);
                instance.GetComponent<CharContent>().CharType = CHAR_TYPE.SERVANT;
            }
            else
            {
                instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/MonsterImage/" + getChar);
                instance.GetComponent<CharContent>().CharDicKey = monsterIndex - 1;
                instance.transform.SetParent(LobbyManager.Inst.MonsterContentList.transform.transform);
                instance.GetComponent<CharContent>().CharType = CHAR_TYPE.MONSTER;
            }          
        } 
    }

    public void RemoveUserInfo()
    {
        Debug.Log("Remove UserInfo");
        servantDic.Clear();
        userCharsKeyList.Clear();
        characterIndex = 0;
    }


}
