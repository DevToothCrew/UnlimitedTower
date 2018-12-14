using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using LitJson;
[System.Serializable]


public class Test
{
    public int a = 1;
    public int b = 2;
    public int c = 3;

}
public class PacketManager : MonoSingleton<PacketManager> {

    #region ServerConnect

    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    [DllImport("__Internal")]
    private static extern void PrintFloatArray(float[] array, int size);

    [DllImport("__Internal")]
    private static extern int AddNumbers(int x, int y);

    [DllImport("__Internal")]
    private static extern string StringReturnValueFunction();

    [DllImport("__Internal")]
    private static extern void BindWebGLTexture(int texture);

    // Added by John
    [DllImport("__Internal")]
    //private static extern void SetFormation(FormationParty formationParty);
    //private static extern void SetFormation(int[] formationArr);
    //private static extern void SetFormation (int party_num, int pos_1, int pos_2, int pos_3, int pos_4, int pos_5, int pos_6, int pos_7, int pos_8, int pos_9, int pos_10);
    private static extern void SetFormation(string formation);
    [DllImport("__Internal")]
    private static extern void GetStageInfo (int stage_num);

    [DllImport("__Internal")]
    private static extern void BattleAction (int target_1, int action_1, int target_2, int action_2);

    [DllImport("__Internal")]
    private static extern void StartBattle (int stage_num, int party_num);
    



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

