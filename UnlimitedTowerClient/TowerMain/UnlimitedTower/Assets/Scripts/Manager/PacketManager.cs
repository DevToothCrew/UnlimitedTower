﻿using LitJson;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class PacketManager : MonoSingleton<PacketManager> {

    #region ServerConnect

    [DllImport("__Internal")]
    private static extern void Login();

    [DllImport("__Internal")]
    private static extern void SignUp();

    [DllImport("__Internal")]
    private static extern void Logout();

    [DllImport("__Internal")]
    private static extern void Gacha();

    [DllImport("__Internal")]
    private static extern void GetItem();

    [DllImport("__Internal")]
    private static extern void SetFormation(string formation);

    [DllImport("__Internal")]
    private static extern void GetStageInfo (int stage_num);

    [DllImport("__Internal")]
    private static extern void BattleAction (string battleAction);

    [DllImport("__Internal")]
    private static extern void StartBattle (int stage_num, int party_num);

    public bool receiveGacha = false;




    void Start()
    {

        //Hello();

        //HelloString("This is a string.");

        //float[] myArray = new float[10];
        //PrintFloatArray(myArray, myArray.Length);

        //int result = AddNumbers(5, 7);
        //Debug.Log(result);

        //Debug.Log(StringReturnValueFunction());

        //var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        //BindWebGLTexture(texture.GetNativeTextureID());
    }

    #endregion

    #region Request

    public void Request_Login()
    {
        Debug.Log("Request_Login");  
        Login();
    }

    public void Request_CreatePlayer()
    {
        Debug.Log("Request_CreatePlayer");
        Response_CreatePlayer();
    }


    public void Request_Gacha()
    {
        Debug.Log("Request_Gacha");
        Gacha();
    }

    public void Request_GachaResult(GACHA_RESULT_TYPE gachaResultType)
    {
        Debug.Log("Request_GachaResult");
        Response_GachaResult(gachaResultType);
    }

    public void Request_GetPartnerInfo()
    {
        Debug.Log("Request_GetPartnerInfo");
    }

    //add by canie
    public void Request_SaveParty()
    {
        // TODO : 추후에 저장 파티 인덱스 넣을것.
        int partyNumber = UserDataManager.Inst.usingPartyNum;
        Party party_info = new Party();
        party_info.partyIndex = partyNumber;


        if (UserDataManager.Inst.partyDic.ContainsKey(partyNumber) == false)
        {
            Debug.Log("Error : send party info");
        }
        else
        {
            party_info = UserDataManager.Inst.partyDic[partyNumber];

            JsonParty data = new JsonParty();
            data.partyNum = party_info.partyIndex;

            for (int i=0; i< party_info.characterList.Count; ++i)
            {
                data.partyList.Add(party_info.characterList[i].index);
            }
            

            string json = JsonUtility.ToJson(data);
            Debug.Log("print Jsson : : " + json);


            // TODO : Test Code
            // JsonParty test = JsonUtility.FromJson<JsonParty>(json);

            SetFormation(json);
        }
    }

    public void Request_GetStageInfo(int stageNum)
    {
        // TODO : if delete
        //if(UserDataManager.Inst.servantDic.Count<=0)
        //{
        //    Debug.Log("캐릭터가 없습니다. 뽑기를 해주세요");
        //    return;
        //}
        //if(UserDataManager.Inst.formationDic.ContainsKey(7) == false)
        //{
        //    Debug.Log("포메이션 설정이 안됬습니다.");
        //    return;
        //}


        Debug.Log("Requset_GetStageInfo : " + stageNum);
        Response_GetStageInfo(stageNum);
        SceneManager.LoadScene("CharacterBattleScene");
        Debug.Log("Success BattleSceene Loading");
 


    }

    public void Request_EnterStage(int stageNum)
    {
        Debug.Log("RequestEnterStage : " + stageNum);
        Response_EnterStage(stageNum);
    }
    public void Request_BattleAction(int action_1,  int action_2, int target_1, int target_2)
    {
        Debug.Log("Request_BattleAction");


        List<JsonBattleAction> actionList = new List<JsonBattleAction>();
        actionList.Add(new JsonBattleAction(target_1, action_1));
        actionList.Add(new JsonBattleAction(target_2, action_2));

        //JsonList fList = new JsonList();
        //fList.list.Add(target_1);
        //fList.list.Add(action_1);
        //fList.list.Add(target_2);
        //fList.list.Add(action_2);


        string json = JsonUtility.ToJson(actionList);

        BattleAction(json);
        Debug.Log("Json action : " + json);


    }

    public void Request_Logout()
    {
        Debug.Log("Request_Logout");
        Logout();
        Response_Logout();
    }


    #endregion



    #region Response

    public void Response_Login(string getLoginInfo)
    {
        Debug.Log(getLoginInfo);

        if (getLoginInfo.StartsWith("{\"sign"))
        {
            // 유저 정보가 없으면 SignUp으로
            Debug.Log("SignUp");
            SignUp();
            return;
        }

        Debug.Log("Response_Login");
        // 스캐터 답받기
        
        UserLoginData userLoginData = JsonUtility.FromJson<UserLoginData>(getLoginInfo); 
        if(userLoginData == null)
        {
            Debug.Log("Invalid Login Data : " + getLoginInfo);
        }

        UserDataManager.Inst.Login(userLoginData);

        // 상태변화에 대한것은 LobbyManager에서 표현할수 있어야 한다.
        UserDataManager.Inst.SetUserLoginFlag(true);
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Lobby);
    }

    public void Response_CreatePlayer()
    {
        Debug.Log("Response_CreatePlayer");
        UserDataManager.Inst.SetCreatePlayerFlag(true);
    }

    public void Response_GetLobbyInfo()
    {
        Debug.Log("Response_GetLobbyInfo");
    }

    //###
    public void Response_Gacha(string getGachaInfo)
    {
        Debug.Log("Response_Gacha : " + getGachaInfo);

        JsonData getInfo = JsonMapper.ToObject(getGachaInfo);
        int type = Convert.ToInt32(getInfo["result_type"].ToString());

        // Servant
        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {
            Debug.Log(getGachaInfo);
            gachaServantData gachaData = JsonUtility.FromJson<gachaServantData>(getGachaInfo);
            Servant getServant = UserDataManager.Inst.ParseServant(gachaData.data.index, gachaData.data.servant);

            GachaImage.Inst.SetGachaResult_Servant(getServant);
            UserDataManager.Inst.AddServantImage(getServant);
        }
        // Monster
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {
            Debug.Log(getGachaInfo);
            gachaMonsterData gachaData = JsonUtility.FromJson<gachaMonsterData>(getGachaInfo);
            Monster getMonster = UserDataManager.Inst.ParseMonster(gachaData.data.index, gachaData.data.monster);
            GachaImage.Inst.SetGachaResult_Monster(getMonster);
            UserDataManager.Inst.AddMonsterImage(getMonster);
        }
        // Item
        else if (type == (int)GACHA_RESULT_TYPE.Item)
        {
            Debug.Log(getGachaInfo);
            gachaItemData gachaData = JsonUtility.FromJson<gachaItemData>(getGachaInfo);
            Item getItem = UserDataManager.Inst.ParseItem(gachaData.data.index, gachaData.data.item);
            GachaImage.Inst.SetGacharResult_Item(getItem);
            //TODO : 아이템 이미지 미구현
        }
    }

    //add by canie
    public void Response_GetParty(string getPartyInfo)
    {
        Debug.Log("Response_GetParty : " + getPartyInfo);
        partyData partyInfo = JsonUtility.FromJson<partyData>(getPartyInfo);

        Party getParty = UserDataManager.Inst.ParseParty(partyInfo.index, partyInfo);
        if (UserDataManager.Inst.partyDic.ContainsKey(getParty.partyIndex) == false)
        {
            UserDataManager.Inst.partyDic.Add(getParty.partyIndex, getParty);
        }
        else
        {
            UserDataManager.Inst.partyDic[getParty.partyIndex] = getParty;
        }
    }

    public void Response_GetBattle(string battle_info)
    {
        Debug.Log("Response_GetBattle : " + battle_info);
        //var pasingData = JsonUtility.FromJson<cbattle>(battle_info);
       // BattleManager.Inst.SetBattle(pasingData);

    }

    //TODO : Test Code
    public void Response_GachaResult(GACHA_RESULT_TYPE gachaResultType)
    {
        switch (gachaResultType)
        {
            case GACHA_RESULT_TYPE.Servant:
                {
                    Debug.Log("Response_ServantGacha");
                    int servantCount = UserDataManager.Inst.GetServantCount() + 1;
                    Servant servant = UserDataManager.Inst.CreateServant(servantCount);
                    GachaImage.Inst.SetGachaCharacterResult(servant.name, servant.status, GACHA_RESULT_TYPE.Servant);
                    break;
                }
            case GACHA_RESULT_TYPE.Monster:
                {
                    Debug.Log("Response_MonsterGacha");

                    //Character newChar = new Character(UserDataManager.Inst.GetMonsterIndex(), gachaType);
                    //UserDataManager.Inst.SetMonster(newChar);
                    //UserDataManager.Inst.AddNewCharImage(newChar, CHAR_TYPE.MONSTER);
                    //GachaImage.Inst.SetGachaReult(newChar, gachaType);

                    int monsterCount = UserDataManager.Inst.GetMonsterCount() + 1;
                    Monster monster = UserDataManager.Inst.CreateMonster(monsterCount);
                    GachaImage.Inst.SetGachaCharacterResult(monster.name, monster.status, GACHA_RESULT_TYPE.Monster);
                    break;
                }
            case GACHA_RESULT_TYPE.Item:
                {
                    Debug.Log("Response_ItemGacha");
                    break;
                }
        }
    }

    public void Response_GetPartnerInfo()
    {
        Debug.Log("Response_GetPartnerInfo");
    }

    public void Response_SaveFormation()
    {
        Debug.Log("Response_SaveFormation");
    }

    public void Response_GetStageInfo(int stageNum)
    {
        Debug.Log("Response_GetStageInfo");
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Stage);
    }

    public void Response_EnterStage(int stageNum)
    {
        // 씬 전환 데이터 정보 변경 필요

        // TODO : 확실히 필요없다고 판단되면 삭제할것
        //SceneManager.LoadScene("CharacterBattleScene");
    }

    public void Response_Logout()
    {
        Debug.Log("Response_Logout");
        UserDataManager.Inst.InitFlag();

        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Login);
    }

    #endregion


    #region Test Response Fuction
    public void Test_Response_Gacha(string getGachaInfo)
    {
        Debug.Log("Response_Gacha : " + getGachaInfo);

        //JsonData getInfo = JsonMapper.ToObject(getGachaInfo);
        //int type = Convert.ToInt32(getInfo["result_type"].ToString());


        Debug.Log(getGachaInfo);
        gachaServantData gachaData = new gachaServantData();
        gachaData.data.index = 2;
        Servant getServant = UserDataManager.Inst.ParseServant(gachaData.data.index, gachaData.data.servant);

        GachaImage.Inst.SetGachaResult_Servant(getServant);
        UserDataManager.Inst.AddServantImage(getServant);

        // Servant
        //if (type == (int)GACHA_RESULT_TYPE.Servant)
        //{
        //    Debug.Log(getGachaInfo);
        //    gachaServantData gachaData = JsonUtility.FromJson<gachaServantData>(getGachaInfo);
        //    Servant getServant = UserDataManager.Inst.ParseServant(gachaData.data.index, gachaData.data.servant);

        //    GachaImage.Inst.SetGachaResult_Servant(getServant);
        //    UserDataManager.Inst.AddServantImage(getServant);
        //}
        //// Monster
        //else if (type == (int)GACHA_RESULT_TYPE.Monster)
        //{
        //    Debug.Log(getGachaInfo);
        //    gachaMonsterData gachaData = JsonUtility.FromJson<gachaMonsterData>(getGachaInfo);
        //    Monster getMonster = UserDataManager.Inst.ParseMonster(gachaData.data.index, gachaData.data.monster);
        //    GachaImage.Inst.SetGachaResult_Monster(getMonster);
        //    UserDataManager.Inst.AddMonsterImage(getMonster);
        //}
        //// Item
        //else if (type == (int)GACHA_RESULT_TYPE.Item)
        //{
        //    Debug.Log(getGachaInfo);
        //    gachaItemData gachaData = JsonUtility.FromJson<gachaItemData>(getGachaInfo);
        //    Item getItem = UserDataManager.Inst.ParseItem(gachaData.data.index, gachaData.data.item);
        //    GachaImage.Inst.SetGacharResult_Item(getItem);
        //    //TODO : 아이템 이미지 미구현
        //}
    }
    #endregion
}
