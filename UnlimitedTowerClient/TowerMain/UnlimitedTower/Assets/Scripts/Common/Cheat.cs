using System;
using LitJson;

public class Cheat : MonoSingleton<Cheat>
{
    private Random rand = new Random();

    public string GetUserLoginData(string user, int gameMoney)
    {
        UserLoginData userLoginData = new UserLoginData();

        userLoginData.userinfo.user = user;
        userLoginData.userinfo.game_money = gameMoney;
        userLoginData.userinfo.state = 2;
        userLoginData.userinfo.hero = GetRandomServant(GetRandomServantJob());

        for (int i = 1; i <= 5; i++)
        {
            servantData servantdata = GetRandomServantData(i, GetRandomServantJob());
            userLoginData.servant_list.Add(servantdata);

            userLoginData.monster_list.Add(GetRandomMonster(i));
        }

        partyData partyData = new partyData();
        partyData.index = 1;
        partyData.state = 0;
        partyData.party.Add(0);

        userLoginData.party_list.Add(partyData);

        return JsonMapper.ToJson(userLoginData);
    }

    public SERVANT_JOB GetRandomServantJob()
    {
        int job = rand.Next(0, 6);

        if (job == 1)
        {
            return SERVANT_JOB.Warrior;
        }
        else if (job == 2)
        {
            return SERVANT_JOB.Theif;
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

    public string GetBattleActionData(int heroTarget, int heroAction, int monsterTarget, int monsterAction)
    {
        return null;
    }

    public string GetStageStartData(int stageNum, int partyNum)
    {
        StageStateData stateData = new StageStateData();
        stateData.turn = 0;

        // 파티데이터를 넣어서 쓰는건 아직 정리중
        //UserPartyData partyData = UserDataManager.Inst.GetUserPartyInfo(partyNum);
        //if(partyData == null)
        //{
        //    return null;
        //}

        // 아군 영웅
        stageStateInfo heroStateInfo = GetRandomStageStateInfo(0, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.HERO);
        stateData.info_list.Add(heroStateInfo);

        // 아군 서번트
        for(int i = 1; i< 5; i++)
        {
            stageStateInfo servantStateInfo = GetRandomStageStateInfo(i, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.SERVANT);
            stateData.info_list.Add(servantStateInfo);
        }

        // 아군 몬스터
        for(int i = 5; i < 10; i++)
        {
            stageStateInfo monsterStateInfo = GetRandomStageStateInfo(i, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.MONSTER);
            stateData.info_list.Add(monsterStateInfo);
        }

        // 적군 서번트
        for (int i = 10; i < 15; i++)
        {
            stageStateInfo servantStateInfo = GetRandomStageStateInfo(i, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.SERVANT);
            stateData.info_list.Add(servantStateInfo);
        }

        // 적군 몬스터
        for (int i = 15; i < 20; i++)
        {
            stageStateInfo monsterStateInfo = GetRandomStageStateInfo(i, GetRandomStatus(), rand.Next(0, DEFINE.MAX_EXP), (int)CHAR_TYPE.MONSTER);
            stateData.info_list.Add(monsterStateInfo);
        }

        return JsonMapper.ToJson(stateData);
    }

    public stageStateInfo GetRandomStageStateInfo(int party_index, Status status, int exp, int type)
    {
        stageStateInfo stateInfo = new stageStateInfo();

        stateInfo.party_index = party_index;
        stateInfo.state = 0;

        // DB 나오기 전까지 임시로 사용
        stateInfo.status = status;
        stateInfo.status_type = rand.Next(0, 3);
        stateInfo.exp = exp;
        stateInfo.speed = rand.Next(10, 50);

        stateInfo.now_hp = Calculator.GetMaxHp(stateInfo.status);
        stateInfo.damage = Calculator.GetDamage((STATUS_TYPE)stateInfo.status_type, stateInfo.status);
        stateInfo.defence = Calculator.GetDefence(stateInfo.status);
        stateInfo.crit_per = Calculator.GetCriticalPer(stateInfo.status);
        stateInfo.crit_dmg = 150;
        stateInfo.avoid = Calculator.GetAvoid(stateInfo.status);

        stateInfo.type = type;
        if (type == (int)CHAR_TYPE.HERO)
        {
            stateInfo.index = 105;
        }
        else if (type == (int)CHAR_TYPE.SERVANT)
        {
            stateInfo.index = rand.Next(105, 109);
        }
        else if (type == (int)CHAR_TYPE.MONSTER)
        {
            stateInfo.index = rand.Next(201, 230);
        }

        return stateInfo;
    }

    public string GetStageResultData(int stageNum)
    {
        return null;
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
        servant.job = (int)job;
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
        monsterData.monster.look = rand.Next(0, 3);
        monsterData.monster.grade = rand.Next(0, 4);
        monsterData.monster.upgrade = 0;
        monsterData.monster.status = GetRandomStatusInfo();
        // TODO : 업그레이드에 따른 스테이터스 가중치 추가 필요

        return monsterData;
    }

    public appearInfo GetRandomAppear()
    {
        appearInfo appear = new appearInfo();
        appear.hair = rand.Next(0, (int)APPEAR_HAIR.MAX);
        appear.head = rand.Next(0, (int)APPEAR_HEAD.MAX);
        appear.body = rand.Next(0, (int)APPEAR_BODY.MAX);

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
}