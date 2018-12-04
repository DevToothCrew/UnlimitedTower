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
        foreach(var num in damage.ToString())
        {
            var ret = -1;
            int.TryParse(num.ToString(), out ret);
            InstantiateNumber(ret, target.transform.position + Vector3.up * _DIST_FROM_HEAD + i * Camera.main.transform.right * _DIST_EACH_NUMBER);
            i++;
        }
    }

    public GameObject InstantiateNumber(int num, Vector3 position)
    {
        var go = Instantiate(prefNum, position, Quaternion.identity, null);
        
        // should look the main camera always
        go.transform.LookAt(Camera.main.transform.position);

        var numberUI = go.GetComponent<NumberUI>();

        if (num < texNums.Length)
        {
            numberUI.SetTexture(texNums[num]);
        }

        return go;
    }
}
