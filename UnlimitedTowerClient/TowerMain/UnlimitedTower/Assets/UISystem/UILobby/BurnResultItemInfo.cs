using UnityEngine.UI;
using UnityEngine;

public class BurnResultItemInfo : MonoBehaviour {

    public Image ItemResourceImage;
    public Text ItemCount;
    public Text ItemName;

    public void UpdateItem(int itemID, int itemCount)
    {
        DBItemData itemData = CSVData.Inst.GetItemData(itemID);
        if(itemData == null)
        {
            Debug.Log("Invalid Item ID : " + itemID);
            return;
        }

        ItemResourceImage.sprite = itemData.ItemIcon;
        ItemName.text = itemData.name;
        ItemCount.text = "X" + itemCount;
    }
}
