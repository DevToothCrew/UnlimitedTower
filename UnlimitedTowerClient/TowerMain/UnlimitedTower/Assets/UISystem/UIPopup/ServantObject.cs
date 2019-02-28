using UnityEngine;
using UnityEngine.UI;

public class ServantObject : MonoBehaviour {

    public GameObject gradeImage;
    public GameObject servantImage;
    public GameObject jobImage;
    public GameObject level;
    public GameObject maxLevel;

    public UserServantData servantData;

    private string resourcePath = "UISystem/UIPopup/Resources/Images/";
    private string servantPath = "Resources/UI/CharacterImage/";

    public void Awake()
    {
        servantData = new UserServantData();
        SetServantData(servantData);
    }

    public void SetServantData(UserServantData servantData)
    {
        gradeImage.GetComponent<Image>().sprite = GetGradeResources(5);
        gradeImage.SetActive(true);

        servantImage.GetComponent<Image>().sprite = GetServantImage(servantData.jobNum);
        servantImage.SetActive(true);

        jobImage.SetActive(false);
        level.SetActive(false);
        maxLevel.SetActive(false);
    }

    public Sprite GetGradeResources(int grade)
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

            case (int)SERVANT_JOB.Theif:
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

            default:
                sprite = Resources.Load<Sprite>(resourcePath + "1_EmptyInventory") as Sprite;
                break;
        }

        return sprite;
    }
}