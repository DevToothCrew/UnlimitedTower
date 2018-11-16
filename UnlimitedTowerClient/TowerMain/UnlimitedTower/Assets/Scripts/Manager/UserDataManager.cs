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



    // TODO : 확실히 필요없다고 판단되면 삭제할것
    //public GameObject LobbyBackGround;
    //public GameObject StageBackGround;
    // public GameObject StageList;

    public int characterIndex = 0;

    public void Awake()
    {
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


    // TODO : 확실히 필요없다고 판단되면 삭제할것

    //public void ChangeSceneState(SCENE_STATE state)
    //{
    //    sceneState = state;

    //    switch (state)
    //    {
    //        case SCENE_STATE.Lobby:
    //            StageBackGround.SetActive(false);
    //            StageList.SetActive(false);
    //            LobbyBackGround.SetActive(true);
    //            break;
    //        case SCENE_STATE.Stage:
    //            LobbyBackGround.SetActive(false);
    //            StageBackGround.SetActive(true);
    //            StageList.SetActive(true);
    //            break;
    //    }
    //}

    public int GetCharacterIndex()
    {
        return characterIndex;
    }

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

        for (int i = 0; i < charDicCount; i++)
        {
            var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
            if (instance.GetComponent<Image>())
            {
                instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("CharaterImage/" + UserDataManager.Inst.characterDic[i].Name);
                instance.transform.SetParent(LobbyManager.Inst.CharacterListContent.transform.transform);
            }

        }
    }
    public void AddNewChar(string getChar)
    {
        var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
        if (instance.GetComponent<Image>())
        {
            instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("CharaterImage/" + getChar);
        }
        instance.transform.SetParent(LobbyManager.Inst.CharacterListContent.transform.transform);

       //SetFormation();
    }

    // TODO : Test Code if deleted
    private void SetFormation()
    {
        List<int> formationList = new List<int>()
        {2 };

 

        switch(characterDic.Count)
        {
            case 1:
                {
                    formationDic.Add(2, characterDic[characterDic.Count - 1]);
                    break;
                }
            case 2:
                {
                    formationDic.Add(1, characterDic[characterDic.Count - 1]);
                    break;
                }
            case 3:
                {
                    formationDic.Add(3, characterDic[characterDic.Count - 1]);
                    break;
                }
            case 4:
                {
                    formationDic.Add(0, characterDic[characterDic.Count - 1]);
                    break;
                }
            case 5:
                {
                    formationDic.Add(4, characterDic[characterDic.Count - 1]);
                    break;
                }
            case 6:
                {
                    formationDic.Add(7, characterDic[characterDic.Count - 1]);
                    break;
                }
            case 7:
                {
                    formationDic.Add(6, characterDic[characterDic.Count - 1]);
                    break;
                }
            case 8:
                {
                    formationDic.Add(8, characterDic[characterDic.Count - 1]);
                    break;
                }
             case 9:
                {
                    formationDic.Add(5, characterDic[characterDic.Count - 1]);
                    break;
                }
            case 10:
                {
                    formationDic.Add(9, characterDic[characterDic.Count - 1]);
                    break;
                }
        }
        
        
       
    }

}
