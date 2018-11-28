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
    public Dictionary<int, Character> formationDic = new Dictionary<int, Character>();

    public List<int> fomationList = new List<int>();
    public GameObject PutChar;

    public List<int> userCharsKeyList = new List<int>();
    public List<int> formationOrderList = new List<int>()
    {
        2, 1, 3, 0, 4, 7, 6, 8, 5, 9
    };

    // 캐릭터 딕셔너리에 이 캐릭터가 포메이션에 들어가 있는지
    // 확인할 필요가 있을거 같다.



    // TODO : 확실히 필요없다고 판단되면 삭제할것
    //public GameObject LobbyBackGround;
    //public GameObject StageBackGround;
    // public GameObject StageList;

    public int characterIndex = 0;

    public void Awake()
    {
        Character newChar = new Character(UserDataManager.Inst.GetCharacterIndex() + 1);

        //TODO :  Test 
        UserDataManager.Inst.SetCharacter(newChar);
        UserDataManager.Inst.AddNewCharImage(newChar.Name);
        InitFlag();
        // TODO : 확실히 필요없다고 판단되면 삭제할것
        //LobbyBackGround.SetActive(true);
        //StageBackGround.SetActive(false);
        //StageList.SetActive(false);
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

    // 새로운 캐릭터를 dic에 저장한다.
    public void SetCharacter(Character newChar)
    {
       
        characterDic.Add(characterIndex, newChar);
        characterIndex += 1;
    }

    // TODO : Test Code if deleted
    public void SetChar(Dictionary<int, Character> getCharcterDic)
    {
        characterDic = getCharcterDic;
    }


    public void LoadCharList()
    {
        int charDicCount = characterDic.Count;

        // 캐릭터 개수만큼 캐릭터 목록을 다시 불러온다.
        for (int i = 0; i < charDicCount; i++)
        {
            var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
            if (instance.GetComponent<Image>())
            {
                instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + characterDic[i].Name);
                instance.transform.SetParent(LobbyManager.Inst.CharacterListContent.transform.transform);
            }

        }
    }
    public void AddNewCharImage(string getChar)
    {
        var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
        if (instance.GetComponent<Image>())
        {
            instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + getChar);
        }
        instance.transform.SetParent(LobbyManager.Inst.CharacterListContent.transform.transform);

       SetFormation();
    }

    // TODO : Test Code if deleted
    private void SetFormation()
    {
        // 지금은 들어가는 순서대로 세팅.
        userCharsKeyList.Add(characterDic.Count - 1);
    }

    public void RemoveUserInfo()
    {
        Debug.Log("Remove UserInfo");
        characterDic.Clear();
        userCharsKeyList.Clear();
        characterIndex = 0;
    }

}
