using UnityEngine.UI;
using UnityEngine;

public class BurnResultItemInfo : MonoBehaviour {

    public Image ItemResourceImage;
    public Text ItemCount;
    public Text ItemName;

    public void UpdateItem(UserItemData getItemData)
    {
        DBItemData itemData = CSVData.Inst.GetItemData(getItemData.id);
        if(itemData == null)
        {
            Debug.Log("Invalid Item ID : " + getItemData.id);
            return;
        }

        ItemResourceImage.sprite = itemData.ItemIcon;
        ItemName.text = itemData.name;
        ItemCount.text = "X" + getItemData.itemInfoList[0].count;
    }
}
