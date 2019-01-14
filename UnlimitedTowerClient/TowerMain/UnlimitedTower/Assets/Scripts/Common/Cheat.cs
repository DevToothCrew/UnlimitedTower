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
            if (i == 1)
            {
                servantdata.servant.isMainServant = true;
                servantdata.servant.onformation = true;
                servantdata.servant.formationindex = 2;
            }
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
        return null;
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
        servant.status = GetRandomStatus();
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
        monsterData.monster.status = GetRandomStatus();
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

    public statusInfo GetRandomStatus()
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
}