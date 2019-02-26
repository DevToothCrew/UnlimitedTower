using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heroinfo_InvenPannel : MonoBehaviour {

    [SerializeField] GameObject itemprefab;

    public Transform itemParent;
    public List<HeroInfoInventoryItemicon> iconlist;
    public List<HeroInfoInventoryItemicon> activeIconList;

    public HeroInfoInventoryItemicon GetIcon()
    {
        if (iconlist.Count == 0)
        {
            CreateIcon();
        }
        HeroInfoInventoryItemicon icon = PopIcon();
        activeIconList.Add(icon);
        icon.gameObject.SetActive(true);
        return icon;
    }
    public void PushIcon(HeroInfoInventoryItemicon icon)
    {
        icon.gameObject.SetActive(false);
        iconlist.Add(icon);

        if (activeIconList.Contains(icon))
        {
            activeIconList.Remove(icon);
        }
    }

    // object pooling 함수
    public void CreateIcon()
    {
        HeroInfoInventoryItemicon icon = Instantiate(itemprefab).GetComponent<HeroInfoInventoryItemicon>();
        PushIcon(icon);
    }
    HeroInfoInventoryItemicon PopIcon()
    {
        HeroInfoInventoryItemicon icon = iconlist[0];
        iconlist.Remove(icon);

        return icon;
    }
    
    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        while (activeIconList.Count > 0)
        {
            // 엑티브에서 빼주기
            HeroInfoInventoryItemicon icon = activeIconList[0];
            activeIconList.Remove(icon);

            // 푸쉬하기
            PushIcon(icon);
        }
    }
}
