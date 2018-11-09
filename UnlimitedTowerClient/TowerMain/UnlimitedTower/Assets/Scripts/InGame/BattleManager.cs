using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoSingleton<BattleManager> {

    // TODO : Test Prefabs if Delete
    // 인게임 보여주는 용도의 오브젝트
    public GameObject[] playerObjects = new GameObject[DEFINE.PARTY_MAX_NUM];
    public GameObject[] enemyObjects = new GameObject[DEFINE.PARTY_MAX_NUM];

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


    public int AttackOrder = -1;
    public GameObject AttackObject;
    public GameObject TargetObject;
    public CharacterAction AttackerAction;

    GameObject PlayerParty;
    GameObject EnemyParty;

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
        if (Input.GetMouseButtonDown(0) && MouseClick == false && BattleState != BATTLE_STATE.BATTLE)
        {
            MouseClick = true;
            Debug.Log("공격 시작");
            // 공격전 준비.
            ReadyNextTurn();
            // 전투 씬 시작.
            BattleState = BATTLE_STATE.BATTLE;
        }
    }


    private bool CheckGameOver()
    {
        if (enemyStatusDic.Count == 0 || playerStatusDic.Count == 0)
        {
            Debug.Log("게임 끝 : " );
            BattleState = BATTLE_STATE.END;
            AttackOrder = -1;
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
            myObject = playerObjects[myIndex];
            TargetObject = enemyObjects[targetNum];
        }
        else
        {
            myObject = enemyObjects[myIndex];
            TargetObject = playerObjects[targetNum];
        }   
    }

    public bool CheckAttackCycle()
    {
        if (AttackOrder >= enemyStatusDic.Count + playerStatusDic.Count)
        {
            // 두 개의 dic의 크기를 넘으면 모든 캐릭터가 한번씩 했음.
            Debug.Log("1턴 종료 -> attackaNum : " + AttackOrder);
            BattleState = BATTLE_STATE.NONE;
            AttackOrder = -1;
            MouseClick = false;
            return true;
        }
        return false;
    }

    public void DecreaseHp(CHAR_TYPE charType, int myIndex, int targetIndex)
    {
       if(charType != CHAR_TYPE.PLAYER)
        {
            RemoveBattleDic(ref enemyStatusDic, ref playerStatusDic, playerObjects, myIndex, targetIndex);         
        }
        else
        {
            RemoveBattleDic(ref playerStatusDic, ref enemyStatusDic, enemyObjects, myIndex, targetIndex);
        }     
    }

    public void ReadyNextTurn()
    {
        ++AttackOrder;
        CheckCharIsLive();
        if (CheckGameOver())
        {
            return;
        }
        if (AttackOrder < enemyStatusDic.Count + playerStatusDic.Count)
        {

            AttackerAction = turnActionList[AttackOrder];


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
        PlayerParty = new GameObject("PlayerParty");
        EnemyParty = new GameObject("EnemyParty");
        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            if (CharacterManager.Inst.characterDic.ContainsKey(i) == false)
            {
                return;
            }
            //캐릭터 정보,                     //partyIndex   //chartIndex(필요없는 값일 수도 있음_
            Battle_Character_Status status = new Battle_Character_Status(CharacterManager.Inst.characterDic[i], i, i, 0);
            playerStatusDic.Add(i, status);
            playerObjects[i] = Instantiate(GetCharacterObject(status.character.Index), new Vector3(), Quaternion.identity);         
            playerObjects[i].GetComponent<CharController>().charType = CHAR_TYPE.PLAYER;
            playerObjects[i].GetComponent<CharController>().charSize = status.sizeType;
            playerObjects[i].transform.SetParent(PlayerParty.transform.transform, false);
            //playerObject[i].GetComponent<CharController>().battleDicIndex = i;
        }
        SetBattlePosition(playerObjects, CHAR_TYPE.PLAYER, ref playerStatusDic);
        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            playerObjects[i].GetComponent<CharController>().SetFirstPosition();
        }
            


        // TODO : DB 대신 임시로
        Dictionary<int, Character> enemyDic = TestDB.LoadMonstersData();

        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            if (enemyDic.ContainsKey(i) == false)
            {
                return;
            }
            Battle_Character_Status status = new Battle_Character_Status(enemyDic[i], i, i, enemyDic[i].Size);
            enemyStatusDic.Add(i, status);


            enemyObjects[i] = Instantiate(GetCharacterObject(status.character.Index), new Vector3(), Quaternion.Euler(new Vector3(0, 180, 0)));
            enemyObjects[i].GetComponent<CharController>().charType = CHAR_TYPE.ENEMY;
            enemyObjects[i].GetComponent<CharController>().battleDicIndex = i;
            enemyObjects[i].GetComponent<CharController>().charSize = status.sizeType;
            enemyObjects[i].transform.SetParent(EnemyParty.transform.transform, false);
        }
        SetBattlePosition(enemyObjects, CHAR_TYPE.ENEMY, ref enemyStatusDic);

        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            enemyObjects[i].GetComponent<CharController>().SetFirstPosition();
        }
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

    Vector3 GetBackLineCenterCharPos(CHAR_TYPE charType, SIZE_TYPE sizeType)
    {
        Vector3 pos = DEFINE.PLAYER_BACKLINE_CENTER_POS;
        if (charType == CHAR_TYPE.ENEMY)
        {
            pos = DEFINE.ENEMY_BACKLINE_CENTER_POS;
        }


        float middleOffset = 0.25f;
        switch (sizeType)
        {
            case SIZE_TYPE.SMALL:
                {
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    pos.x += +middleOffset;
                    pos.z += -middleOffset;
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                    pos.x += +middleOffset * 2.0f;
                    pos.z += -middleOffset * 2.0f;
                    break;
                }   
        }
        return pos;
    }
    Vector3 GetFrontLineCenterCharPos(Vector3 frontCenterPos, SIZE_TYPE sizeType)
    {
        float middleOffset = 0.25f; //중형 크기이면 칸에 맞추기 위해 움직인다.
        Vector3 pos = frontCenterPos;

        switch (sizeType)
        {
            case SIZE_TYPE.SMALL:
                {
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    pos.x += middleOffset;
                    pos.z += -middleOffset;
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                    pos.x += middleOffset * 2;
                    pos.z += -middleOffset * 2;
                    break;
                }         
        }
        return pos;
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
                        z = 0.5f;
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
                        z = 0.5f;
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
    void SetLeftPosition(GameObject[] charObjects, int centerNum, int num, SIZE_TYPE sizeType, ref Dictionary<int, Battle_Character_Status> charBattleStatusDic)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        float x = 0, z = 0;
        float sign = 1.0f;
        pos = charObjects[centerNum].transform.position;
   
        GetCharOffset(sizeType, charBattleStatusDic[num].sizeType, ref x, ref z);
        if (sizeType < charBattleStatusDic[num].sizeType)
            sign = -1.0f;

        pos.x += -x;
        pos.z += +z * sign;
        charObjects[num].transform.position = pos;
    }
    void SetRightPosition(GameObject[] charObjects, int centerNum, int num, SIZE_TYPE sizeType, ref Dictionary<int, Battle_Character_Status> charBattleStatusDic)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        pos = charObjects[centerNum].transform.position;
        float x = 0, z = 0;

        float sign = 1.0f;
        GetCharOffset(sizeType, charBattleStatusDic[num].sizeType, ref x, ref z);
        if (sizeType < charBattleStatusDic[num].sizeType)
            sign = -1.0f;


        pos.x += +x;
        pos.z += +z * sign;
        charObjects[num].transform.position = pos;
    }
    float GetBackLineLargestDistance(GameObject[] charObjects, CHAR_TYPE charType, ref Dictionary<int, Battle_Character_Status> charBattleStatusDic)
    {
        int num = 0;
        float dis = 0.0f;
        float offset = -2.0f;
        if (charType == CHAR_TYPE.ENEMY)
        {
            offset = +2.0f;
        }

        for (int i = 0; i < charBattleStatusDic.Count / 2; i++)
        {
            if (charObjects[i].transform.position.z < charObjects[num].transform.position.z)
            {
                num = i;
            }
        }
        Debug.Log("**** 가장 큰 인덱스 : " + num);
        dis = charObjects[num].transform.position.z;
        switch (charBattleStatusDic[num].sizeType)
        {
            case SIZE_TYPE.SMALL:
                {
                    Debug.Log("*****스몰");
                    //dis -= 0.25f + 0.5f + 0.5f;
                    dis -= offset;
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    Debug.Log("*****미들");
                    dis -= 0.25f + offset;
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                    Debug.Log("*****빅");
                    dis -= 0.5f + offset;
                    break;
                }
        }
        return dis;
    }
    void SetBattlePosition(GameObject[] charObjects, CHAR_TYPE charType, ref Dictionary<int, Battle_Character_Status> charBattleStatusDic)
    {
        int backLineCenterIndex = charBattleStatusDic.Count / 2 / 2;
        int frontLineCenterIndex = charBattleStatusDic.Count / 2 / 2 + charBattleStatusDic.Count / 2;

        Battle_Character_Status centerCharStatus = charBattleStatusDic[backLineCenterIndex];
        Vector3 backCenterPos = GetBackLineCenterCharPos(charType, centerCharStatus.sizeType);
        Vector3 frontCenterPos = DEFINE.PLAYER_BACKLINE_CENTER_POS;
        if (charType == CHAR_TYPE.ENEMY)
        {
            frontCenterPos = DEFINE.ENEMY_BACKLINE_CENTER_POS;
        }

        charObjects[backLineCenterIndex].transform.position = backCenterPos;
        SetLeftPosition(charObjects, backLineCenterIndex, 0, centerCharStatus.sizeType, ref charBattleStatusDic);
        SetRightPosition(charObjects, backLineCenterIndex, 2, centerCharStatus.sizeType, ref charBattleStatusDic);


     
        float frontLineDis = GetBackLineLargestDistance(charObjects, charType, ref charBattleStatusDic);
        frontCenterPos.z = frontLineDis;
        frontCenterPos = GetFrontLineCenterCharPos(frontCenterPos, charBattleStatusDic[frontLineCenterIndex].sizeType);



        Debug.Log("z값 중심 : " + frontLineDis);
        charObjects[frontLineCenterIndex].transform.position = frontCenterPos;
        SetLeftPosition(charObjects, frontLineCenterIndex, 3, charBattleStatusDic[frontLineCenterIndex].sizeType, ref charBattleStatusDic);
        SetRightPosition(charObjects, frontLineCenterIndex, 5, charBattleStatusDic[frontLineCenterIndex].sizeType, ref charBattleStatusDic);
    }









    public void TestSetPosition()
    {
        SetBattlePosition(enemyObjects, CHAR_TYPE.ENEMY, ref enemyStatusDic);
    }

    public void RestBattle()
    {
        for(int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
        {
            Destroy(playerObjects[i]);
            Destroy(enemyObjects[i]);
        }
        Destroy(PlayerParty);
        Destroy(EnemyParty);
        AttackOrder = -1;
        BattleState = BATTLE_STATE.NONE;
        MouseClick = false;

        playerStatusDic.Clear();
        enemyStatusDic.Clear();
        turnActionList.Clear();

        CreateGameObject();
        SetTurnSpeed();
    }
}
