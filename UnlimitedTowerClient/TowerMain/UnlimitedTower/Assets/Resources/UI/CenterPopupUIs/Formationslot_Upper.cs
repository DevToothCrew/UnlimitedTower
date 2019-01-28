using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Formationslot_Upper : MonoBehaviour
{
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
    public GameObject possibleCheckObj;
    
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
        Debug.Log(FormationInfoPopup.instance.curTeamNum + "/" + FormationIndex);
        if (isplaced)
        {
            // 몬스터
            if (FormationIndex >= 5)
            {
                isPlaced = true;
                curMonster = GameDataManager.instance.getMonsterPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);


                InitializeUI();
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

                InitializeUI();
                charImage.gameObject.SetActive(true);
                charImage.sprite = ErdManager.instance.GetServantIconSprite(curServant.isLegend, curServant.body, curServant.jobNum);
                lefttopImage.gameObject.SetActive(true);
                lefttopImage.sprite = ErdManager.instance.JobIcons[curServant.jobNum];
                leftbottomleveltext.gameObject.SetActive(true);
                leftbottomleveltext.text = "lv." + curServant.level;



                return;
            }

        }

        // 없다면 lock상태로
        ToDeregister();
    }
    public void ToDeregister()
    {
        isPlaced = false;

        InitializeUI();
        emptyImage.gameObject.SetActive(true);

        emptyImage.sprite = FormationInfoPopup.instance.bgsprite;
    }

    private void OnEnable()
    {
        GameDataManager.instance.placeChangedEvent += monsterPlaceChanged;
        FormationInfoPopup.placeModeChanged += placeModeChanged;
        placeModeChanged();
    }
    
    private void OnDisable()
    {
        ToDeregister();
        GameDataManager.instance.placeChangedEvent -= monsterPlaceChanged;
        FormationInfoPopup.placeModeChanged -= placeModeChanged;
    }


    // 이벤트

    // 특정몬스터의 배치가 변경되면, 일단 내 칸을 초기화 해준다.
    public void monsterPlaceChanged()
    {
        // 다시 업데이트 해준다.
        SetDisplayteam(FormationInfoPopup.instance.curTeamNum);
    }
    public void placeModeChanged()
    {
        
        switch (PopupUIsManager.instance.formationInfoPopup.placeMode)
        {
            case FormationInfoPopup.PlaceMode.JUST_DISPLAY:
                {
                    possibleCheckObj.SetActive(false);
                }
                break;

            case FormationInfoPopup.PlaceMode.SERVANT_PLACE:
                {
                    // 몬스터자리면 return
                    if (FormationIndex <= DEFINE.MonsterMaxFormationNum && FormationIndex >= DEFINE.MonsterMinFormationNum)
                    {
                        possibleCheckObj.SetActive(false);
                        return;
                    }


                    // 서번트가 배치되어있는경우
                    if (isPlaced )
                    {
                        possibleCheckObj.SetActive(false);

                    }
                    // 아닌경우
                    else
                    {

                        possibleCheckObj.SetActive(true);
                    }
                }
                break;

            case FormationInfoPopup.PlaceMode.MONSTER_PLACE:
                {
                    // 서번트자리일경우 return
                    if (FormationIndex >= DEFINE.ServantMinFormationNum && FormationIndex <= DEFINE.ServantMaxFormationNum)
                    {
                        possibleCheckObj.SetActive(false);
                        return;
                    }

                    // 몬스터가 배치되어있는경우
                    if (isPlaced )
                    {
                        possibleCheckObj.SetActive(false);
                        return;
                    }

                    // 앞에 서번트가 배치안되어있는경우
                    if (!GameDataManager.instance.isPlacedAt(FormationInfoPopup.instance.curTeamNum, FormationIndex-5))
                    {
                        return;
                    }
                    
                    
                    // 클릭활성화 표시 하기
                    possibleCheckObj.SetActive(true);

                }
                break;
        }
    }

    public void InitializeUI()
    {
        emptyImage.gameObject.SetActive(false);
        charImage.gameObject.SetActive(false);
        lefttopImage.gameObject.SetActive(false);
        monsterBGimage.gameObject.SetActive(false);
        leftbottomleveltext.gameObject.SetActive(false);
        monsterEnforceText.gameObject.SetActive(false);
    }

    // 클릭시
    public void OnClick()
    {
        // 메인히어로가 배치된 자리라면, return
        if (FormationIndex == 2)
        {
            return;
        }


        switch (FormationInfoPopup.instance.placeMode)
        {
            case FormationInfoPopup.PlaceMode.JUST_DISPLAY:
                // 
                // 배치되어있다면, 배치 풀기 요청
                if (GameDataManager.instance.isPlacedAt(FormationInfoPopup.instance.curTeamNum, FormationIndex))
                {
                    Debug.Log("배치되어있엇음");
                    // 몬스터 일경우
                    if (FormationIndex >= DEFINE.MonsterMinFormationNum)
                    {
                        UserMonsterData monsterdata = GameDataManager.instance.getMonsterPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);
                        GameDataManager.instance.request_deplace(UNIT_TYPE.MONSTER, monsterdata.index);
                    }
                    // 서번트 일경우
                    else
                    {
                        UserServantData servantdata = GameDataManager.instance.getServantPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);
                        GameDataManager.instance.request_deplace(UNIT_TYPE.SERVANT, servantdata.index);

                        // 앞에 몬스터가 배치되어있었다면
                        int monsterTeamNum = FormationInfoPopup.instance.curTeamNum;
                        int monsterFormNum = FormationIndex + 5;
                        if (GameDataManager.instance.isPlacedAt(monsterTeamNum, monsterFormNum))
                        {
                            GameDataManager.instance.request_deplace(UNIT_TYPE.MONSTER, GameDataManager.instance.getMonsterPlacedAt_nullPossible(monsterTeamNum, monsterFormNum).index) ;
                        }
                    }
                }
                else
                {
                    Debug.Log("배치 안되어있음");
                }

                break;
            case FormationInfoPopup.PlaceMode.SERVANT_PLACE:

                // 몬스터자리라면 return
                if (FormationIndex >= DEFINE.MonsterMinFormationNum && FormationIndex <= DEFINE.MonsterMaxFormationNum)
                {
                    PopupUIsManager.instance.CreatePopupText("불가능합니다.");
                    return;
                }

                // 배치되어있다면 return
                if (GameDataManager.instance.isPlacedAt(FormationInfoPopup.instance.curTeamNum, FormationIndex))
                {
                    PopupUIsManager.instance.CreatePopupText("불가능합니다.");
                    return;
                }


                // 배치 요청하기
                GameDataManager.instance.request_Placement(UNIT_TYPE.SERVANT, FormationInfoPopup.instance.registeredServantData.index, FormationInfoPopup.instance.curTeamNum, FormationIndex);
                // 모드 돌려놓기
                FormationInfoPopup.instance.SetPlaceMode(FormationInfoPopup.PlaceMode.JUST_DISPLAY);

                break;
            case FormationInfoPopup.PlaceMode.MONSTER_PLACE:

                // 서번트자리라면 return
                if (FormationIndex >= DEFINE.ServantMinFormationNum && FormationIndex <= DEFINE.ServantMaxFormationNum)
                {
                    PopupUIsManager.instance.CreatePopupText("불가능합니다.");
                    return;
                }

                // 배치되어있다면 return
                if (GameDataManager.instance.isPlacedAt(FormationInfoPopup.instance.curTeamNum, FormationIndex))
                {
                    PopupUIsManager.instance.CreatePopupText("불가능합니다.");
                    return;
                }

                // 해당하는 서번트자리가 비어있다면 return
                if (!GameDataManager.instance.isPlacedAt(FormationManager.Inst.targetPartyNum, FormationIndex-5))
                {
                    PopupUIsManager.instance.CreatePopupText("불가능합니다.");
                    return;
                }



                // 배치 요청하기
                GameDataManager.instance.request_Placement(UNIT_TYPE.MONSTER, FormationInfoPopup.instance.registeredMonsterData.index, FormationInfoPopup.instance.curTeamNum, FormationIndex);
                // 모드 돌려놓기
                FormationInfoPopup.instance.SetPlaceMode(FormationInfoPopup.PlaceMode.JUST_DISPLAY);

                break;
        }

        

        // formaioninfoPopup에 서 TryWaiting
        FormationInfoPopup.instance.TryWaiting(FormationIndex);
    }
}
