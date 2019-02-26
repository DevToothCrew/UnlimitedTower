using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUIsManager : MonoBehaviour {

    // Singleton
    public static PopupUIsManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void OnDestroy()
    {
        instance = null;
    }

    

    /* PopupUIs */

    public ItemInfoPannel iteminfopannel;
    public GameObject PopupStringObj;
    

    /* CenterPopup 들 */
    //
    public HeroInfoPopup heroInfoPopup;
    public PartnerInfoPopup partnerInfoPopup;
    public FormationInfoPopup formationInfoPopup;
    public StageInfoPopup stageInfoPopup;
    public InventoryInfoPopup inventoryInfoPopup;
   
    //
    public void OnClickHerobtn()
    {
        // 이미켜져있다면, return
        if (heroInfoPopup.gameObject.activeSelf)
        {
            return;
        }

        UserServantData servantdata = UserDataManager.Inst.GetHeroInfo();
        heroInfoPopup.ToRegistered(servantdata);
    }
    

    // 팝업창 띄우기
    public void CreatePopupText(string textString)
    {
        PopupString script = Instantiate(PopupStringObj).GetComponent<PopupString>();
        script.popupString.text = textString;
        script.transform.SetParent(transform);
        script.transform.localPosition = new Vector3(0, 0, 0);
    }
}
