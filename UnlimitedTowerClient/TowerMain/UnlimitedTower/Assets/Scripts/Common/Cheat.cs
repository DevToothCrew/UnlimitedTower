using System;
using LitJson;

public class Cheat : MonoSingleton<Cheat>
{
    private Random rand = new Random();

    //
    public string GetUserLoginData(string user, int gameMoney)
    {
        UserLoginData userLoginData = new UserLoginData();

        userLoginData.userinfo.user = user;
        userLoginData.userinfo.game_money = gameMoney;

        userLoginData.userinfo.state = 2;
        userLoginData.userinfo.hero = GetRandomServant(GetRandomServantJob());
        servantData mainherodata = new servantData();
        mainherodata.servant = userLoginData.userinfo.hero;
        mainherodata.index = 0;

        // 서번트 생성 ㅇㅋ
        for (int i = 0; i < 7; i++)
        {
            // 서번트 정보
            servantInfo info = GetRandomServant(GetRandomServantJob());
            
            // 서번트 데이터
            servantData servantdata = new servantData();
            servantdata.index = 1 + i;
            servantdata.servant = info;

            userLoginData.servant_list.Add(servantdata);
        }
        userLoginData.servant_list.Add(mainherodata);   // 메인히어로 추가

        // 몬스터 생성
        for (int i = 0; i < 9; i++)
        {
            MonsterEntity.Param param = ErdManager.instance.MonsterEntityTable.param[UnityEngine.Random.Range(0, ErdManager.instance.MonsterEntityTable.param.Count)];

            monsterData monsterdata = GetRandomMonster(i);
            monsterdata.index = i;

            userLoginData.monster_list.Add(monsterdata);
        }

        // 아이템 생성
        for (int i = 0; i < 9; i++)
        {
            itemData itemdata = new itemData();
            itemdata.index = i;

            itemInfo iteminfo = new itemInfo();
            iteminfo.itemnum = UnityEngine.Random.Range(0, 7);
            iteminfo.tier = UnityEngine.Random.Range(0, 4);
            iteminfo.upgrade = UnityEngine.Random.Range(0, 4);

            itemdata.item = iteminfo;


            userLoginData.item_list.Add(itemdata);
        }

        // 배치 데이터 생성
        int maxteamindex = 3;

        // 파티,포메이션 추가
        for (int partyNum = 0; partyNum < maxteamindex; partyNum++)
        {
            partyData partydata = new partyData();
            partydata.index = partyNum;
            
            for (int forma_index = 0; forma_index < DEFINE.PARTY_MAX_NUM; forma_index++)
            {
                // 메인히어로는 모든 2번자리에 배치되어 있음.
                if (forma_index == 2)
                {
                    partydata.party.Add(0);
                }
                else
                {
                    partydata.party.Add(0);
                }
            }


            userLoginData.party_list.Add(partydata);
        }








        //// 서번트 생성 
        //for (int i = 0; i < 7; i++)
        //{
        //    UserServantData userservantdata = new UserServantData();
        //    userservantdata.index = i;
        //    UserDataManager.Inst.AddServantData(userservantdata);

        //    if (i == 0)
        //    {
        //        userservantdata.isMainHero = true;
        //    }
        //    userservantdata.charNum = UnityEngine.Random.Range(0, 3);
        //    userservantdata.jobNum = UnityEngine.Random.Range(0, 2);
        //    userservantdata.status.basicStr = UnityEngine.Random.Range(5, 10);
        //    userservantdata.status.basicDex = UnityEngine.Random.Range(5, 10);
        //    userservantdata.status.basicInt = UnityEngine.Random.Range(5, 10);
        //}

        //// 몬스터 생성
        //for (int i = 0; i < 9; i++)
        //{
        //    UserMonsterData monsterdata = new UserMonsterData();
        //    monsterdata.index = i;
        //    UserDataManager.Inst.AddMonsterData(monsterdata);

        //    MonsterEntity.Param param = ErdManager.instance.MonsterEntityTable.param[UnityEngine.Random.Range(0, ErdManager.instance.MonsterEntityTable.param.Count)];

        //    monsterdata.monsterNum = param.monsterNum;
        //    monsterdata.monsterTypeNum = param.typeNum;
        //    monsterdata.enforceNum = UnityEngine.Random.Range(0, 5);
        //    monsterdata.gradeNum = UnityEngine.Random.Range(0, 4);
        //}

        //// 아이템 생성
        //for (int i = 0; i < 9; i++)
        //{
        //    UserMountItemData item = new UserMountItemData();
        //    item.index = i;
        //    item.mountitemNum = Random.Range(0, 7);
        //    item.tearNum = Random.Range(0, 4);
        //    item.enforceCount = Random.Range(0, 4);

        //    UserDataManager.Inst.AddMountitemData(item);
        //}

        //// 배치 데이터 생성
        //int maxteamindex = 3;

        //// 포메이션 추가
        //for (int partyNum = 0; partyNum < maxteamindex; partyNum++)
        //{
        //    for (int forma_index = 0; forma_index < 10; forma_index++)
        //    {
        //        UserFormationData userformationdata = new UserFormationData();

        //        if (forma_index == 2)
        //        {
        //            userformationdata.isPlaced = true;
        //            userformationdata.index = 0;
        //        }
        //        userformationdata.partyIndex = partyNum;
        //        userformationdata.formationIndex = forma_index;


        //        UserDataManager.Inst.UserFormationList.Add(userformationdata);


        //    }
        //}

        //// 파티 데이터 생성
        //for (int i = 0; i < maxteamindex; i++)
        //{
        //    // 파티 추가
        //    UserPartyData userpartydata = new UserPartyData();
        //    userpartydata.partyIndex = i;

        //    UserDataManager.Inst.partyDic.Add(userpartydata.partyIndex, userpartydata);
        //    UserDataManager.Inst.partyList.Add(userpartydata);


        //}




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
        
        return servant;
    }

    public monsterData GetRandomMonster(int index)
    {
        monsterData monsterData = new monsterData();
        monsterData.index = index;

        MonsterEntity.Param param = ErdManager.instance.MonsterEntityTable.param[UnityEngine.Random.Range(0, ErdManager.instance.MonsterEntityTable.param.Count)];


        monsterData.monster = new monsterInfo();
        monsterData.monster.state = 0;
        monsterData.monster.exp = rand.Next(0, DEFINE.MAX_EXP);
        monsterData.monster.type = param.typeNum;
        monsterData.monster.look = param.monsterNum;
        monsterData.monster.grade = UnityEngine.Random.Range(0, 4);
        monsterData.monster.upgrade = UnityEngine.Random.Range(0, 5);
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