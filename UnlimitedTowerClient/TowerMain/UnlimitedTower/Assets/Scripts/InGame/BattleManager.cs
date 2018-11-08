using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoSingleton<BattleManager> {

    // TODO : Test Prefabs if Delete
    // 인게임 보여주는 용도의 오브젝트
    public GameObject[] playerObject = new GameObject[DEFINE.PARTY_MAX_NUM];
    public GameObject[] enemyObject = new GameObject[DEFINE.PARTY_MAX_NUM];

    // 실제 배틀하는 게임 데이터를 담고 있는 Dic
    public Dictionary<int, Battle_Character_Status> playerStatusDic = new Dictionary<int, Battle_Character_Status>();
    public Dictionary<int, Battle_Character_Status> enemyStatusDic = new Dictionary<int, Battle_Character_Status>();
    public List<CharacterAction> turnActionList = new List<CharacterAction>();


    // Debug 전용
    public bool debugFlag = false;
    public GameObject debugPanel;
    public Text debugText;

    public BATTLE_STATE BattleState = BATTLE_STATE.NONE;
    public bool MouseClick = false;


    public int attackOrder = -1;
    public GameObject AttackObject;
    public GameObject TargetObject;
    public CharacterAction AttackerAction;

    GameObject playerParty;
    GameObject enemyParty;

    public bool FirstAcess = false;

    void Awake()
    {
        if(debugFlag == true)
        {
            debugPanel.SetActive(true);
           debugText.text = "Debug ";
        }
     
        SetObject();
        SetTurnSpeed();
        
    }
    // TODO : Test code if delete
    private void OnGUI()
    {
        if(GUI.Button(new Rect(10, 800, 100, 70), "BattleRest"))
        {
            RestBattle();
        }


        if (GUI.Button(new Rect(800, 800, 100, 70), "BattleRest"))
        {
            TestSetPosition();
        }

    }

    private void Update()
    {
        SetDebug();
        BattleLogic();
    }
    private void BattleLogic()
    {
        switch (BattleState)
        {
            case BATTLE_STATE.NONE:
                StartBattle();
                break;
            case BATTLE_STATE.BATTLE:
                // 마우스 클릭(공격 시작)을 할 수 없는 상태
                // AutoBattle();
                break;
            case BATTLE_STATE.USER_TURN:
                break;
            case BATTLE_STATE.END:
                break;
        }
    }
    private void SetDebug()
    {

        string playerList = "플레이어 : ";
        foreach(var playerIndex in playerStatusDic.Keys)
        {
            playerList += playerIndex.ToString() + " ";
        }
         playerList += ",   적 : ";
        foreach (var playerIndex in enemyStatusDic.Keys)
        {
            playerList += playerIndex.ToString() + " ";
        }
        debugText.text = playerList;
      

    }
   
    // TODO : Test StartBattle : 배틀 전 플레이어의 세팅.
    void StartBattle()
    {
        //if (Input.GetMouseButtonDown(0) && MouseClick == false && BattleState != BATTLE_STATE.BATTLE)
        //{
        //    MouseClick = true;
        //    Debug.Log("공격 시작");
        //    // 공격전 준비.
        //    ReadyNextTurn();
        //    // 전투 씬 시작.
        //   BattleState = BATTLE_STATE.BATTLE;
        //}
    }


    private bool CheckGameOver()
    {
        if (enemyStatusDic.Count == 0 || playerStatusDic.Count == 0)
        {
            Debug.Log("게임 끝 : " );
            BattleState = BATTLE_STATE.END;
            attackOrder = -1;
            MouseClick = false;
            return true;
        }
        return false;
    }
    private void CheckTargetIsLive(CHAR_TYPE charType)
    {
        if (AttackerAction.charType == CHAR_TYPE.PLAYER)
        {
            CheckTargetDic(ref enemyStatusDic);
        }
        else
        {
            CheckTargetDic(ref playerStatusDic);
        }
    }
    private void CheckCharIsLive()
    {
        for(int i=0; i< turnActionList.Count; i++)
        {
            if(turnActionList[i].charType == CHAR_TYPE.PLAYER)
            {
                if (!playerStatusDic.ContainsKey(turnActionList[i].myIndex))
                    turnActionList.RemoveAt(i);
            }
            else
            {
                if (!enemyStatusDic.ContainsKey(turnActionList[i].myIndex))
                    turnActionList.RemoveAt(i);
            }
        }
    }

  



    private void SetObject()
    {
        // TODO : Test Setting if Delete
        CreateTestObjetct();
    }

    // TODO : Test Setting if Delete
    private void CreateTestObjetct()
    {
        if(!FirstAcess)
        {
            CharacterManager.Inst.SetChar(TestDB.LoadCharactersData());
            FirstAcess = true;
        }
       
        CreateGameObject();
    }
   
    private GameObject GetCharacterObject(int num)
    {
        return Resources.Load("Prefabs/" + ( (CHARACTER_NUM)num ).ToString() ) as GameObject;
    }

    // TODO : Test Setting 
    private void SetTurnSpeed()
    {
        //공격대상, 공격자, 공격하는 타입, 공격자 타입등을 알려줌.
        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            //플레이어, 적
            CharacterAction temp1 = new CharacterAction(i, GetTargetIndex(CHAR_TYPE.PLAYER), ACTION_TYPE.Attack, CHAR_TYPE.PLAYER);
            CharacterAction temp2 = new CharacterAction(i, GetTargetIndex(CHAR_TYPE.ENEMY), ACTION_TYPE.Attack, CHAR_TYPE.ENEMY);

            turnActionList.Add(temp1);
            turnActionList.Add(temp2);
        }

        turnActionList.Sort(SpeedComparer);

      #region
        //foreach (CharacterAction temp in turnActionList)
        //{
        //    if (temp.charType == CHAR_TYPE.PLAYER)
        //    {
        //        Debug.Log(playerStatusDic[temp.myIndex]+" : 스피드 : " + playerStatusDic[temp.myIndex].speed);
        //    }
        //    else
        //        Debug.Log(enemyStatusDic[temp.myIndex] + " : 스피드 : " + enemyStatusDic[temp.myIndex].speed);

        //}
        #endregion
    }
    

    int GetTargetIndex(CHAR_TYPE charType)
    {
        int targetIndex = Random.Range(0, DEFINE.PARTY_MAX_NUM);
        if (charType == CHAR_TYPE.PLAYER)
        {          
            while (!enemyStatusDic.ContainsKey(targetIndex))
            {
                targetIndex = Random.Range(0, DEFINE.PARTY_MAX_NUM);
            }           
        }
        else
        {
            while (!playerStatusDic.ContainsKey(targetIndex))
            {
                targetIndex = Random.Range(0, DEFINE.PARTY_MAX_NUM);
            }
        }
        return targetIndex;
    }
    public Battle_Character_Status GetChar(CHAR_TYPE charType, int index)
    {
        if(charType == CHAR_TYPE.PLAYER)
        {
            if (playerStatusDic.ContainsKey(index))
            {
                return playerStatusDic[index];
             }
            else
            {
                return null;
            }
        }
        else
        {
            if (enemyStatusDic.ContainsKey(index))
            {
                return enemyStatusDic[index];
            }
            else
            {
                return null;
            }
        }

    }

    int SpeedComparer(CharacterAction x, CharacterAction y)
    {
        int xSpeed;
        int ySpeed;
        if(x.charType == CHAR_TYPE.PLAYER)
        {
            xSpeed = playerStatusDic[x.myIndex].speed;
        }
        else
        {
            xSpeed = enemyStatusDic[x.myIndex].speed;

        }

        if (y.charType == CHAR_TYPE.PLAYER)
        {
            ySpeed = playerStatusDic[y.myIndex].speed;
        }
        else
        {
            ySpeed = enemyStatusDic[y.myIndex].speed;

        }
        return ySpeed.CompareTo(xSpeed);


    }


    public void GetCharObject(CHAR_TYPE charType, int myIndex,  ref GameObject myObject, int targetNum, ref GameObject TargetObject)
    {
        if (charType == CHAR_TYPE.PLAYER)
        {
            myObject = playerObject[myIndex];
            TargetObject = enemyObject[targetNum];
        }
        else
        {
            myObject = enemyObject[myIndex];
            TargetObject = playerObject[targetNum];
        }   
    }

    public bool CheckAttackCycle()
    {
        if (attackOrder >= enemyStatusDic.Count + playerStatusDic.Count)
        {
            // 두 개의 dic의 크기를 넘으면 모든 캐릭터가 한번씩 했음.
            Debug.Log("1턴 종료 -> attackaNum : " + attackOrder);
            BattleState = BATTLE_STATE.NONE;
            attackOrder = -1;
            MouseClick = false;
            return true;
        }
        return false;
    }

    public void DecreaseHp(CHAR_TYPE charType, int myIndex, int targetIndex)
    {
       if(charType != CHAR_TYPE.PLAYER)
        {
            RemoveBattleDic(ref enemyStatusDic, ref playerStatusDic, playerObject, myIndex, targetIndex);         
        }
        else
        {
            RemoveBattleDic(ref playerStatusDic, ref enemyStatusDic, enemyObject, myIndex, targetIndex);
        }     
    }

    public void ReadyNextTurn()
    {
        ++attackOrder;
        CheckCharIsLive();
        if (CheckGameOver())
        {
            return;
        }
        if (attackOrder < enemyStatusDic.Count + playerStatusDic.Count)
        {

            AttackerAction = turnActionList[attackOrder];


            // 타켓이 죽었는지 검사
            CheckTargetIsLive(AttackerAction.charType);
            GetCharObject(AttackerAction.charType, AttackerAction.myIndex, ref AttackObject,
                                                                AttackerAction.targetIndex, ref TargetObject);

            // 현재 공격하는 오브젝트를 실행한다.
            CharController charController = AttackObject.GetComponent<CharController>();
            charController.StartMyTurn(STATE_TYPE.RUN, TargetObject, AttackerAction);

        }
    }

    private void RemoveBattleDic(ref Dictionary<int, Battle_Character_Status> attackDic, ref Dictionary<int, Battle_Character_Status> targetDic,
    GameObject[] charObject, int myIndex, int targetIndex)
    {
        targetDic[targetIndex].nowHp -= attackDic[myIndex].damage;
        if (targetDic[targetIndex].nowHp <= 0)
        {
            targetDic[targetIndex].nowHp = 0;
            charObject[targetIndex].GetComponent<CharController>().PlayDeadAnimation();


            // 죽으면..
            if (targetDic.ContainsKey(targetIndex))
            {
                targetDic.Remove(targetIndex);
                Debug.Log("플레이어 : " + targetIndex + "삭제 실행");

            }
        }
        else
        {
            charObject[targetIndex].GetComponent<CharController>().PlayHitAnimation();
            Debug.Log("플레이어 : " + targetIndex + "피격 애니 실행");
        }
    }

    private void CheckTargetDic(ref Dictionary<int, Battle_Character_Status> charStatusDic)
    {
        if (charStatusDic.ContainsKey(AttackerAction.targetIndex))
        {
            return;
        }
        else
        {
            AttackerAction.targetIndex = GetTargetIndex(AttackerAction.charType);
        }
    }
    private void CreateGameObject()
    {
        playerParty = new GameObject("playerParty");
        enemyParty = new GameObject("enemyParty");
        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            if (CharacterManager.Inst.characterDic.ContainsKey(i) == false)
            {
                return;
            }
            //캐릭터 정보,                     //partyIndex   //chartIndex(필요없는 값일 수도 있음_
            Battle_Character_Status status = new Battle_Character_Status(CharacterManager.Inst.characterDic[i], i, i, 0);
            playerStatusDic.Add(i, status);
            playerObject[i] = Instantiate(GetCharacterObject(status.character.Index), DEFINE.GetBattlePosition(i, CHAR_TYPE.PLAYER, CharacterManager.Inst.characterDic[i].Index), Quaternion.identity);         
            playerObject[i].GetComponent<CharController>().charType = CHAR_TYPE.PLAYER;
            playerObject[i].GetComponent<CharController>().charSize = status.sizeType;
            playerObject[i].transform.SetParent(playerParty.transform.transform, false);
            //playerObject[i].GetComponent<CharController>().battleDicIndex = i;
        }
   

        // TODO : DB 대신 임시로
        Dictionary<int, Character> enemyList = TestDB.LoadMonstersData();

        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            if (enemyList.ContainsKey(i) == false)
            {
                return;
            }
            Battle_Character_Status status = new Battle_Character_Status(enemyList[i], i, i, enemyList[i].Size);
            enemyStatusDic.Add(i, status);


            enemyObject[i] = Instantiate(GetCharacterObject(status.character.Index), DEFINE.GetBattlePosition(i, CHAR_TYPE.ENEMY, enemyList[i].Index), Quaternion.Euler(new Vector3(0, 180, 0)));
           //enemyObject[i] = Instantiate(GetCharacterObject(status.character.Index), enemyStatusDic[i].firstPos, Quaternion.Euler(new Vector3(0, 180, 0)));
            enemyObject[i].GetComponent<CharController>().charType = CHAR_TYPE.ENEMY;
            enemyObject[i].GetComponent<CharController>().battleDicIndex = i;
            enemyObject[i].GetComponent<CharController>().charSize = status.sizeType;
            enemyObject[i].transform.SetParent(enemyParty.transform.transform, false);
        }
   
   }

    void SetBattolePosition( GameObject[] charObjects, ref Dictionary<int, Battle_Character_Status> charBattleStatusDic)
    {
        float bigOffset = 0.0f;

        int backLineCenterIndex = charBattleStatusDic.Count/2/2;
        int frontLineCenterIndex = charBattleStatusDic.Count / 2 / 2  + charBattleStatusDic.Count/2;

        Battle_Character_Status centerCharStatus = charBattleStatusDic[backLineCenterIndex];
        Vector3 backCenterPos = GetBackLineCenterCharPos(centerCharStatus.sizeType);
        charObjects[backLineCenterIndex].transform.position = backCenterPos;


        SetLeftPosition(charObjects, backLineCenterIndex, 0, centerCharStatus.sizeType, ref charBattleStatusDic);
        SetRightPosition(charObjects, backLineCenterIndex, 2, centerCharStatus.sizeType, ref charBattleStatusDic);

 
        // 앞 라인 중심 위치.
        Vector3 frontCenterPos = GetFrontLineCenterCharPos(charBattleStatusDic[frontLineCenterIndex].sizeType);
                                

        float frontLineDis = GetBackLineLargestDistance(ref charBattleStatusDic);

        //bigOffset = GetBigOffset(ref charBattleStatusDic);

        // ***수정할것.
        if (charBattleStatusDic[frontLineCenterIndex].sizeType == SIZE_TYPE.MIDDLE)
            frontLineDis -= 0.25f;

        else if (charBattleStatusDic[frontLineCenterIndex].sizeType == SIZE_TYPE.BIG)
            frontLineDis -= 0.5f;


        frontLineDis -= bigOffset;


        frontCenterPos.z = frontLineDis;
        charObjects[frontLineCenterIndex].transform.position = frontCenterPos;

        SetLeftPosition(charObjects, frontLineCenterIndex, 3, charBattleStatusDic[frontLineCenterIndex].sizeType, ref charBattleStatusDic);
        SetRightPosition(charObjects, frontLineCenterIndex, 5, charBattleStatusDic[frontLineCenterIndex].sizeType, ref charBattleStatusDic);
    }

    void SetLeftPosition(GameObject[] charObjects, int centerNum, int num, SIZE_TYPE sizeType, ref Dictionary<int, Battle_Character_Status> charBattleStatusDic)
    {   
        Vector3 pos = new Vector3(0, 0, 0);
        pos = charObjects[centerNum].transform.position;

        float x = 0, z = 0;

        GetCharOffset(sizeType, charBattleStatusDic[num].sizeType, ref x, ref z);
 
        pos.x += -x;
        pos.z += -z;
        charObjects[num].transform.position = pos;
    }
    void SetRightPosition(GameObject[] charObjects, int centerNum, int num, SIZE_TYPE sizeType, ref Dictionary<int, Battle_Character_Status> charBattleStatusDic)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        pos = charObjects[centerNum].transform.position;
        float x=0, z=0;

        GetCharOffset(sizeType, charBattleStatusDic[num].sizeType, ref x, ref z);
       
        pos.x += +x;
        pos.z += -z;
        charObjects[num].transform.position = pos;
    }
    void GetCharOffset(SIZE_TYPE centerSize, SIZE_TYPE mySize, ref float x, ref float z)
    {
        switch (centerSize) //기준점
        {
            case SIZE_TYPE.SMALL:
                {
                    if (mySize == SIZE_TYPE.SMALL)
                    {
                        x = 1.0f;
                        z = 0.0f;
                    }
                    else if (mySize == SIZE_TYPE.MIDDLE)
                    {
                        x = 1.25f;
                        z = 0.25f;
                    }
                    else
                    {
                        x = 1.5f;
                        z = 0.0f;
                    }
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    if (mySize == SIZE_TYPE.SMALL)
                    {
                        x = 1.25f;
                        z = 0.25f;
                    }


                    else if (mySize == SIZE_TYPE.MIDDLE)
                    {
                        x = 1.5f;
                        z = 0.0f;
                    }
                    else
                    {
                        x = 1.75f;
                        z = 0.25f;
                    }

                  
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                    if (mySize == SIZE_TYPE.SMALL)
                    {
                        x = 1.5f;
                        z = 0.0f;
                    }
                      
                    else if (mySize == SIZE_TYPE.MIDDLE)
                    {
                        x = 1.75f;
                        z = 0.25f;
                    }
                       
                    else
                    {

                        x = 2.0f;
                        z = 0.0f;
                    }
                    break;
                }
        }

    }


    float GetBackLineLargestDistance(ref Dictionary<int, Battle_Character_Status> charBattleStatusDic)
    {
        int num = 0;
        float dis = 0.0f;
        for (int i = 0; i < charBattleStatusDic.Count / 2; i++)
        {
            if(enemyObject[i].transform.position.z < enemyObject[num].transform.position.z)
            {
                num = i;
            }
        }
        Debug.Log("**** 가장 큰 인덱스 : " + num);
        dis = enemyObject[num].transform.position.z;
        //dis -= 0.5f
        switch (charBattleStatusDic[num].sizeType)
        {
            case SIZE_TYPE.SMALL:
                {
                    Debug.Log("*****스몰");
                    //dis -= 0.25f + 0.5f + 0.5f;
                    dis -=   0.5f + 0.5f;
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    Debug.Log("*****미들");
                    dis -= 0.25f + 0.5f + 0.5f;
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                    Debug.Log("*****빅");
                    dis -= 0.5f + 0.5f + 0.5f;
                    break;
                }
        }

        return dis;

    }



    Vector3 GetBackLineCenterCharPos(SIZE_TYPE sizeType)
    {
        Vector3 pos = new Vector3(0.5f, 0f, 5f);
        float offset = 0.25f;
        switch (sizeType)
        {
            case SIZE_TYPE.SMALL:
                {

                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    pos = new Vector3(0.5f - offset, 0f, 5.0f - offset);
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                   // pos = new Vector3(0.5f - offset, 0f, 4 - offset);

                    break;
                }
            default:
                {
                    pos = new Vector3(0.5f, 0f, 5f);
                    break;
                }

        }

        return pos;
    }
    Vector3 GetFrontLineCenterCharPos(SIZE_TYPE sizeType)
    {
        float middleoffset = 0.25f; //중형 크기이면 칸에 맞추기 위해 움직인다.
        float offset = 0.0f; // 얼마나 떨어질지 결정.
        Vector3 pos = new Vector3(0.5f, 0f, 5f);
        
        switch (sizeType)
        {
            case SIZE_TYPE.SMALL:
                {
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    pos = new Vector3(0.5f - middleoffset, 0f, 5.0f - middleoffset - offset);
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                   // pos = new Vector3(0.5f - middleoffset, 0f, 4 - middleoffset);
                    break;
                }
            default:
                {
                    pos = new Vector3(0.5f, 0f, 4f);
                    break;
                }
        }
        return pos;
    }

    float GetBigOffset(ref Dictionary<int, Battle_Character_Status> charBattleStatusDic )
    {
        bool isBigBackLine = false;
        bool isBigFrontLine = false;
        for(int i=0; i< charBattleStatusDic.Count/2; i++)
        {
            if(charBattleStatusDic[i].sizeType == SIZE_TYPE.BIG)
            {
                isBigBackLine = true;
                break;
            }
        }
        for (int i = charBattleStatusDic.Count / 2; i < charBattleStatusDic.Count; i++)
        {
            if (charBattleStatusDic[i].sizeType == SIZE_TYPE.BIG)
            {
                isBigFrontLine = true;
                break;
            }
        }

        if(isBigBackLine && isBigFrontLine)
        {
            return 0.5f;
        }
        return 0.0f;

    }


    public void TestSetPosition()
    {
        SetBattolePosition(enemyObject, ref enemyStatusDic);
    }

    public void RestBattle()
    {
        for(int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
        {
            Destroy(playerObject[i]);
            Destroy(enemyObject[i]);
        }
        Destroy(playerParty);
        Destroy(enemyParty);
       attackOrder = -1;
        BattleState = BATTLE_STATE.NONE;
        MouseClick = false;

        playerStatusDic.Clear();
        enemyStatusDic.Clear();
        turnActionList.Clear();

        CreateGameObject();
        SetTurnSpeed();
    }
}
