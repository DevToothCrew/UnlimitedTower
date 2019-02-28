using System.Collections;
using UnityEngine;


/// <summary>
/// Debug를 위해 사용되는 로컬 UMS 제공자입니다.
/// </summary>
public class UTLocalUMSProvider : UTUMSProvider
{
    IEnumerator WaitAndLogin()
    {
        yield return new WaitForSeconds(2.0f);
        UTEventPoolInterface.SendEventData("login", new UTPlayerManager.UTPlayerData() { user = "devtooth", gameMoney = 999999 });
    }

    public override void RequestLoginWithScatter()
    {
        UTLoadingManager.Description desc = new UTLoadingManager.Description
        {
            startComment = "Try to login ...",
            finishedComment = "Success!",
            predicate = () => UserDataManager.Inst.userInfo != default(UserInfo),
        };

        UTLoadingManager.Instance.BeginScene(desc);
        UTLoadingManager.Instance.StartCoroutine(WaitAndLogin());
    }

    public override void RequestLogout()
    {
        PacketManager.Inst.ResponseLogout();
    }

    public override void RequestGacha()
    {
        UTEventPoolInterface.SendEventData("gacha", new UTPlayerManager.UTGachaData() { });
    }

    public override void RequestSaveParty(UserPartyData partyInfo)
    {
        UTEventPoolInterface.SendEventData("saveparty", new UTPlayerManager.UTPartyData() { partyNum = 1});
    }

    public override void RequestBattleAction(int getHeroTarget, int getHeroAction, int getMonsterTarget, int getMonsterAction)
    {
        UTEventPoolInterface.SendEventData("battleaction", new UTPlayerManager.UTBattleActionData() { heroTarget = getHeroTarget, heroAction = getHeroAction, monsterTarget = getMonsterTarget, monsterAction = getMonsterAction });
    }

    public override void RequestStageStart(int getStageNum, int getPartyNum)
    {
        UTEventPoolInterface.SendEventData("stagestart", new UTPlayerManager.UTStageStartData() { stageNum = getStageNum, partyNum = getPartyNum });
    }

    public override void RequestStageResult(int stageNum)
    {
        //PacketManager.Inst.RequestStageResult(stageNum);
        PacketManager.Inst.RequestStageResult();
    }

    public override void RequestTowerStart()
    {
        throw new System.NotImplementedException();
    }

    public override void RequestTowerResult()
    {
        throw new System.NotImplementedException();
    }
}
