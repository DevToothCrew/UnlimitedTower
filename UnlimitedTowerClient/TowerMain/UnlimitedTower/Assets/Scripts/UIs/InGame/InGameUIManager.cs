using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoSingleton<InGameUIManager>
{
    private readonly float _DIST_FROM_HEAD = 0.8f;
    private readonly float _DIST_EACH_NUMBER = 0.2f;

    [SerializeField]
    private Texture2D[] texNums;

    [SerializeField]
    private GameObject prefNum;

    private void Awake()
    {
        BattleManager.Inst.decreaseHpDelegate += DecreaseHp;
    }

    public void DecreaseHp(int damage, GameObject target)
    {
        var i = 0;
        InstantiateNumber(damage, target.transform.position + Vector3.up * _DIST_FROM_HEAD);
    }

    public GameObject InstantiateNumber(int num, Vector3 position)
    {
        var go = Instantiate(prefNum, position, Quaternion.identity, null);
        
        // should look the main camera always
        go.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);

        var numberUI = go.GetComponent<InGameNumberUI>();
        if(numberUI != null)
        {
            numberUI.SetValue(num);
        }

        return go;
    }
}
