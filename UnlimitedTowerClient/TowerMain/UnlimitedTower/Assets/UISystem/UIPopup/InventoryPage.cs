using UnityEngine.UI;
using UnityEngine;

public class InventoryPage : MonoBehaviour {

    public GameObject[] subPages = new GameObject[4];
    private int pageIndex = 10;

    public void OnEnable()
    {
        OnClickPopupPageButton(0);
    }

    public void OnClickPopupPageButton(int num)
    {
        for(int i = 0; i < 4; i++)
        {
            if(i == num)
            {
                subPages[i].SetActive(true);
                LobbyManager.Inst.SetChangeMenu(i + pageIndex);
            }
            else
            {
                subPages[i].SetActive(false);
            }
        }
    }
}

