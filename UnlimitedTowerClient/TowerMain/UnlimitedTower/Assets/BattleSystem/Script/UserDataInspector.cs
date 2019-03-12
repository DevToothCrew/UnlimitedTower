using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataInspector : MonoBehaviour {
    public List<UserServantData> userServantData;
    public List<UserMonsterData> userMonsterData;
    public List<UserMountItemData> userMountItemData;
    public List<UserEtcItemData> userEtcItemData;

    void Start () {
        userServantData = new List<UserServantData>(UserDataManager.Inst.servantDic.Values);
        userMonsterData = new List<UserMonsterData>(UserDataManager.Inst.monsterDic.Values);
        userMountItemData = new List<UserMountItemData>(UserDataManager.Inst.mountItemDic.Values);
        userEtcItemData = new List<UserEtcItemData>(UserDataManager.Inst.etcItemDic.Values);
    }
}
