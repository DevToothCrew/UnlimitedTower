using UnityEngine;
using UnityEngine.UI;

public class MonsterObject : MonoBehaviour {

    public GameObject gradeImage;
    public GameObject monsterImage;
    public GameObject typeImage;
    public GameObject upgradeText;
    public GameObject level;
    public GameObject maxLevel;

    public UserMonsterData monsterData;

    private string resourcePath = "Inventory/";
    private string monsterPath = "UI/CharacterImage/";

    public void SetMonsterData(UserMonsterData getMonsterData)
    {
        if (getMonsterData == null)
        {
            return;
        }

        monsterData = getMonsterData;

        gradeImage.GetComponent<Image>().sprite = GetGradeImage(monsterData.gradeNum);
        gradeImage.SetActive(true);

        typeImage.SetActive(false);

        monsterImage.GetComponent<Image>().sprite = GetMonsterImage(getMonsterData.index);
        monsterImage.SetActive(true);

        level.GetComponent<Text>().text = "Lv." + getMonsterData.level.ToString();
        level.SetActive(true);

        if(getMonsterData.upgradeCount > 0)
        {
            upgradeText.GetComponent<Text>().text = "+" + getMonsterData.upgradeCount.ToString();
            upgradeText.SetActive(true);
        }
        else
        {
            upgradeText.SetActive(false);
        }

        maxLevel.SetActive(false);
    }

    #region 추후 CSV 연결로 변경

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

    public Sprite GetMonsterImage(int index)
    {
        Sprite sprite = new Sprite();

        string resource = CharacterCSVData.Inst.GetMonsterDBResource(index);
        if(resource != null)
        {
            sprite = Resources.Load<Sprite>(monsterPath + resource) as Sprite;
        }

        return sprite;
    }

    #endregion
}
