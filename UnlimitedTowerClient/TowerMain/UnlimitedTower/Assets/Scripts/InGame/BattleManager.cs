using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoSingleton<BattleManager> {

    #region event listeners 
    [System.Serializable]
    public class EventListener : UnityEvent { }

    public EventListener onBattleClear = new EventListener();
    public EventListener onBattleFailed = new EventListener();

    public EventListener onBattlePause = new EventListener();
    public EventListener onBattleReset = new EventListener();

    public EventListener onBattleStarted = new EventListener();
    public EventListener onBattleExited = new EventListener();

    #endregion

    // TODO : Test Prefabs if Delete

    // 인게임 보여주는 용도의 오브젝트
    public GameObject[] playerObjects = new GameObject[DEFINE.PARTY_MAX_NUM];
    public GameObject[] enemyObjects = new GameObject[DEFINE.PARTY_MAX_NUM];

    // 실제 배틀하는 게임 데이터를 담고 있는 Dic
    public Dictionary<int, CharacterBattleStatus> playerStatusDic = new Dictionary<int, CharacterBattleStatus>();
    public Dictionary<int, CharacterBattleStatus> enemyStatusDic = new Dictionary<int, CharacterBattleStatus>();
    public List<CharacterAction> turnActionList = new List<CharacterAction>();

    // Delegates for receiving event
    public delegate void DecreaseHpDelegate(int demage, GameObject targetGo);
    public event DecreaseHpDelegate decreaseHpDelegate = null;


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
        if (debugFlag == true)
        {
            // TODO : Test
            // debugPanel.SetActive(true);
            debugText.text = "Debug ";
        }

       SetObject();
       SetTurnSpeed();
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
               // StartBattle();
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
    public void StartBattle()
    {
        //if (Input.GetMouseButtonDown(0) && MouseClick == false && BattleState != BATTLE_STATE.BATTLE)
        {
            onBattleStarted.Invoke();

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


            /// 임시로 여기서 이벤트를 받습니다.
            /// 기본적으로 Check 함수에서는 어떤 결과를 알수 있는지만 리턴하고 Check함수 밖에서 모든 행동을 처리합니다.
            /// 
            if(enemyStatusDic.Count == 0)
                onBattleClear.Invoke();

            if (playerStatusDic.Count == 0)
                onBattleFailed.Invoke();

            return true;
        }
        return false;
    }

    private void CheckTargetIsLive(FORMATION_TYPE formationType)
    {
        if (AttackerAction.formationType == FORMATION_TYPE.PLAYER)
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
            if(turnActionList[i].formationType == FORMATION_TYPE.PLAYER)
            {
                if (!playerStatusDic.ContainsKey(turnActionList[i].myIndex))
                {
                    turnActionList.RemoveAt(i);
                }                 
            }
            else
            {
                if (!enemyStatusDic.ContainsKey(turnActionList[i].myIndex))
                {
                    turnActionList.RemoveAt(i);
                }
            }
        }
    }

  

    private void SetObject()
    {
        // TODO : Test Setting if Delete
        if (!FirstAcess)
        {
            FirstAcess = true;
            Debug.Log("First BattleScene");
        }
        
        CreateGameObject();
    }


   
    private GameObject GetCharacterObject(int num)
    {
        return Resources.Load("Prefabs/" + ( (CHARACTER_NUM)num ).ToString() ) as GameObject;
    }

    public void SetTurnSpeed()
    {
        //공격대상, 공격자, 공격하는 타입, 공격자 타입등을 알려줌.
        foreach (KeyValuePair<int, CharacterBattleStatus> dic in playerStatusDic)
        {
            CharacterAction temp1 = new CharacterAction(dic.Key, GetTargetIndex(FORMATION_TYPE.ENEMY), ACTION_TYPE.Attack, FORMATION_TYPE.PLAYER);
            turnActionList.Add(temp1);
        }

        foreach (KeyValuePair<int, CharacterBattleStatus> dic in enemyStatusDic)
        {
            CharacterAction temp1 = new CharacterAction(dic.Key, GetTargetIndex(FORMATION_TYPE.PLAYER), ACTION_TYPE.Attack, FORMATION_TYPE.ENEMY);
            turnActionList.Add(temp1);
        }

        turnActionList.Sort(SpeedComparer);
    }
    

    public int GetTargetIndex(FORMATION_TYPE formationType)
    {
        int targetIndex = Random.Range(0, DEFINE.PARTY_MAX_NUM);
        if (formationType == FORMATION_TYPE.PLAYER)
        {
            while (!playerStatusDic.ContainsKey(targetIndex))
            {
                targetIndex = Random.Range(0, DEFINE.PARTY_MAX_NUM);          
            }
        }
        else
        {         
            while (!enemyStatusDic.ContainsKey(targetIndex))
            {
                targetIndex = Random.Range(0, DEFINE.PARTY_MAX_NUM);              
            }
        }
        return targetIndex;
    }

    public CharacterBattleStatus GetChar(FORMATION_TYPE formationType, int index)
    {
        if(formationType == FORMATION_TYPE.PLAYER)
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

    public int SpeedComparer(CharacterAction x, CharacterAction y)
    {
        int xSpeed;
        int ySpeed;
        if(x.formationType == FORMATION_TYPE.PLAYER)
        {
            xSpeed = playerStatusDic[x.myIndex].speed;
        }
        else
        {
            xSpeed = enemyStatusDic[x.myIndex].speed;

        }

        if (y.formationType == FORMATION_TYPE.PLAYER)
        {
            ySpeed = playerStatusDic[y.myIndex].speed;
        }
        else
        {
            ySpeed = enemyStatusDic[y.myIndex].speed;

        }

        return ySpeed.CompareTo(xSpeed);
    }


    #region BattleLogic Funcs

    public void GetCharObject(FORMATION_TYPE formationType, int myIndex,  ref GameObject myObject, int targetNum, ref GameObject TargetObject)
    {
        if (formationType == FORMATION_TYPE.PLAYER)
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
            Debug.Log("모든 캐릭 한 턴 종료 -> attackaNum : " + AttackOrder);
            BattleState = BATTLE_STATE.NONE;
            AttackOrder = -1;
            MouseClick = false;
            return true;
        }
        return false;
    }
    public void CheckCharBeHit(FORMATION_TYPE formationType, int myIndex, int targetIndex)
    {
       if(formationType != FORMATION_TYPE.PLAYER)
        {
            DecreaseHp(ref enemyStatusDic, ref playerStatusDic, playerObjects, myIndex, targetIndex);         
        }
        else
        {
            DecreaseHp(ref playerStatusDic, ref enemyStatusDic, enemyObjects, myIndex, targetIndex);
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
            CheckTargetIsLive(AttackerAction.formationType);

      
            GetCharObject(AttackerAction.formationType, AttackerAction.myIndex, ref AttackObject,  AttackerAction.targetIndex, ref TargetObject);
            // 현재 공격하는 오브젝트를 실행한다
            CharController charController = AttackObject.GetComponent<CharController>();
            charController.StartMyTurn(STATE_TYPE.RUN, TargetObject, AttackerAction);

        }
    }
    
    private void DecreaseHp(ref Dictionary<int, CharacterBattleStatus> attackDic, ref Dictionary<int, CharacterBattleStatus> targetDic, GameObject[] charObjects, int myIndex, int targetIndex)
    {
        CharacterBattleStatus targetStatus = null;
        CharacterBattleStatus attackStatus = null;

        if(targetIndex ==7)
        {
            Debug.Log("히어로 피격당함");
        }

        if(targetDic.TryGetValue(targetIndex, out targetStatus) && targetStatus != null &&
            attackDic.TryGetValue(myIndex, out attackStatus) && attackStatus != null && 
            targetIndex < charObjects.Length)
        {
            var targetObject = charObjects[targetIndex];
            var targetController = targetObject.GetComponent<CharController>();

            targetStatus.nowHp -= attackStatus.damage;
            if (targetStatus.nowHp <= 0)
            {
                targetStatus.nowHp = 0;
                if (targetController != null)
                {
                    targetController.PlayDeadAnimation();
                }

                // 죽으면..
                targetDic.Remove(targetIndex);
                Debug.Log("플레이어 : " + targetIndex + "삭제 실행");
            }
            else
            {
                if(targetController != null)
                {
                    targetController.PlayHitAnimation();
                }

                Debug.Log("플레이어 : " + targetIndex + "피격 애니 실행");
            }

            if (decreaseHpDelegate != null)
            {
                decreaseHpDelegate(attackStatus.damage, targetObject);
            }
        }
        else
        {
            Debug.LogWarning("[DecreaseHp]" +
                "Logger : DH\n" +
                "Has targetDic the key(" + targetIndex + ") ? : " + targetDic.ContainsKey(targetIndex) + "" +
                "Has attackDic the key(" + myIndex + ") ? : " + attackDic.ContainsKey(myIndex) + "" +
                "Lenght of charObjects : " + charObjects.Length + "");
        }

        
    }

    private void CheckTargetDic(ref Dictionary<int, CharacterBattleStatus> charStatusDic)
    {
        if (charStatusDic.ContainsKey(AttackerAction.targetIndex))
        {
            return;
        }
        else
        {
            if(AttackerAction.formationType == FORMATION_TYPE.PLAYER)
            {
                AttackerAction.targetIndex = GetTargetIndex(FORMATION_TYPE.ENEMY);
            }
            else
            {
                AttackerAction.targetIndex = GetTargetIndex(FORMATION_TYPE.PLAYER);
            }
        }
    }


    #endregion

    private void CreateGameObject()
    {
        Debug.Log(" CreateGameObject()");
        PlayerParty = new GameObject("PlayerParty");
        EnemyParty = new GameObject("EnemyParty");

        // 배틀씬에서 바로 시작할 경우 플레이어 캐릭터들을 초기화해준다.
        if (UserDataManager.Inst.GetSceneState() == SCENE_STATE.None)
        {
            Debug.Log("Only BattleScene Start");
            Test_CreatePlayerObjects();
            SetCharBattlePosition(playerObjects);
        }
        else
        {
            CreatePlayerObjects(1);
            SetCharBattlePosition(playerObjects);
        }

        // 임의의 몬스터 생성
        CreateEnemyObjects();
        CreateEnemyBattlePosition(enemyObjects, FORMATION_TYPE.ENEMY, ref enemyStatusDic);
        SetCharBattlePosition(enemyObjects);

        // TODO : Stage 정보에 따른 몬스터 생성 필요

    }

    // 배틀씬 테스트용 함수
    private void Test_CreatePlayerObjects()
    {
        List<int> characterList = new List<int>
        {
           105, 106, 107, 108, 108,  220, 225, 226, 205, 202
        };   

        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            int formationNum = i;
            CharacterBattleStatus status;
            int charIndex = -1;
      
            if (i < 5)
            {
                Servant servant = new Servant(characterList[i]);
                status = new CharacterBattleStatus();
                charIndex = servant.appear;
            }
            else
            {
                Monster monster = new Monster(characterList[i]);
                status = new CharacterBattleStatus();
                charIndex = monster.index;
            }
            playerStatusDic.Add(formationNum, status);
            if (playerObjects[formationNum] == null)
            {
                playerObjects[formationNum] = Instantiate(GetCharacterObject(charIndex), new Vector3(), Quaternion.identity);

                playerObjects[formationNum].transform.SetParent(PlayerParty.transform.transform, false);
                if (playerObjects[formationNum].GetComponent<CharController>())
                {
                    playerObjects[formationNum].GetComponent<CharController>().status = status;
                    playerObjects[formationNum].GetComponent<CharController>().formationType = FORMATION_TYPE.PLAYER;
                    playerObjects[formationNum].GetComponent<CharController>().charSize = status.sizeType;

                    CreatePlayerBsttlePosition(formationNum, playerObjects, FORMATION_TYPE.PLAYER, ref playerStatusDic);
                    Debug.Log(formationNum + " CharSize : " + status.sizeType);
                    playerObjects[formationNum].GetComponent<CharController>().battleDicIndex = formationNum;
                }
            }

        }
    }

    private void CreatePlayerObjects(int partyNum)
    {
        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            int formationNum = i;
            PartyCharacterInfo charInfo = new PartyCharacterInfo();
            if (UserDataManager.Inst.partyDic.ContainsKey(partyNum))
            {
                charInfo = UserDataManager.Inst.partyDic[partyNum].characterList[formationNum];
                if(charInfo == null)
                {
                    break;
                }
            }
            else
            {
                break;
            }

            CharacterBattleStatus battleStatus = new CharacterBattleStatus();
            battleStatus = GetBattleStatus(charInfo);
            if (battleStatus == null)
            {
                Debug.Log("Error  : Acollcate BattleStatue");
                break;
            }

            playerStatusDic.Add(formationNum, battleStatus);

            if (!playerObjects[formationNum])
            {
                playerObjects[formationNum] = Instantiate(GetCharacterObject(battleStatus.index), new Vector3(), Quaternion.identity);

                playerObjects[formationNum].transform.SetParent(PlayerParty.transform.transform, false);
                if (playerObjects[formationNum].GetComponent<CharController>())
                {
                    playerObjects[formationNum].GetComponent<CharController>().status = battleStatus;
                    playerObjects[formationNum].GetComponent<CharController>().formationType = FORMATION_TYPE.PLAYER;
                    playerObjects[formationNum].GetComponent<CharController>().charSize = battleStatus.sizeType;

                    CreatePlayerBsttlePosition(formationNum, playerObjects, FORMATION_TYPE.PLAYER, ref playerStatusDic);
                    Debug.Log(formationNum + " CharSize : " + battleStatus.sizeType);
                    playerObjects[formationNum].GetComponent<CharController>().battleDicIndex = formationNum;
                }
            }

        }
    }

    private CharacterBattleStatus GetBattleStatus(PartyCharacterInfo getCharInfo)
    {
        if(getCharInfo.type == CHAR_TYPE.HERO)
        {
            Servant heroInfo = UserDataManager.Inst.GetHeroInfo();
            if(heroInfo == null)
            {
                return null;
            }

            CharacterBattleStatus battleStatus = GetServantBattleStatus(heroInfo);

            return battleStatus;
        }
        else if (getCharInfo.type == CHAR_TYPE.SERVANT)
        {
            Servant servantInfo = UserDataManager.Inst.GetServantInfo(getCharInfo.index);
            if (servantInfo == null)
            {
                return null;
            }

            CharacterBattleStatus battleStatus = GetServantBattleStatus(servantInfo);

            return battleStatus;

        }
        else if(getCharInfo.type == CHAR_TYPE.MONSTER)
        {
            Monster monsterInfo = UserDataManager.Inst.GetMonsterInfo(getCharInfo.index);
            if(monsterInfo == null)
            {
                return null;
            }

            CharacterBattleStatus battleStatus = GetMonsterBattleStatus(monsterInfo);

            return battleStatus;
        }
        else
        {
            Debug.Log("Invalid GetCharInfo Type : " + getCharInfo.type.ToString());
            return null;
        }
    }

    public CharacterBattleStatus GetServantBattleStatus(Servant getServantInfo)
    {
        CharacterBattleStatus battleStatus = new CharacterBattleStatus();

        return battleStatus;
    }

    public CharacterBattleStatus GetMonsterBattleStatus(Monster getMonsterInfo)
    {
        CharacterBattleStatus battleStatus = new CharacterBattleStatus();

        return battleStatus;
    }

    private void CreateEnemyObjects()
    {
        // TODO :  Test Enemy Setting 코드 필요
        Dictionary<int, Monster> enemyDic = new Dictionary<int, Monster>();

        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            if (enemyDic.ContainsKey(i) == false)
            {
                break;
            }
            else
            {
                CharacterBattleStatus status = new CharacterBattleStatus();
                enemyStatusDic.Add(i, status);

                int monsterIndex = enemyDic[i].index;

                if (!enemyObjects[i])
                {
                    enemyObjects[i] = Instantiate(GetCharacterObject(monsterIndex), new Vector3(), Quaternion.Euler(new Vector3(0, 180, 0)));
                    enemyObjects[i].transform.SetParent(EnemyParty.transform.transform, false);
                    if (enemyObjects[i].GetComponent<CharController>())
                    {
                        enemyObjects[i].GetComponent<CharController>().status = status;
                        enemyObjects[i].GetComponent<CharController>().formationType = FORMATION_TYPE.ENEMY;
                        enemyObjects[i].GetComponent<CharController>().battleDicIndex = i;
                        enemyObjects[i].GetComponent<CharController>().charSize = status.sizeType;
                    }
                }
            }
        }
    }




    #region CharacterPosition Setting Funcs

    private float GetBigOffset(ref Dictionary<int, CharacterBattleStatus> charBattleStatusDic )
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

    private Vector3 GetBackLineCenterCharPos(FORMATION_TYPE formationType, SIZE_TYPE sizeType)
    {
        Vector3 pos = DEFINE.PLAYER_BACKLINE_CENTER_POS;
        if (formationType == FORMATION_TYPE.ENEMY)
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
    private Vector3 GetFrontLineCenterCharPos(Vector3 frontCenterPos, FORMATION_TYPE formationType, SIZE_TYPE sizeType)
    {
        float middleOffset = 0.25f; //중형 크기이면 칸에 맞추기 위해 움직인다.
        Vector3 pos = frontCenterPos;
        
        // ### 수정할것
        if(formationType == FORMATION_TYPE.PLAYER)
        {
            middleOffset *= -1.0f;
        }
 
        switch (sizeType)
        {
            case SIZE_TYPE.SMALL:
                {
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    pos.x += middleOffset;
                    pos.z -= middleOffset;
                    //pos.z += -middleOffset;
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                    pos.x += middleOffset * 2;
                    pos.z -= middleOffset * 2;
                    //pos.z += -middleOffset * 2;
                    break;
                }         
        }
        // x: 0.25, z : -3.25
        Debug.Log("앞 라인 중앙 pos.x : " + pos.x + ", pos.z : " + pos.z);
        return pos;
    }
    private void GetCharOffset(SIZE_TYPE centerSize, SIZE_TYPE mySize, ref float x, ref float z)
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
    private void SetLeftPosition(GameObject[] charObjects, int centerNum, int num, FORMATION_TYPE formationType, SIZE_TYPE standardCharSizeType, ref Dictionary<int, CharacterBattleStatus> charBattleStatusDic)
    {
        if (charBattleStatusDic.ContainsKey(num) == false)
        {
            return;
        }

        Vector3 pos = new Vector3(0, 0, 0);
        float x = 0, z = 0;
        float sign = 1.0f;

        pos = charObjects[centerNum].transform.position;
   
        GetCharOffset(standardCharSizeType, charBattleStatusDic[num].sizeType, ref x, ref z);
        // 기준보다 내가 크면
        // 기준이 더 작을 때 문제가 생기는거 같다.


        if (formationType == FORMATION_TYPE.ENEMY)
        {
            if (standardCharSizeType < charBattleStatusDic[num].sizeType)
            {
                sign = -1.0f;
            }
        }
        else
        {
            if (standardCharSizeType > charBattleStatusDic[num].sizeType)
            {
                sign = -1.0f;
            }
        }

        pos.x += -x;
        pos.z += +z * sign;
        charObjects[num].transform.position = pos;
    }
    private void SetRightPosition(GameObject[] charObjects, int centerNum, int num, FORMATION_TYPE formationType, SIZE_TYPE standardCharSizeType, ref Dictionary<int, CharacterBattleStatus> charBattleStatusDic)
    {
        if (charBattleStatusDic.ContainsKey(num) == false)
        {
            return;
        }

        Vector3 pos = new Vector3(0, 0, 0);
        pos = charObjects[centerNum].transform.position;
        float x = 0, z = 0;

        float sign = 1.0f;
        GetCharOffset(standardCharSizeType, charBattleStatusDic[num].sizeType, ref x, ref z);
        if (formationType == FORMATION_TYPE.ENEMY)
        {
            if (standardCharSizeType < charBattleStatusDic[num].sizeType)
            {
                sign = -1.0f;
            }
        }
        else
        {
            if (standardCharSizeType > charBattleStatusDic[num].sizeType)
            {
                sign = -1.0f;
            }
        }


        pos.x += +x;
        pos.z += +z * sign;
        charObjects[num].transform.position = pos;
    }
    private float GetBackLineLargestDistance(GameObject[] charObjects, FORMATION_TYPE formationType, ref Dictionary<int, CharacterBattleStatus> charBattleStatusDic)
    {
        int num = 5;
        float dis = 0.0f;
        // 앞라인과 뒷 라인의 간격 offset
        float offset = -2.0f;
        if (formationType == FORMATION_TYPE.ENEMY)
        {
            offset = +2.0f;
        }

        if(charBattleStatusDic.Count <=5)
        {
            num = 7;
        }
        else
        {
            for (int i = 5; i < charBattleStatusDic.Count; i++)
            {
                if (charObjects[i].transform.position.z < charObjects[num].transform.position.z)
                {
                    num = i;
                }
            }
        }
        
        dis = charObjects[num].transform.position.z;
        switch (charBattleStatusDic[num].sizeType)
        {
            case SIZE_TYPE.SMALL:
                {                              
                    dis -= offset;
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {                
                    dis -= 0.25f + offset;
                    break;
                }
            case SIZE_TYPE.BIG:
                {                
                    dis -= 0.5f + offset;
                    break;
                }
        }
        return dis;
    }

    private void CreatePlayerBsttlePosition(int formationOrder, GameObject[] charObjects, FORMATION_TYPE formationType, ref Dictionary<int, CharacterBattleStatus> charBattleStatusDic)
    {
        Vector3 frontCenterPos = DEFINE.PLAYER_BACKLINE_CENTER_POS;

        CharacterBattleStatus centerCharStatus = charBattleStatusDic[7];
        int frontLineCenterIndex = 2;
        int backLineCenterIndex = 7;
        switch (formationOrder)
        {
            case 7:
                {
                    Vector3 backCenterPos = GetBackLineCenterCharPos(formationType, centerCharStatus.sizeType);
                    charObjects[formationOrder].transform.position = backCenterPos;                
                    break;
                }
            case 6:
                {
                    SetLeftPosition(charObjects, backLineCenterIndex, formationOrder, FORMATION_TYPE.PLAYER, charBattleStatusDic[backLineCenterIndex].sizeType, ref charBattleStatusDic);
                    break;
                }
            case 8:
                {
                    SetRightPosition(charObjects, backLineCenterIndex, formationOrder, FORMATION_TYPE.PLAYER, charBattleStatusDic[backLineCenterIndex].sizeType, ref charBattleStatusDic);
                    break;
                }
            case 5:
                {
                    SetLeftPosition(charObjects, 6, formationOrder, FORMATION_TYPE.PLAYER, charBattleStatusDic[6].sizeType, ref charBattleStatusDic);           
                    break;
                }
            case 9:
                {
                    SetRightPosition(charObjects, 8, formationOrder, FORMATION_TYPE.PLAYER, charBattleStatusDic[8].sizeType, ref charBattleStatusDic);
                    break;
                }

                // 몬스터 배치 부분으로 여기서 부턴 몬스터가 존재하지 않을 수도 있다.
            case 2:
                {
                    float frontLineDis = GetBackLineLargestDistance(charObjects, formationType, ref charBattleStatusDic);
                    frontCenterPos.z = frontLineDis;
                    frontCenterPos = GetFrontLineCenterCharPos(frontCenterPos, formationType, charBattleStatusDic[frontLineCenterIndex].sizeType);
                    Debug.Log("몬스터 센터  크기 : " + charBattleStatusDic[frontLineCenterIndex].sizeType);
                    Debug.Log("frontLineDis : " + frontLineDis);
                    charObjects[frontLineCenterIndex].transform.position = frontCenterPos;
                    break;
                }
            case 1:
                {
                    SetLeftPosition(charObjects, 2, formationOrder, FORMATION_TYPE.PLAYER, charBattleStatusDic[2].sizeType, ref charBattleStatusDic);
                    break;
                }
            case 3:
                {
                    SetRightPosition(charObjects, 2, formationOrder, FORMATION_TYPE.PLAYER, charBattleStatusDic[2].sizeType, ref charBattleStatusDic);
                    break;
                }
            case 0:
                {
                    SetLeftPosition(charObjects, 1, formationOrder, FORMATION_TYPE.PLAYER, charBattleStatusDic[1].sizeType, ref charBattleStatusDic);

                    break;
                }
            case 4:
                {
                    SetRightPosition(charObjects, 3, formationOrder, FORMATION_TYPE.PLAYER, charBattleStatusDic[3].sizeType, ref charBattleStatusDic);
                    break;
                }

        }
    }
    private void CreateEnemyBattlePosition(GameObject[] charObjects, FORMATION_TYPE formationType, ref Dictionary<int, CharacterBattleStatus> charBattleStatusDic)
    {
        int frontLineCenterIndex = charBattleStatusDic.Count / 2 / 2;
        int backLineCenterIndex = charBattleStatusDic.Count / 2 / 2 + charBattleStatusDic.Count / 2;



        CharacterBattleStatus centerCharStatus = charBattleStatusDic[backLineCenterIndex];
        Vector3 backCenterPos = GetBackLineCenterCharPos(formationType, centerCharStatus.sizeType);
        Vector3 frontCenterPos = DEFINE.PLAYER_BACKLINE_CENTER_POS;
        if (formationType == FORMATION_TYPE.ENEMY)
        {
            frontCenterPos = DEFINE.ENEMY_BACKLINE_CENTER_POS;
        }

        charObjects[backLineCenterIndex].transform.position = backCenterPos;

        SetLeftPosition(charObjects, backLineCenterIndex, 6, FORMATION_TYPE.ENEMY, centerCharStatus.sizeType, ref charBattleStatusDic);
        SetRightPosition(charObjects, backLineCenterIndex, 8, FORMATION_TYPE.ENEMY, centerCharStatus.sizeType, ref charBattleStatusDic);

        SetLeftPosition(charObjects, 6, 5, FORMATION_TYPE.ENEMY, charBattleStatusDic[6].sizeType, ref charBattleStatusDic);
        SetRightPosition(charObjects, 8, 9, FORMATION_TYPE.ENEMY, charBattleStatusDic[8].sizeType, ref charBattleStatusDic);


        float frontLineDis = GetBackLineLargestDistance(charObjects, formationType, ref charBattleStatusDic);
        frontCenterPos.z = frontLineDis;
        frontCenterPos = GetFrontLineCenterCharPos(frontCenterPos, formationType, charBattleStatusDic[frontLineCenterIndex].sizeType);

        if(charObjects[frontLineCenterIndex]== false)
        {
            return;
        }


        charObjects[frontLineCenterIndex].transform.position = frontCenterPos;

        SetLeftPosition(charObjects, frontLineCenterIndex, 1, FORMATION_TYPE.ENEMY, charBattleStatusDic[frontLineCenterIndex].sizeType, ref charBattleStatusDic);
        SetRightPosition(charObjects, frontLineCenterIndex, 3, FORMATION_TYPE.ENEMY, charBattleStatusDic[frontLineCenterIndex].sizeType, ref charBattleStatusDic);

        SetLeftPosition(charObjects, 1, 0, FORMATION_TYPE.ENEMY, charBattleStatusDic[1].sizeType, ref charBattleStatusDic);
        SetRightPosition(charObjects, 3, 4, FORMATION_TYPE.ENEMY, charBattleStatusDic[3].sizeType, ref charBattleStatusDic);
    }

    private void SetCharBattlePosition(GameObject[] charObjects)
    {
        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
        {
            if (charObjects[i])
            {
                if (charObjects[i].GetComponent<CharController>())
                {
                    charObjects[i].GetComponent<CharController>().SetFirstPosition();
                }
            }
        }
    }

    #endregion

    public void RestBattle()
    {
        onBattleReset.Invoke();
        Debug.Log("Debugging : Rest Battle");
        for (int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
        {
            Destroy(playerObjects[i]);
            Destroy(enemyObjects[i]);
        }

        for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
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

    public void ExitBattle()
    {
        onBattleExited.Invoke();
        Inst.BattleState = BATTLE_STATE.NONE;
        SceneManager.LoadScene("Lobby");
    }
}