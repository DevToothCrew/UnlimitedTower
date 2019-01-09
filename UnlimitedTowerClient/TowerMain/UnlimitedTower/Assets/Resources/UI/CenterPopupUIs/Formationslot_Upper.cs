using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Formationslot_Upper : MonoBehaviour {

    /// <summary>
    /// 클릭시, (메인히어로가 아니면)현재 배치가되어있는 칸이라면 배치를 해제한다
    /// </summary>

    //
    public Image image;
    [SerializeField] int FormationIndex;

    // 현재 자리넘버가 어떻게 되는지
    [Header("debugdata")]
    public bool isPlaced;
    public UserServantData curServant;
    public UserMonsterData curMonster;

    // 저장되어있는 FormationIndex에다가 teamnumber만 줬을때, 해당자리에 배치되어있는놈 display
    public void SetDisplayteam(int curTeamNum)
    {
        // 배치된 서번트가 있다면 -> 배치표시후 return
        List<UserServantData> list = new List<UserServantData>(UserDataManager.Inst.servantDic.Values);
        UserServantData rightServant = list.Find((rowdata) => { return rowdata.onFormation == true && rowdata.partyIndex == curTeamNum && rowdata.formationIndex == FormationIndex; });
        if (rightServant != null )
        {
            isPlaced = true;
            curServant = rightServant;
            image.sprite = ErdManager.instance.ServantbodySprite[curServant.charNum];
            return;
        }

        // 배치된 몬스터가 있다면 -> 배치표시후 return
        List<UserMonsterData> mlist = new List<UserMonsterData>(UserDataManager.Inst.monsterDic.Values);
        UserMonsterData rightMonster = mlist.Find((rowdata) => { return rowdata.OnFormation == true && rowdata.partyIndex == curTeamNum && rowdata.formationIndex == FormationIndex; });
        if (rightMonster != null)
        {
            isPlaced = true;
            curMonster = rightMonster;
            image.sprite = ErdManager.instance.MonsterSprite[curMonster.monsterNum];
            return;
        }

        toDeregister();
    }
    public void toDeregister()
    {
        isPlaced = false;
        image.sprite = FormationInfoPopup.instance.bgsprite;
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



    // 클릭시
    public void onclick()
    {
        // 배치되어있다면, 배치 풀기 요청
        if (GameDataManager.instance.isPlacedAt(FormationInfoPopup.instance.curTeamNum, FormationIndex))
        {
            // 몬스터 일경우
            if (FormationIndex >= 5)
            {
                UserMonsterData monsterdata = GameDataManager.instance.getMonsterPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);
                monsterdata.request_deplace();
            }
            // 서번트 일경우
            else
            {
                UserServantData servantdata = GameDataManager.instance.getServantPlacedAt_nullPossible(FormationInfoPopup.instance.curTeamNum, FormationIndex);
                servantdata.request_deplace();
            }
        }

        // formaioninfoPopup에 서 TryWaiting
        FormationInfoPopup.instance.TryWaiting(FormationIndex);
    }
}
