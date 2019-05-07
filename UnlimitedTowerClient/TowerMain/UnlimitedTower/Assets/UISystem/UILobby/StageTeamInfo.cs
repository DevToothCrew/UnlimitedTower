using UnityEngine;
using UnityEngine.UI;

public class StageTeamInfo : MonoBehaviour {

    public Image[] GradeImage = new Image[10];
    public Image[] IconImage = new Image[10];

    public GameObject[] objectInfo = new GameObject[10];
    public Image[] imageClass = new Image[10];
    public Image[] imageType = new Image[5];
    public Image[] imageExp = new Image[10];
    public Text[] textLevel = new Text[10];
    public Text[] textExpPer = new Text[10];

    public void OnEnable()
    {
        Sprite xSprite = Resources.Load<Sprite>("UI/Common/exit");

        UserPartyData partyData = UserDataManager.Inst.GetUserPartyInfo();
        if(partyData.partyIndex == 0)
        {
            DebugLog.Log(true, "Invalid User Data");
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
                objectInfo[i].SetActive(false);
                continue;
            }

            if (i <= DEFINE.ServantMaxFormationNum)
            {
                UserServantData servantData = UserDataManager.Inst.GetServantInfo(partyData.formationDataDic[i].index);
                if(servantData == null)
                {
                    DebugLog.Log(true, "Invalid Servant Index : " + partyData.formationDataDic[i].index);
                }

                DBServantData dBServantData = CSVData.Inst.GetServantData(servantData.id);
                if(dBServantData == null)
                {
                    DebugLog.Log(true, "Invalid Servant ID : " + servantData.id);
                }

                GradeImage[i].sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)servantData.grade);
                IconImage[i].sprite = dBServantData.servantIcon;

                imageClass[i].sprite = CSVData.Inst.GetSpriteServantJob(servantData.jobType);
                textLevel[i].text = servantData.level.ToString();

                DBExpData dbExpData = CSVData.Inst.GetExpData(servantData.level);
                if (dbExpData == null)
                {
                    DebugLog.Log(false, "Invalid Level Data");
                }
                else
                {
                    int exExp = 0;
                    if (servantData.level - 1 > 0)
                    {
                        DBExpData exDBExpData = CSVData.Inst.GetExpData(servantData.level - 1);
                        if (exDBExpData == null)
                        {
                            DebugLog.Log(false, "Invalid Level Data");
                        }
                        else
                        {
                            exExp = exDBExpData.charExp;
                        }
                    }
                    float expPer = (exExp - servantData.exp) / (float)(exExp - dbExpData.charExp);
                    textExpPer[i].text = (int)(expPer * 100) + "%";
                    imageExp[i].fillAmount = expPer;
                }
                objectInfo[i].SetActive(true);
            }
            else
            {
                UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(partyData.formationDataDic[i].index);
                if(monsterData == null)
                {
                    DebugLog.Log(true, "Invalid Monster Index : " + partyData.formationDataDic[i].index);

                }

                DBMonsterData dbMonsterData = CSVData.Inst.GetMonsterData(monsterData.id);
                if(dbMonsterData == null)
                {
                    DebugLog.Log(true, "Invalid Monster ID : " + monsterData.id);
                }

                GradeImage[i].sprite = CSVData.Inst.GetSpriteGrade((GRADE_TYPE)monsterData.grade);
                IconImage[i].sprite = dbMonsterData.monsterIcon;

                imageClass[i].sprite = CSVData.Inst.GetSmallSpriteTribeType(monsterData.tribeType);
                imageType[i - 5].sprite = CSVData.Inst.GetSpriteElementType(monsterData.elementType);
                textLevel[i].text = monsterData.level.ToString();

                DBExpData dbExpData = CSVData.Inst.GetExpData(monsterData.level);
                if (dbExpData == null)
                {
                    DebugLog.Log(false, "Invalid Level Data");
                }
                else
                {
                    int exExp = 0;
                    if (monsterData.level - 1 > 0)
                    {
                        DBExpData exDBExpData = CSVData.Inst.GetExpData(monsterData.level - 1);
                        if (exDBExpData == null)
                        {
                            DebugLog.Log(false, "Invalid Level Data");
                        }
                        else
                        {
                            exExp = exDBExpData.charExp;
                        }
                    }
                    float expPer = (exExp - monsterData.exp) / (float)(exExp - dbExpData.charExp);
                    textExpPer[i].text = (int)(expPer * 100) + "%";
                    imageExp[i].fillAmount = expPer;
                }
                objectInfo[i].SetActive(true);
            }
        }


    }
}