        // Scatter 보내기
        Response_Login();
    }

    public void Request_CreatePlayer()
    {
        Debug.Log("Request_CreatePlayer");

        Response_CreatePlayer();
    }

    public void Request_GetLobbyInfo()
    {
        Debug.Log("Request_GetLobbyInfo");
        if(UserDataManager.Inst.CheckEnterLobby() == false)
        {
            Debug.Log("You Need Login And Create Player");
        }
        else
        {
            Response_GetLobbyInfo();
        }
    }


    public void Request_EntryGacha()
    {
        Debug.Log("Request_EntryGacha");
        Response_EntryGacha();
    }
    public void Request_ExecuteGacha()
    {
        Debug.Log("Request_ExecuteGacha");
        Response_ExecuteGacha();
    }
    public void Request_GachaResult(GACHA_TYPE gachaType)
    {
        Debug.Log("Request_GachaResult");
        Response_GachaResult(gachaType);
    }
    public void Request_ExitGacha()
    {
        Response_ExitGacha();
    }

    public void Request_GetPartnerInfo()
    {
        Debug.Log("Request_GetPartnerInfo");
    }
    public void Request_SaveFormation()
    {
        Debug.Log("Request_SaveFormation");

        List<int> formationList = new List<int>();
        int[] testFormatonArr = new int[10];

        int formationNum = 0;
        for (int i = 0; i < 10; i++)
        {
            if (UserDataManager.Inst.formationDic.ContainsKey(i) == true)
            {
                if (!UserDataManager.Inst.formationDic.TryGetValue(i, out formationNum))
                {
                    Debug.Log("Error : SendFormationInfo");
                }
            }
            else
            {
                formationNum = 0;

            }
            formationList.Add(formationNum);
            testFormatonArr[i] = 10;
        }



        Test t = new Test();
        t.a = 10;
        t.b = 1;
        t.c = 2;

        string json = JsonUtility.ToJson(t);
        Debug.Log("print Jsson : : " + json);


        //var a = int.Parse(str);
        //Debug.Log("Parse : " + a);
       SetFormation(json);

        Response_SaveFormation();
    }


    public void SendFormtionInfo()
    {
        List<int> formationList = new List<int>();
        int formationNum = 0;
        for (int i = 0; i < 10; i++)
        {
            if (UserDataManager.Inst.formationDic.ContainsKey(i) == true)
            {
                if (!UserDataManager.Inst.formationDic.TryGetValue(i, out formationNum))
                {
                    Debug.Log("Error : SendFormationInfo");
                }
            }
            else
            {
                formationNum = 0;

            }
            formationList.Add(formationNum);

        }
 
        //SetFormation(
        // var serializer = new System.Web.Script.Ser
        //avaScriptSerializer json = new JavaScriptSerializer();

    }

    public void Request_GetStageInfo(int stageNum)
    {
        // TODO : if delete
        if(UserDataManager.Inst.servantDic.Count<=0)
        {
            Debug.Log("캐릭터가 없습니다. 뽑기를 해주세요");
            return;
        }
        if(UserDataManager.Inst.formationDic.ContainsKey(7) == false)
        {
            Debug.Log("포메이션 설정이 안됬습니다.");
            return;
        }


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

    public void Request_Logout()
    {
        Debug.Log("Request_Logout");
        Response_Logout();
    }

    #endregion



    #region Response

    public void Response_Login()
    {
        Debug.Log("Response_Login");
        // 스캐터 답받기
        

        UserDataManager.Inst.SetUserLoginFlag(true);
        // 상태변화에 대한것은 LobbyManager에서 표현할수 있어야 한다.
    }

    public void Response_CreatePlayer()
    {
        Debug.Log("Response_CreatePlayer");
        UserDataManager.Inst.SetCreatePlayerFlag(true);
    }

    public void Response_GetLobbyInfo()
    {

        Debug.Log("Response_GetLobbyInfo");
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Lobby);

        //  TODO : Test Code if deleted
        // void RecivePacketCharInfo()..

        // 로비에 들어가면 데이터 연동
        //UserDataManager.Inst.SetChar(TestDB.LoadCharactersData());


        // TODO : 확실히 필요없다고 판단되면 삭제할것
        UserDataManager.Inst.ChangeSceneState(SCENE_STATE.Lobby);
    }


    public void Response_EntryGacha()
    {
        Debug.Log("Response_EntryGacha");
    }
    public void Response_ExecuteGacha()
    {
        Debug.Log("Response_ExecuteGacha");
    }


    public void Response_GachaResult(GACHA_TYPE gachaType)
    {
        switch (gachaType)
        {
            case GACHA_TYPE.Servant:
                {
                    Debug.Log("Response_ServantGacha");
                    // TODO : 현재 임시로 TestDB에서 캐릭터 정보 가져와서
                    Character newChar = new Character(UserDataManager.Inst.GetCharacterIndex(), gachaType);
                    // 가챠의 결과로 나온 캐릭터 정보를 저장한다.
                    UserDataManager.Inst.SetServant(newChar);
                    UserDataManager.Inst.AddNewCharImage(newChar.Name, CHAR_TYPE.SERVANT);
                    GachaImage.Inst.SetGachaReult(newChar, gachaType);
                    //GachaInfoPopup.Inst.SetGachaResultInfoPopup(newChar);
                    break;
                }
            case GACHA_TYPE.Monster:
                {
                    Debug.Log("Response_MonsterGacha");

                    Character newChar = new Character(UserDataManager.Inst.GetMonsterIndex(), gachaType);
                    UserDataManager.Inst.SetMonster(newChar);
                    UserDataManager.Inst.AddNewCharImage(newChar.Name, CHAR_TYPE.MONSTER);
                    GachaImage.Inst.SetGachaReult(newChar, gachaType);

                    break;
                }
            case GACHA_TYPE.Item:
                {
                    Debug.Log("Response_ItemGacha");
                    break;
                }
        }
    }
    public void Response_ExitGacha()
    {
        Debug.Log("Response_ExitGacha");
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


        // TODO : 확실히 필요없다고 판단되면 삭제할것
        UserDataManager.Inst.ChangeSceneState(SCENE_STATE.Stage);
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
        UserDataManager.Inst.RemoveUserInfo();
        LobbyManager.Inst.ChangeSceneState(SCENE_STATE.Login);



        // TODO : 확실히 필요없다고 판단되면 삭제할것
        UserDataManager.Inst.ChangeSceneState(SCENE_STATE.Login);
    }

    #endregion
}
