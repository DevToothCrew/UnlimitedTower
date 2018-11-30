using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataManager : MonoSingleton<UserDataManager> {

    // TODO : Test Flag
    public bool UserLoginFlag;
    public bool CreatePlayerFlag;
    public SCENE_STATE sceneState = SCENE_STATE.None;

    public Dictionary<int, Character> characterDic = new Dictionary<int, Character>();
    public Dictionary<int, int> formationDic = new Dictionary<int, int>();

    public List<int> fomationList = new List<int>();
    public GameObject PutChar;

    public List<int> userCharsKeyList = new List<int>();

    // 포메이션 임시 틀
    public List<int> formationOrderList = new List<int>()
    {
      7, 6, 8, 5, 9, 2, 1, 3, 0, 4 
    };

    public int TestCharNum = 10;

    // Delegates for receiving event
    public delegate void LoaderEvent();
    public event LoaderEvent loaderEvent = null;

    // TODO : 확실히 필요없다고 판단되면 삭제할것
    //public GameObject LobbyBackGround;
    //public GameObject StageBackGround;
    // public GameObject StageList;

    public int characterIndex = 0;

    public void Awake()
    {
       

        //TODO :  나의 캐릭터(무조건 존재하는 값)이라고 가정
       for(int i=0; i<TestCharNum; i++)
        {
            CreateChar();
        }


        InitFlag();
        // TODO : 확실히 필요없다고 판단되면 삭제할것
        //LobbyBackGround.SetActive(true);
        //StageBackGround.SetActive(false);
        //StageList.SetActive(false);
    }

    void CreateChar()
    {
        Character newChar = new Character(UserDataManager.Inst.GetCharacterIndex() + 1);
        UserDataManager.Inst.SetCharacter(newChar);
        UserDataManager.Inst.AddNewCharImage(newChar.Name);
    }

    public void InitFlag()
    {
        Debug.Log("InitFlag");
        UserLoginFlag = false;
        CreatePlayerFlag = false;
 
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

    // TODO : Test Code if deleted
    public void SetChar(Dictionary<int, Character> getCharcterDic)
    {
        characterDic = getCharcterDic;
    }

    // 새로운 캐릭터를 dic에 저장한다.
    public void SetCharacter(Character newChar)
    {

        characterDic.Add(characterIndex, newChar);
        characterIndex += 1;
    }
    public void  LoadUserData()
    {
        LoadCharList();
       // LoadFormation();
    }
    public void LoadFormation()
    {
        // 포메이션 데이터를 가져온다.

        GameObject[] decks = GameObject.FindGameObjectsWithTag("Deck");
        string deckName = null;
        foreach (KeyValuePair<int, int> dic in formationDic)
        {

            Debug.Log("포메이션 세팅");

            // 위치값으로 해당 덱 오브젝트를 찾으면될듯
            // 근데 덱 오브젝트를 어찌 찾냐?

            // 포메이션 위치
            deckName = "Deck" + dic.Key;

            for (int i=0; i<10; i++)
            {
                //if(FormationManager.Inst.gameObject.transform.GetChild(i).name == deckName)


            
                if (LobbyManager.Inst.FormationList.gameObject.transform.GetChild(i).gameObject.name == deckName)
                {
                    Debug.Log("자식 존재");
                }


                //if (FormationManager.Inst.deckList[i].name == deckName)
                FormationManager.Inst.transform.gameObject.transform.GetChild(i);
                if (FormationManager.Inst.gameObject.transform.GetChild(i).gameObject.name == deckName)
                {
                    Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + characterDic[dic.Value].Name);
                    FormationManager.Inst.deckList[i].transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                    //FormationManager.Inst.gameObject.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = sprite;
                }
            }
           
            


           //GameObject deck =  GetDeck(ref decks, deckName);
           // if(deck)
           // {
           //     Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + characterDic[dic.Value].Name);
           //     deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
           // }
                     
        }
    }


    // 로비로 되돌아 올때 캐릭터 리스트 다시 불러오는 함수
    private void LoadCharList()
    {
        int charDicCount = characterDic.Count;

        // 캐릭터 개수만큼 캐릭터 목록을 다시 불러온다.
        foreach (KeyValuePair<int, Character> dic in characterDic)
        {
            var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
            if (instance.GetComponent<Image>())
            {
                instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + characterDic[dic.Key].Name);
                instance.transform.SetParent(LobbyManager.Inst.CharacterListContent.transform.transform);
                instance.GetComponent<CharListContent>().CharDicKey = dic.Key;
                if (characterDic[dic.Key].OnFormation)
                {
                    Color color = instance.GetComponent<Image>().color;
                    color.r = color.g = color.b = 0.35f;
                    instance.GetComponent<Image>().color = color;
                }

            }
        }
        // 이전 코드
        #region 
        //for (int i = 0; i < charDicCount; i++)
        //{
        //    var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
        //    if (instance.GetComponent<Image>())
        //    {
        //        instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + characterDic[i].Name);
        //        instance.transform.SetParent(LobbyManager.Inst.CharacterListContent.transform.transform);
        //        instance.GetComponent<CharListContent>().CharDicKey = i;
        //        if (characterDic[i].OnFormation)
        //        {
        //            Color color = instance.GetComponent<Image>().color;
        //            color.r = color.g = color.b = 0.35f;
        //            instance.GetComponent<Image>().color = color;
        //        }

        //    }

        //}
        #endregion 
    }

    //딕셔너리 기준으로 0 1 2 3 5 ... 키 값 순으로 들어가있다.



    // 캐릭터 딕셔너리에 이 캐릭터가 포메이션에 들어가 있는지
    // 확인할 필요가 있을거 같다.

    // 쉽게 말해서 하단 캐릭터 이미지가 내 캐릭터 정보를 시각화한것이라고 생각하면 될거 같다
    // 즉, dic의 정보의 시각화를 하면될듯

    // 근데 bat
    public void AddNewCharImage(string getChar)
    {
        var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
        if (instance.GetComponent<Image>())
        {
            instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + getChar);
            instance.GetComponent<CharListContent>().CharDicKey = characterIndex - 1;

            instance.transform.SetParent(LobbyManager.Inst.CharacterListContent.transform.transform);




            SetFormation();
        } 
    }

    // TODO : Test Code if deleted
    private void SetFormation()
    {
        // 지금은 들어가는 순서대로 세팅.
        userCharsKeyList.Add(characterDic.Count - 1);
    }
    private GameObject GetDeck(ref GameObject[] decks, string deckName)
    {
        if(decks == null)
        {
            return null;
        }

        for(int i=0; i<decks.Length; i++)
        {
            if(decks[i].name == deckName)
            {
                return decks[i];
            }
        }
        return null;

    }
    public void RemoveUserInfo()
    {
        Debug.Log("Remove UserInfo");
        characterDic.Clear();
        userCharsKeyList.Clear();
        characterIndex = 0;
    }



}
