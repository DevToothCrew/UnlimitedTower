using UnityEngine.UI;
using UnityEngine;

public class HeroPage : MonoBehaviour {

    public Text userInfoText;

    private void OnEnable()
    {
        UserInfo userInfo = UserDataManager.Inst.GetUserInfo();
        if (userInfo.userName != null)
        {
            string userAccountInfo = "UserName : " + userInfo.userName + "\n" + "UserMoney : " + userInfo.userMoney + "\n" + "UserEOS : " + userInfo.userEOS + "\n";
            string userHeroInfo = "UserHero - Name : " + userInfo.userHero.name + "\n" + "Index : " + userInfo.userHero.index + "\n" 
                + "PartyIndex : " + userInfo.userHero.partyIndex + "\n" + "PartyNum : " + userInfo.userHero.partyNum + "\n"
                + "Formation : " + userInfo.userHero.formationNum + "\n" + "Level : " + userInfo.userHero.level + "\n"
                + "Status - basic STR : " + userInfo.userHero.status.basicStr + "\n" + "plus STR : " + userInfo.userHero.status.plusStr + "\n"
                + "basic DEX : " + userInfo.userHero.status.basicDex + "\n" + "plus DEX : " + userInfo.userHero.status.plusDex + "\n"
                + "basic INT : " + userInfo.userHero.status.basicInt + "\n" + "plus INT : " + userInfo.userHero.status.plusInt + "\n";
            userInfoText.text = userAccountInfo + userHeroInfo;
        }
    }
}
