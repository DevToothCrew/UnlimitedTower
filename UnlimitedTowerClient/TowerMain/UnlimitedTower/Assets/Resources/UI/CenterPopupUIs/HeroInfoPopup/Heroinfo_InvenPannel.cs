using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heroinfo_InvenPannel : MonoBehaviour {

    [SerializeField] GameObject itemprefab;

    public Transform itemParent;
    public List<HeroInfoInventoryItemicon> iconlist;
    public List<HeroInfoInventoryItemicon> activeIconList;

    public HeroInfoInventoryItemicon geticon()
    {
        if (iconlist.Count == 0)
        {
            createicon();
        }
        HeroInfoInventoryItemicon icon = popicon();
        activeIconList.Add(icon);
        icon.gameObject.SetActive(true);
        return icon;
    }
    public void pushicon(HeroInfoInventoryItemicon icon)
    {
        icon.gameObject.SetActive(false);
        iconlist.Add(icon);

        if (activeIconList.Contains(icon))
        {
            activeIconList.Remove(icon);
        }
    }

    // object pooling 함수
    public void createicon()
    {
        HeroInfoInventoryItemicon icon = Instantiate(itemprefab).GetComponent<HeroInfoInventoryItemicon>();
        pushicon(icon);
    }
    HeroInfoInventoryItemicon popicon()
    {
        HeroInfoInventoryItemicon icon = iconlist[0];
        iconlist.Remove(icon);

        return icon;
    }


    private void OnEnable()
    {
        for (int i = 0; i < UserDataManager.Inst.MountItemList.Count; i++)
        {
            HeroInfoInventoryItemicon icon = geticon();
            icon.register(UserDataManager.Inst.MountItemList[i]);

            icon.transform.SetParent(itemParent);
            icon.transform.SetAsLastSibling();
        }
    }
    private void OnDisable()
    {
        while (activeIconList.Count > 0)
        {
            // 엑티브에서 빼주기
            HeroInfoInventoryItemicon icon = activeIconList[0];
            activeIconList.Remove(icon);

            // 푸쉬하기
            pushicon(icon);
        }
    }
}
