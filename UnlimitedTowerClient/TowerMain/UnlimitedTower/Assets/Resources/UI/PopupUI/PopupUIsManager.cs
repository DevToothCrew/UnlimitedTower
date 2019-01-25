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
        

        LobbyManager.Inst.InitCenterPopup();
        LobbyManager.Inst.CenterPopup.SetActivateWithAnimation(true);
        
        UserServantData servantdata = UserDataManager.Inst.ServantList.Find((rowdata) => { return rowdata.isMainHero; });
        heroInfoPopup.ToRegistered(servantdata);
    }
}
