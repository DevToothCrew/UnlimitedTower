using UnityEngine;
using UnityEngine.UI;

public class ServantObject : MonoBehaviour {

    public GameObject gradeImage;
    public GameObject servantImage;
    public GameObject jobImage;
    public GameObject level;
    public GameObject maxLevel;

    public UserServantData servantData;

    private string resourcePath = "Inventory/";
    private string servantPath = "UI/CharacterImage/";

    public void SetServantData(UserServantData getServantData)
    {
        if(getServantData == null)
        {
            return;
        }

        servantData = getServantData;

        gradeImage.GetComponent<Image>().sprite = GetGradeImage(5);
        gradeImage.SetActive(true);

        servantImage.GetComponent<Image>().sprite = GetServantImage(getServantData.jobNum);
        servantImage.SetActive(true);

        jobImage.GetComponent<Image>().sprite = GetServantJobIcon(getServantData.jobNum);
        jobImage.SetActive(true);

        level.GetComponent<Text>().text = "Lv." + getServantData.level.ToString();
        level.SetActive(true);

        maxLevel.SetActive(false);
    }

    public Sprite GetGradeImage(int grade)
    {
        Sprite sprite = new Sprite();

        switch (grade)
        {
            case 5:
                sprite = Resources.Load<Sprite>(resourcePath + "2_Grade_Common") as Sprite;
                break;
            case 4:
                sprite = Resources.Load<Sprite>(resourcePath + "3_Grade_Uncommon") as Sprite;
                break;
            case 3:
                sprite = Resources.Load<Sprite>(resourcePath + "4_Grade_Rare") as Sprite;
                break;
            case 2:
                sprite = Resources.Load<Sprite>(resourcePath + "5_Grade_Unique") as Sprite;
                break;
            case 1:
                sprite = Resources.Load<Sprite>(resourcePath + "6_Grade_Legend") as Sprite;
                break;

            default:
                break;
        }

        return sprite;
    }

    public Sprite GetServantImage(int job)
    {
        Sprite sprite = new Sprite();

        switch (job)
        {
            case (int)SERVANT_JOB.Warrior:
                sprite = Resources.Load<Sprite>(servantPath + "1_Warrior_1") as Sprite;
                break;

            case (int)SERVANT_JOB.Thief:
                sprite = Resources.Load<Sprite>(servantPath + "3_Thief_1") as Sprite;
                break;

            case (int)SERVANT_JOB.Cleric:
                sprite = Resources.Load<Sprite>(servantPath + "5_Cleric_1") as Sprite;
                break;

            case (int)SERVANT_JOB.Archer:
                sprite = Resources.Load<Sprite>(servantPath + "2_Archer_1") as Sprite;
                break;

            case (int)SERVANT_JOB.Magician:
                sprite = Resources.Load<Sprite>(servantPath + "4_Magician_1") as Sprite;

                break;
        }

        return sprite;
    }

    public Sprite GetServantJobIcon(int job)
    {
        Sprite sprite = new Sprite();

        switch (job)
        {
            case (int)SERVANT_JOB.Warrior:
                sprite = Resources.Load<Sprite>(resourcePath + "13_Job_Warrior") as Sprite;
                break;

            case (int)SERVANT_JOB.Thief:
                sprite = Resources.Load<Sprite>(resourcePath + "15_Job_Thief") as Sprite;
                break;

            case (int)SERVANT_JOB.Cleric:
                sprite = Resources.Load<Sprite>(resourcePath + "16_Job_Cleric") as Sprite;
                break;

            case (int)SERVANT_JOB.Archer:
                sprite = Resources.Load<Sprite>(resourcePath + "14_Job_Archer") as Sprite;
                break;

            case (int)SERVANT_JOB.Magician:
                sprite = Resources.Load<Sprite>(resourcePath + "17_Job_Magician") as Sprite;
                break;
        }

        return sprite;
    }
}