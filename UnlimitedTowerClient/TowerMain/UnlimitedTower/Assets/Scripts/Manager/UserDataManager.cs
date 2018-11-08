using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoSingleton<UserDataManager> {

    // TODO : Test Flag
    public bool UserLoginFlag;
    public bool CreatePlayerFlag;
    public SCENE_STATE sceneState = SCENE_STATE.None;

    public Dictionary<int, Character> characterDic = new Dictionary<int, Character>();

    public GameObject LobbyBackGround;
    public GameObject StageBackGround;

    public GameObject StageList;

    public int characterIndex = 0;

    public void Awake()
    {
        InitFlag();
        LobbyBackGround.SetActive(true);
        StageBackGround.SetActive(false);
        StageList.SetActive(false);
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

        switch (state)
        {
            case SCENE_STATE.Lobby:
                StageBackGround.SetActive(false);
                StageList.SetActive(false);
                LobbyBackGround.SetActive(true);
                break;
            case SCENE_STATE.Stage:
                LobbyBackGround.SetActive(false);
                StageBackGround.SetActive(true);
                StageList.SetActive(true);
                break;
        }
    }

    public int GetCharacterIndex()
    {
        return characterIndex;
    }

    public void SetCharacter(Character newChar)
    {
        characterIndex += 1;
        characterDic.Add(characterIndex, newChar);
    }
}
