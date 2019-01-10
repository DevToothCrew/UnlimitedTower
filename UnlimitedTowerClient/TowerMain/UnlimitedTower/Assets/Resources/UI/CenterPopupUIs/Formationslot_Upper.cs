using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Formationslot_Upper : MonoBehaviour {

    /// <summary>
    /// 클릭시, (메인히어로가 아니면)현재 배치가되어있는 칸이라면 배치를 해제한다
    /// </summary>

    //
    public Image emptyImage;
    public Image charImage;
    public Image lefttopImage;
    public Image monsterBGimage;
    public Text leftbottomleveltext;
    public Text monsterEnforceText;
    [SerializeField] int FormationIndex;

    // 현재 자리넘버가 어떻게 되는지
    [Header("debugdata")]
    public bool isPlaced;
    public UserServantData curServant;
    public UserMonsterData curMonster;

    // 저장되어있는 FormationIndex에다가 teamnumber만 줬을때, 해당자리에 배치되어있는놈 display
    public void SetDisplayteam(int curTeamNum)
    {
        // 배치된 아이가 있다면 -> 배치표시후 return
        bool isplaced = GameDataManager.instance.isPlacedAt(FormationInfoPopup.instance.curTeamNum, FormationIndex);
        if (isplaced)
        {
            // 몬스터
            if (FormationIndex>=5)
            {
                isPlaced = true;
                curMonster = GameDataManager.instance.getMonsterPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);


                uiinitialize();
                charImage.gameObject.SetActive(true);
                charImage.sprite = ErdManager.instance.getMonsterImage(curMonster.monsterNum, curMonster.monsterTypeNum);
                lefttopImage.gameObject.SetActive(true);
                lefttopImage.sprite = ErdManager.instance.TypeIcons[curMonster.monsterTypeNum];
                leftbottomleveltext.gameObject.SetActive(true);
                leftbottomleveltext.text = "lv." + curMonster.level;
                monsterBGimage.gameObject.SetActive(true);
                monsterBGimage.sprite = ErdManager.instance.monstergradeIcons[curMonster.gradeNum];
                monsterEnforceText.gameObject.SetActive(true);
                monsterEnforceText.text = curMonster.enforceNum > 0 ? "+" + curMonster.enforceNum : "";


                return;
            }
            // 서번트
            else
            {
                isPlaced = true;
                curServant = GameDataManager.instance.getServantPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);

                uiinitialize();
                charImage.gameObject.SetActive(true);
                charImage.sprite = ErdManager.instance.GetServantIconSprite(curServant.isLegend, curServant.charNum, curServant.jobNum);
                lefttopImage.gameObject.SetActive(true);
                lefttopImage.sprite = ErdManager.instance.JobIcons[curServant.jobNum];
                leftbottomleveltext.gameObject.SetActive(true);
                leftbottomleveltext.text = "lv." + curServant.level;



                return;
            }

            
        }
        
        // 없다면 lock상태로
        toDeregister();
    }
    public void toDeregister()
    {
        isPlaced = false;

        uiinitialize();
        emptyImage.gameObject.SetActive(true);

        emptyImage.sprite = FormationInfoPopup.instance.bgsprite;
    }



    private void OnEnable()
    {
        GameDataManager.instance.placeChangedEvent += monsterPlaceChanged;
    }
    private void OnDisable()
    {
        toDeregister();
        GameDataManager.instance.placeChangedEvent -= monsterPlaceChanged;
    }


    // 이벤트

    // 특정몬스터의 배치가 변경되면, 일단 내 칸을 초기화 해준다.
    public void monsterPlaceChanged()
    {
        // 현재 내 자리를 초기화하고, 
        toDeregister();
        // 다시 업데이트 해준다.
        SetDisplayteam(FormationInfoPopup.instance.curTeamNum);
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

    // 클릭시
    public void onclick()
    {
        // 메인히어로가 배치된 자리라면, return
        if (FormationIndex == 2)
        {
            return;
        }


        // 배치되어있다면, 배치 풀기 요청
        if (GameDataManager.instance.isPlacedAt(FormationInfoPopup.instance.curTeamNum, FormationIndex))
        {
            // 몬스터 일경우
            if (FormationIndex >= 5)
            {
                UserMonsterData monsterdata = GameDataManager.instance.getMonsterPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);
                GameDataManager.instance.request_deplace(PlayerType.monster, monsterdata.index);
            }
            // 서번트 일경우
            else
            {
                UserServantData servantdata = GameDataManager.instance.getServantPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);
                GameDataManager.instance.request_deplace(PlayerType.servant, servantdata.index);
            }
        }

        // formaioninfoPopup에 서 TryWaiting
        FormationInfoPopup.instance.TryWaiting(FormationIndex);
    }
}
