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

    // 포메이션 : 캐릭터 인덱스 ->가 들어가는 딕셔너리
    public Dictionary<int, int> formationDic = new Dictionary<int, int>();


    // 유저의 캐릭터 키값들 담는 리스트
    public List<int> userCharsKeyList = new List<int>();

    // 포메이션 임시 틀
    public List<int> formationOrderList = new List<int>()
    {
      7, 6, 8, 5, 9, 2, 1, 3, 0, 4 
    };

    public int TestCharNum = 10;



    // TODO : 확실히 필요없다고 판단되면 삭제할것
    //public GameObject LobbyBackGround;
    //public GameObject StageBackGround;
    // public GameObject StageList;

    public int characterIndex = 0;

    public void Awake()
    {


        //TODO :  나의 캐릭터(무조건 존재하는 값)이라고 가정
        for (int i = 0; i < TestCharNum; i++)
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
        LoadFormation();
    }
    public void LoadFormation()
    {

  
        // 포메이션 데이터를 가져온다.
        string deckName = null;
        foreach (KeyValuePair<int, int> dic in formationDic)
        {

            Debug.Log("포메이션 세팅");

            // 포메이션 위치
            deckName = "Deck" + dic.Key;

            for (int i=0; i<10; i++)
            {

                GameObject deck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(i).gameObject;
               // if (FormationManager.Inst.gameObject.transform.GetChild(i).gameObject.name == deckName)
                if (deck.name == deckName)
                {
                    Sprite sprite = Resources.Load<Sprite>("UI/CharaterImage/" + characterDic[dic.Value].Name);
                    deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                    deck.GetComponent<FormationDeck>().Active = true;
                    deck.GetComponent<FormationDeck>().LinkedChar = null;
                    deck.transform.GetChild(1).gameObject.SetActive(false);
                    deck.transform.GetChild(2).gameObject.SetActive(false);

                    // FormationManager.Inst.gameObject.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = sprite;
                }
            }    
           
        }






        for (int i = 0; i < 10; i++)
        {
            int deckNum = UserDataManager.Inst.formationOrderList[i];
            GameObject goDeck = LobbyManager.Inst.FormationList.gameObject.transform.GetChild(deckNum).gameObject;
            if (goDeck.transform.GetChild(0).gameObject.GetComponent<Image>().sprite)
            {
                Debug.Log("Load Open Close : " + deckNum);

            
                if (UserDataManager.Inst.formationDic.ContainsKey(deckNum))
                {
                    int charKey = UserDataManager.Inst.formationDic[deckNum];

                    // 자식 개수가 0이 되서 for문에 접근하질 못한다.
                    // 메모리를 낭비하는 쪽으로 가는건?
                    int charCount = LobbyManager.Inst.CharacterContentList.transform.childCount;
                    for (int j = 0; j < charCount; j++)
                    {
                        GameObject charElement = LobbyManager.Inst.CharacterContentList.gameObject.transform.GetChild(j).gameObject;


                        if (charElement.GetComponent<CharContent>().CharDicKey == charKey)
                        {
                            goDeck.GetComponent<FormationDeck>().LinkedChar = charElement;
                            LobbyManager.Inst.FormationList.GetComponent<FormationManager>().OpenNewDeck(deckNum);
                            LobbyManager.Inst.FormationList.GetComponent<FormationManager>().LoadDeck(deckNum);
                            //주변 덱을 연다.


                            //FormationManager.Inst.OpenNewDeck(deckNum);
                            // 이미 덱이 존재했던 내용을 채운다.
                            // FormationManager.Inst.LoadDeck(deckNum);
                        }
                    }
                }
            }
        }
    }


    // 로비로 되돌아 올때 캐릭터 리스트 다시 불러오는 함수
    public void LoadCharList()
    {
        int charDicCount = characterDic.Count;

        // 캐릭터 개수만큼 캐릭터 목록을 다시 불러온다.
        foreach (KeyValuePair<int, Character> dic in characterDic)
        {
            var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
            if (instance.GetComponent<Image>())
            {
                instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + characterDic[dic.Key].Name);
                instance.transform.SetParent(LobbyManager.Inst.CharacterContentList.transform.transform);
                instance.GetComponent<CharContent>().CharDicKey = dic.Key;
                if (characterDic[dic.Key].OnFormation)
                {
                    Color color = instance.GetComponent<Image>().color;
                    color.r = color.g = color.b = 0.35f;
                    instance.GetComponent<Image>().color = color;
                }
            }
        }    
    }
    public void AddNewCharImage(string getChar)
    {
        var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
        if (instance.GetComponent<Image>())
        {
            instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + getChar);
            instance.GetComponent<CharContent>().CharDicKey = characterIndex - 1;

            instance.transform.SetParent(LobbyManager.Inst.CharacterContentList.transform.transform);




            SetFormation();
        } 
    }

    // TODO : Test Code if deleted
    private void SetFormation()
    {
        // 지금은 들어가는 순서대로 세팅.
       // userCharsKeyList.Add(characterDic.Count - 1);
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
