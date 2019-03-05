using UnityEngine;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour {

    public GameObject gradeImage;
    public GameObject itemImage;
    public GameObject tierText;
    public GameObject upgradeText;

    public UserMountItemData itemData;

    private string resourcePath = "Inventory/";
    private string monsterPath = "UI/CharacterImage/";

    public void SetMountItemData(UserMountItemData getMountItemData)
    {
        if(getMountItemData == null)
        {
            return;
        }

        itemData = getMountItemData;

        gradeImage.GetComponent<Image>().sprite = GetGradeImage(itemData.gradeNum);
        gradeImage.SetActive(true);

        itemImage.GetComponent<Image>().sprite = GetMountItemImage(itemData.mountitemNum);
        itemImage.SetActive(true);

        tierText.GetComponent<Text>().text = itemData.tierNum.ToString() + "T";
        tierText.SetActive(true);

        if(itemData.upgradeCount > 0)
        {
            upgradeText.GetComponent<Text>().text = "+" + itemData.upgradeCount.ToString();
            upgradeText.SetActive(true);
        }
        else
        {
            upgradeText.SetActive(false);
        }
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

    public Sprite GetMountItemImage(int index)
    {
        Sprite sprite = new Sprite();

        string resource = CharacterCSVData.Inst.GetMonsterDBResource(index);
        if (resource != null)
        {
            sprite = Resources.Load<Sprite>(monsterPath + resource) as Sprite;
        }

        return sprite;
    }

    #endregion
}
