using UnityEngine;
using UnityEngine.UI;

public class StageTeamInfo : MonoBehaviour {

    public Image[] GradeImage = new Image[10];
    public Image[] IconImage = new Image[10];

    public void OnEnable()
    {
        Sprite xSprite = Resources.Load<Sprite>("UI/Common/exit");

        UserPartyData partyData = UserDataManager.Inst.GetUserPartyInfo();
        if(partyData.partyIndex == 0)
        {
            Debug.LogError("Invalid User Data");
            return;
        }

        for(int i = 0; i < partyData.formationDataDic.Count; i++)
        {
            if(partyData.formationDataDic.ContainsKey(i) == false)
            {
                continue;
            }

            if (partyData.formationDataDic[i].index == 0)
            {
                GradeImage[i].sprite = CSVData.Inst.GetSpriteGrade(GRADE_TYPE.COMMON);
                IconImage[i].sprite = xSprite;
                continue;
            }

            if (i <= DEFINE.ServantMaxFormationNum)
            {
                UserServantData servantData = UserDataManager.Inst.GetServantInfo(partyData.formationDataDic[i].index);
                if(servantData == null)
                {
                    Debug.LogError("Invalid Servant Index : " + partyData.formationDataDic[i].index);
                }


                DBServantData dBServantData = CSVData.Inst.GetServantData(servantData.id);
                if(dBServantData == null)
                {
                    Debug.LogError("Invalid Servant ID : " + servantData.id);
                }

                GradeImage[i].sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)servantData.grade);
                IconImage[i].sprite = dBServantData.servantIcon;
            }
            else
            {
                UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(partyData.formationDataDic[i].index);
                if(monsterData == null)
                {
                    Debug.LogError("Invalid Monster Index : " + partyData.formationDataDic[i].index);

                }

                DBMonsterData dbMonsterData = CSVData.Inst.GetMonsterData(monsterData.id);
                if(dbMonsterData == null)
                {
                    Debug.LogError("Invalid Monster ID : " + monsterData.id);
                }

                GradeImage[i].sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)monsterData.grade);
                IconImage[i].sprite = dbMonsterData.monsterIcon;
            }
        }


    }
}
