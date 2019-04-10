using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelViewManager : MonoSingleton<ModelViewManager>
{
    //Load Main Unit Model 
    public CharacterCustom characterCustom;
    public Camera modelCamera;
    public GameObject objServantModel = null;
    private GameObject objMonsterModel = null;


    //메인 캐릭터 모델 표시
    public void ChangeMainCharacterModel()
    {
        if (objMonsterModel != null)
        {
            Destroy(objMonsterModel);
        }

        if (UserDataManager.Inst.GetMainCharInfo().mainCharType == CHAR_TYPE.SERVANT)
        {
            objServantModel.SetActive(false);
            int servant_id = UserDataManager.Inst.GetMainCharInfo().mainCharID;
            characterCustom.jobIndex = CSVData.Inst.GetServantData(servant_id).job;
            characterCustom.headIndex = CSVData.Inst.GetServantData(servant_id).head;
            characterCustom.hairIndex = CSVData.Inst.GetServantData(servant_id).hair;
            characterCustom.isMan = CSVData.Inst.GetServantData(servant_id).gender;
            characterCustom.isChildren = CSVData.Inst.GetServantData(servant_id).body;
            characterCustom.Refresh();
            objServantModel.SetActive(true);
        }
        else if (UserDataManager.Inst.GetMainCharInfo().mainCharType == CHAR_TYPE.MONSTER)
        {
            objServantModel.SetActive(false);
            objMonsterModel = Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(UserDataManager.Inst.GetMainCharInfo().mainCharID)),
                    characterCustom.transform);
        }
    }

    //PartyInfo 화면에서 유닛을 선택했을때 해당 모델로 교체
    public void ChangeSelectedUnitModel()
    {
        Debug.Log("change update model");
        if (PartyInfoVC.checkInst())
        {
            PartyInfoVC partyInfo = PartyInfoVC.Inst;

            if (objMonsterModel != null)
            {
                Destroy(objMonsterModel);
            }

            if (partyInfo.selectedMenu == PartyInfoVC.menu_type.SERVANT)
            {
                objServantModel.SetActive(false);
                int servant_id = partyInfo.ServantList[partyInfo.selected_unit_idx].id;
                characterCustom.jobIndex = CSVData.Inst.GetServantData(servant_id).job;
                characterCustom.headIndex = CSVData.Inst.GetServantData(servant_id).head;
                characterCustom.hairIndex = CSVData.Inst.GetServantData(servant_id).hair;
                characterCustom.isMan = CSVData.Inst.GetServantData(servant_id).gender;
                characterCustom.isChildren = CSVData.Inst.GetServantData(servant_id).body;
                characterCustom.Refresh();
                objServantModel.SetActive(true);
                changeCameraOffsetByModelHeight(objServantModel.GetComponent<CharInfo>().Height);
            }
            else if (partyInfo.selectedMenu == PartyInfoVC.menu_type.MONSTER)
            {
                objServantModel.SetActive(false);
                objMonsterModel = Instantiate(Resources.Load<GameObject>("InGameCharacterPrefabs/" + CSVData.Inst.GetMonsterDBResourceModel(partyInfo.MonsterList[partyInfo.selected_unit_idx].id)),
                        characterCustom.transform);
                changeCameraOffsetByModelHeight(objMonsterModel.GetComponent<CharInfo>().Height);
            }
        }

    }

    private void changeCameraOffsetByModelHeight(float model_height)
    {
        if (model_height >= 1.8f)
        {
            modelCamera.transform.localPosition = new Vector3(0, 0.5f, -1.3f);
        }
        else if (model_height >= 1.5f && model_height < 1.8f)
        {
            modelCamera.transform.localPosition = new Vector3(0, 0.4f, -1.3f);
        }
        else if (model_height < 1.5f)
        {
            modelCamera.transform.localPosition = new Vector3(0, 0.3f, -1.3f);
        }

    }
}
