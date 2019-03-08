using UnityEngine.UI;
using UnityEngine;

public class ShopPage : MonoBehaviour {

    public GameObject[] subPages = new GameObject[3];
    private int pageIndex = 20;

    public void OnEnable()
    {
        OnClickPopupPageButton(0);
    }

    public void OnClickPopupPageButton(int num)
    {
        for(int i = 0; i < 3; i++)
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