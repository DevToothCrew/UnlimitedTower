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
        // 이미 등록되어있다면 return
        if (PopupUIsManager.instance.heroInfoPopup.isRegistered)
        {
            return;
        }

        // UI들 초기화
        LobbyManager.Inst.InitCenterPopup();
        // CenterPopup 켜주기
        LobbyManager.Inst.CenterPopup.SetActivateWithAnimation(true);

        // 히어로팝업 켜기
        UserServantData servantdata = UserDataManager.Inst.ServantList.Find((rowdata) => { return rowdata.isMainHero; });
        heroInfoPopup.ToRegistered(servantdata);
    }
}
