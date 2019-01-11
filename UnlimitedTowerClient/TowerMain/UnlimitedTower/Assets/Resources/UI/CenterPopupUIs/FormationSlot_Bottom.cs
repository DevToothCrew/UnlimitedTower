using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationSlot_Bottom : MonoBehaviour {

    /// <summary>
    /// 클릭시, 현재 formationinfopopup의 대기칸이 있다면 거기로 배치된다.
    /// 
    /// Placed변수변화에 콜백함수를 걸어놓아야한다.
    /// </summary>

    //
    public Image emptyImage;
    public Image charImage;
    public Image lefttopImage;
    public Image monsterBGimage;
    public Text leftbottomleveltext;
    public Text monsterEnforceText;
    //
    public enum SlotType
    {
        none,
        servant,
        monster
    }
    [Header("debugdata")]
    public SlotType slottype;

    public UserServantData servantdata;
    public UserMonsterData monsterdata;
    
    public void to_servant(UserServantData servantdata)
    {
        slottype = SlotType.servant;
        this.servantdata = servantdata;


        uiinitialize();
        charImage.gameObject.SetActive(true);
        charImage.sprite = ErdManager.instance.GetServantIconSprite(servantdata.isLegend ,servantdata.charNum, servantdata.jobNum);
        lefttopImage.gameObject.SetActive(true);
        lefttopImage.sprite = ErdManager.instance.JobIcons[servantdata.jobNum];
        leftbottomleveltext.gameObject.SetActive(true);
        leftbottomleveltext.text = "lv." + servantdata.level;


        placedUpdate();
    }
    public void to_monster(UserMonsterData monsterdata)
    {
        slottype = SlotType.monster;
        this.monsterdata = monsterdata;


        uiinitialize();
        charImage.gameObject.SetActive(true);
        charImage.sprite = ErdManager.instance.getMonsterImage(monsterdata.monsterNum, monsterdata.monsterTypeNum);
        lefttopImage.gameObject.SetActive(true);
        lefttopImage.sprite = ErdManager.instance.TypeIcons[monsterdata.monsterTypeNum];
        leftbottomleveltext.gameObject.SetActive(true);
        leftbottomleveltext.text = "lv." + monsterdata.level;
        monsterBGimage.gameObject.SetActive(true);
        monsterBGimage.sprite = ErdManager.instance.monstergradeIcons[monsterdata.gradeNum];
        monsterEnforceText.gameObject.SetActive(true);
        monsterEnforceText.text = monsterdata.enforceNum > 0 ? "+" + monsterdata.enforceNum : "";


        placedUpdate();
    }
    public void to_none()
    {
        slottype = SlotType.none;
        uiinitialize();
        emptyImage.gameObject.SetActive(true);
    }

    // 버튼 온클릭
    public void onclick()
    {
        // 이미 배치된 녀석이면, return
        if (slottype == SlotType.monster && GameDataManager.instance.isPlaced(PlayerType.monster, monsterdata.index))
        {
            return;
        }
        if (slottype == SlotType.servant && GameDataManager.instance.isPlaced(PlayerType.servant, servantdata.index))
        {
            return;
        }

        // 아얘등록안된 칸이면, return
         if (slottype == SlotType.none)
        {
            return;
        }




        int curTeamNum = FormationInfoPopup.instance.curTeamNum;

        // father에 웨이팅칸이 없다면 -> 가장 앞에서부터 채워나가기
        if (!FormationInfoPopup.instance.isWaiting)
        {
            // 서번트이고, 0~4중남은칸이 있다면
            if (slottype == SlotType.servant && GameDataManager.instance.isServantPlaceExist(curTeamNum))
            {
                // 거기로 배치 요청하기
                GameDataManager.instance.request_Placement(PlayerType.servant, servantdata.index, curTeamNum, GameDataManager.instance.GetServantPlaceExist(curTeamNum));
            }
            // 몬스터이고, 5~9중 남은칸이 있다면
            else if (slottype == SlotType.monster && GameDataManager.instance.isMonsterPlaceExist(curTeamNum))
            {
                // 거기로 배치 요청하기
                GameDataManager.instance.request_Placement(PlayerType.monster, monsterdata.index, curTeamNum, GameDataManager.instance.GetMonsterPlaceExist(curTeamNum));
            }
            // 남은 칸이 없다면
            else
            {
                // 암것도 못함.
            }
        }
        // father에 웨이팅칸이 있다면 -> 거기로 배치
        else
        {

            // 해당 칸에 배치 하기
        }

    }

    // 배치중이라면 조금 흐리게, 아니라면 밝게
    public void placedUpdate()
    {
        // 배치중
        if ((slottype == SlotType.servant && GameDataManager.instance.isPlaced(PlayerType.servant,servantdata.index)) ||
            slottype == SlotType.monster && GameDataManager.instance.isPlaced(PlayerType.monster, monsterdata.index))
        {
            charImage.color = new Color(1f, 1f, 1f, 0.4f);
        }
        // 배치중아님
        else
        {
            charImage.color = new Color(1f, 1f, 1f, 1f);
        }
        
    }



    public void uiinitialize()
    {
        emptyImage.gameObject.SetActive(false);
        charImage.gameObject.SetActive(false);
        lefttopImage.gameObject.SetActive(false);
        monsterBGimage.gameObject.SetActive(false);
        leftbottomleveltext.gameObject.SetActive(false);
        monsterEnforceText.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        GameDataManager.instance.placeChangedEvent += placedUpdate;
    }
    private void OnDisable()
    {
        to_none();
        placedUpdate();

        GameDataManager.instance.placeChangedEvent -= placedUpdate;
    }
}
