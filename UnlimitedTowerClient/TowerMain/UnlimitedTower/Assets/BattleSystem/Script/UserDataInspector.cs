using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataInspector : MonoBehaviour {
    public List<UserServantData> userServantData;
    public List<UserMonsterData> userMonsterData;
    public List<UserEquipmentData> userEquipmentData;
    public List<UserItemData> userEtcItemData;

    void Start () {
        userServantData = new List<UserServantData>(UserDataManager.Inst.servantDic.Values);
        userMonsterData = new List<UserMonsterData>(UserDataManager.Inst.monsterDic.Values);
        userEquipmentData = new List<UserEquipmentData>(UserDataManager.Inst.equipmentDic.Values);
        userEtcItemData = new List<UserItemData>(UserDataManager.Inst.itemDic.Values);
    }
}
