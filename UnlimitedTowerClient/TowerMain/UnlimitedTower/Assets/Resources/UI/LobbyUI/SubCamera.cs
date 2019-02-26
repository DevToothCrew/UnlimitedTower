using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour {

    [SerializeField] GameObject _CharObj;
    [SerializeField] Vector3 CharPos;

    public static SubCamera instance;
    private void Awake()
    {
        instance = this;
    }


    public void Register(UserServantData servant)
    {
        if (_CharObj != null)
        {
            Destroy(_CharObj);
        }


        // 캐릭터 카메라
        
        // Servant Body Prefab
        //_CharObj = Instantiate(ErdManager.instance.ServantBodyPrefabs[servant.body]);
        _CharObj.transform.position = CharPos;
        _CharObj.transform.eulerAngles = new Vector3(0, -160, 0);
    }
    public void Register(UserMonsterData monster)
    {
        if (_CharObj != null)
        {
            Destroy(_CharObj);
        }


        // 캐릭터 카메라
        // Monster Body Prefabs
        //_CharObj = Instantiate(ErdManager.instance.getMonsterPrefabs(monster.monsterNum, monster.monsterTypeNum));
        _CharObj.transform.position = CharPos;
        _CharObj.transform.eulerAngles = new Vector3(0, -160, 0);
    }
    public void Deregister()
    {
        // 캐릭터 카메라
        if (_CharObj != null)
        {
            Destroy(_CharObj);
        }
    }
}
