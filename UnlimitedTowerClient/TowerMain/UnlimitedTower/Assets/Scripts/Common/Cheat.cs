using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class Cheat : MonoSingleton<Cheat>
{
    private System.Random rand = new System.Random();

    public string GetUserLoginData(string user, int gameMoney)
    {
        UserLoginData userLoginData = new UserLoginData();

        userLoginData.token = "10000000";
        userLoginData.eos = "15400000";

        userLoginData.userinfo.user = user;
        userLoginData.userinfo.state = 2;
        userLoginData.userinfo.hero = GetRandomServant(GetRandomServantJob());

        partyData partyData = new partyData();
        partyData.index = 1;
        partyData.state = 0;

        for (int i = 1; i < 5; i++)
        {
            userLoginData.servant_list.Add(GetRandomServantData(i, GetRandomServantJob()));

            if (i < 5)
            {
                partyData.servant_list.Add(i);
            }
        }

        for (int i = 1; i < 6; i++)
        {
            userLoginData.monster_list.Add(GetRandomMonster(i));

            if (i < 6)
            {
                partyData.monster_list.Add(i);
            }
        }

        // Item 추가 예정

        userLoginData.party_info = partyData;

        return JsonMapper.ToJson(userLoginData);
    }

    public SERVANT_JOB GetRandomServantJob()
    {
        int job = rand.Next(1, 6);

        if (job == 1)
        {
            return SERVANT_JOB.Warrior;
        }
        else if (job == 2)
        {
            return SERVANT_JOB.Thief;
        }
        else if (job == 3)
        {
            return SERVANT_JOB.Archer;
        }
        else if (job == 4)
        {
            return SERVANT_JOB.Magician;
        }
        else if (job == 5)
        {
            return SERVANT_JOB.Cleric;
        }
        else
        {
            return SERVANT_JOB.WhiteHand;
        }
    }

    public string GetGachaResultData()
    {
        int type = rand.Next((int)GACHA_RESULT_TYPE.Servant, (int)GACHA_RESULT_TYPE.Item);

        if (type == (int)GACHA_RESULT_TYPE.Servant)
        {
            gachaServantData gachaResult = new gachaServantData();
            gachaResult.result_type = type;
            gachaResult.data = GetRandomServantData(UserDataManager.Inst.servantDic.Count + 1, GetRandomServantJob());

            return JsonMapper.ToJson(gachaResult);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Monster)
        {
            gachaMonsterData gachaResult = new gachaMonsterData();
            gachaResult.result_type = type;
            gachaResult.data = GetRandomMonster(UserDataManager.Inst.monsterDic.Count + 1);

            return JsonMapper.ToJson(gachaResult);
        }
        else if (type == (int)GACHA_RESULT_TYPE.Item)
        {
            // 아이템은 아직
            gachaItemData gachaResult = new gachaItemData();

            return JsonMapper.ToJson(gachaResult);
        }
        else
        {
            return null;
        }
    }

    public string GetBattleActionData(string user, int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {
        stageStateData stageStateInfo = UserDataManager.Inst.GetStageState();

        stageActionInfoData battleactiondata = new stageActionInfoData();
        battleactiondata.user = user;
        battleactiondata.turn += 1;

        for (int i = 0; i < 10; ++i)
        {
            if (BattleSystem.Inst.characterControl[i].nowHp == 0)
            {
                continue;
            }
            actionInfo action = new actionInfo();
            do
            {
                action.target_position = UnityEngine.Random.Range(10, 20);
            } while (BattleSystem.Inst.characterControl[action.target_position].nowHp == 0);
            action.avoid = false;
            action.critical = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
            action.damage = rand.Next(200,500);

            battleActionInfo actioninfo = new battleActionInfo();
            actioninfo.my_position = i;
            actioninfo.action_type = 2;
            actioninfo.battle_action_list.Add(action);
            
            battleactiondata.battle_info_list.Add(actioninfo);
        }

        for (int i = 10; i < 20; ++i)
        {
            if (BattleSystem.Inst.characterControl[i].nowHp == 0)
            {
                continue;
            }
            actionInfo action = new actionInfo();
            do
            {
                action.target_position = UnityEngine.Random.Range(0, 10);
            } while (BattleSystem.Inst.characterControl[action.target_position].nowHp == 0);
            action.avoid = false;
            action.critical = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
            action.damage = rand.Next(200, 500);
            
            battleActionInfo actioninfo = new battleActionInfo();
            actioninfo.my_position = i;
            actioninfo.action_type = 2;
            actioninfo.battle_action_list.Add(action);
            
            battleactiondata.battle_info_list.Add(actioninfo);
        }


        return JsonMapper.ToJson(battleactiondata);
    }

    public string GetStageStartData(string user, int stageNum, int partyNum)
    {

        stageStateData battlestatedata = new stageStateData();
        battlestatedata.user = user;
        battlestatedata.stage_type = 0;
        battlestatedata.enemy_user = user;
        battlestatedata.stage_number = 0;
        battlestatedata.state = 1;


        for (int i = 0; i < 10; ++i)
        {
            stageState newMember = new stageState();
            if (i == 0)
            {
                newMember.position = 0;
                newMember.index = 0;
            }
            else if (i != 0 && i < 5)
            {
                newMember.position = i;
                newMember.index = i;
            }
            else
            {
                newMember.position = i;
                newMember.index = i - 4;
            }

            newMember.id = CharacterCSVData.Inst.monsterDataInspector[UnityEngine.Random.Range(0, CharacterCSVData.Inst.monsterDataInspector.Count)].indexNumber; 
            newMember.now_hp = 10000;
            newMember.physical_attack = 10000;
            newMember.physical_defense = 10;
            newMember.crit_physical_dmg = 1;
            newMember.magic_attack = 10000;
            newMember.magic_defense = 10;
            newMember.crit_magic_dmg = 1;
            newMember.crit_per = 5;
            newMember.avoid = 5;
            newMember.state = 0;
            newMember.speed = 25;

            battlestatedata.my_state_list.Add(newMember);
        }

        for (int i = 0; i < 10; ++i)
        {
            stageState newMember = new stageState();
            newMember.position = i + 10;
            newMember.index = 0;
            newMember.id = CharacterCSVData.Inst.monsterDataInspector[UnityEngine.Random.Range(0, CharacterCSVData.Inst.monsterDataInspector.Count)].indexNumber;
            newMember.now_hp = 10000;
            newMember.physical_attack = 10000;
            newMember.physical_defense = 10;
            newMember.crit_physical_dmg = 1;
            newMember.magic_attack = 10000;
            newMember.magic_defense = 10;
            newMember.crit_magic_dmg = 1;
            newMember.crit_per = 5;
            newMember.avoid = 5;
            newMember.state = 0;
            newMember.speed = 25;

            battlestatedata.enemy_state_list.Add(newMember);
        }

        return JsonMapper.ToJson(battlestatedata);
    }

    public string GetBattleActionData(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {

        return null;
    }

    //public string GetStageStartData(int stageNum, int partyNum)
    //{
    //    StageStateData stageStateData = new StageStateData();
    //    stageStateData.turn = 0;
    //    stageStateData.party_num = partyNum;
    //    stageStateData.stage_num = stageNum;

    //    // 아군 영웅
    //    stageStateInfo heroStateInfo = GetRandomStageStateInfo(0, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.HERO);
    //    stageStateData.my_team_list.Add(heroStateInfo);

    //    // 아군 서번트
    //    for(int i = 1; i< 5; i++)
    //    {
    //        int charType = (int)CHAR_TYPE.SERVANT;
    //        // 영웅 정보
    //        if (i == 2)
    //        {
    //            charType = (int)CHAR_TYPE.HERO;
    //        }

    //        stageStateInfo servantStateInfo = GetRandomStageStateInfo(i, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), charType);
    //        stageStateData.my_team_list.Add(servantStateInfo);
    //    }

    //    // 아군 몬스터
    //    for(int i = 5; i < 10; i++)
    //    {
    //        stageStateInfo monsterStateInfo = GetRandomStageStateInfo(i, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.MONSTER);
    //        stageStateData.my_team_list.Add(monsterStateInfo);
    //    }

    //    // 적군 서번트
    //    for (int i = 10; i < 15; i++)
    //    {
    //        stageStateInfo servantStateInfo = GetRandomStageStateInfo(i, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.SERVANT);
    //        stageStateData.enemy_team_list.Add(servantStateInfo);
    //    }

    //    // 적군 몬스터
    //    for (int i = 15; i < 20; i++)
    //    {
    //        stageStateInfo monsterStateInfo = GetRandomStageStateInfo(i, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.MONSTER);
    //        stageStateData.enemy_team_list.Add(monsterStateInfo);
    //    }

    //    return JsonMapper.ToJson(stageStateData);
    //}

    //public stageStateInfo GetRandomStageStateInfo(int party_index, Status status, int exp, int type)
    //{
    //    stageStateInfo stateInfo = new stageStateInfo();

    //    stateInfo.party_index = party_index;
    //    stateInfo.state = 0;

    //    // DB 나오기 전까지 임시로 사용
    //    stateInfo.status = status;
    //    stateInfo.status_type = rand.Next(0, 3);
    //    stateInfo.exp = exp;
    //    stateInfo.speed = rand.Next(10, 50);

    //    stateInfo.now_hp = Calculator.GetMaxHp(stateInfo.status);
    //    stateInfo.damage = Calculator.GetDamage((STATUS_TYPE)stateInfo.status_type, stateInfo.status);
    //    stateInfo.defence = Calculator.GetDefence(stateInfo.status);
    //    stateInfo.crit_per = Calculator.GetCriticalPer(stateInfo.status);
    //    stateInfo.crit_dmg = 150;
    //    stateInfo.avoid = Calculator.GetAvoid(stateInfo.status);

    //    stateInfo.type = type;
    //    if (type == (int)CHAR_TYPE.HERO)
    //    {
    //        stateInfo.index = 105;
    //    }
    //    else if (type == (int)CHAR_TYPE.SERVANT)
    //    {
    //        // index 넣는 부분
    //        stateInfo.index = rand.Next(105, 109);
    //    }
    //    else if (type == (int)CHAR_TYPE.MONSTER)
    //    {
    //        // index 넣는 부분
    //        stateInfo.index = rand.Next(201, 230);
    //    }

    //    return stateInfo;
    //}
    public string GetStageExit()
    {
        return null;
    }

    public string GetStageResultData()
    {
        stageRewardData rewardData = new stageRewardData();
        rewardData.user = "devtooth";
        rewardData.reward_money = 100000;

        for (int i = 0; i < 10; ++i)
        {
            rewardData.get_exp_list.Add(100);
        }

        return JsonMapper.ToJson(rewardData);
    }

    public string GetTowerStartData()
    {
        return null;
    }

    public string GetTowerResultData()
    {
        return null;
    }

    public servantData GetRandomServantData(int index, SERVANT_JOB job)
    {
        servantData servant = new servantData();
        servant.index = index;
        servant.servant = GetRandomServant(job);

        return servant;
    }

    public servantInfo GetRandomServant(SERVANT_JOB job)
    {
        servantInfo servant = new servantInfo();

        servant.state = 0;
        servant.exp = rand.Next(0, DEFINE.MAX_EXP);
        servant.job = 1; // rand.Next(0, 6);
        servant.stat_point = (Calculator.GetLevelForExp(servant.exp) - 1) * DEFINE.BONUS_STAT;
        servant.appear = GetRandomAppear();
        servant.status = GetRandomStatusInfo();
        servant.equip_slot.Add(0);
        servant.equip_slot.Add(0);
        servant.equip_slot.Add(0);

        return servant;
    }

    public monsterData GetRandomMonster(int index)
    {
        monsterData monsterData = new monsterData();
        monsterData.index = index;

        monsterData.monster = new monsterInfo();
        monsterData.monster.state = 0;
        monsterData.monster.exp = rand.Next(0, DEFINE.MAX_EXP);

        monsterData.monster.type = 0;

        List<int> monsterIndexList = CharacterCSVData.Inst.GetMonsterIndexList();
        if(monsterIndexList == null)
        {
            Debug.LogError("MonsterDataBaseDic Error");
            return null;
        }
        int monsterNum = rand.Next(0, monsterIndexList.Count);
        monsterData.monster.id = monsterIndexList[monsterNum];
        monsterData.monster.grade = rand.Next(1, 6);
        monsterData.monster.upgrade = 0;
        monsterData.monster.status = GetRandomStatusInfo();
        // TODO : 업그레이드에 따른 스테이터스 가중치 추가 필요

        return monsterData;
    }

    public appearInfo GetRandomAppear()
    {
        appearInfo appear = new appearInfo();
        appear.hair = rand.Next((int)APPEAR_HAIR.BASE, (int)APPEAR_HAIR.MAX);
        appear.head = rand.Next((int)APPEAR_HEAD.BASE, (int)APPEAR_HEAD.MAX);
        appear.body = rand.Next((int)APPEAR_BODY.BASE, (int)APPEAR_BODY.MAX);
        appear.gender = rand.Next(0, 2);

        return appear;
    }

    public statusInfo GetRandomStatusInfo()
    {
        statusInfo status = new statusInfo();
        status.basic_str = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
        status.basic_dex = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
        status.basic_int = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);

        status.plus_str = 0;
        status.plus_dex = 0;
        status.plus_int = 0;

        return status;
    }

    public Status GetRandomStatus()
    {
        Status status = new Status();
        status.basicStr = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
        status.basicDex = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);
        status.basicInt = rand.Next(DEFINE.MIN_STATUS, DEFINE.MAX_STATUS);

        status.plusStr = 0;
        status.plusDex = 0;
        status.plusInt = 0;

        return status;
    }

    // Button Cheat


    public void SetLoginCheat()
    {
        if (UserDataManager.Inst.GetUserInfo() == null)
        {
            Debug.Log("Start SetLoginCheat");

            string loginInfo = GetUserLoginData("devtooth", 10000).ToString();
            Debug.Log("[SUCCESS] user login :" + loginInfo);
            PacketManager.Inst.ResponseLogin(loginInfo);
        }
        else
        {
            Debug.Log("Already Get UserInfo");
        }
    }

    public void SetStageStartCheat()
    {
        if (UserDataManager.Inst.GetUserInfo() == null)
        {
            Debug.Log("Invalid UserInfo, Please First SetLoginCheat");
        }
        else
        {
            Debug.Log("Start SetStageStartCheat");
            UTUMSProvider.Instance.RequestStageStart(1, 1);
        }
    }
}