using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginRewardVC : MonoSingleton<LoginRewardVC> {
    public Text textTitle;
    public Transform frameUnits;
    public GameObject prefabLoginRewardUnit;

    private const int MaxLoginRewardDay = 28;

    //임시 데이터 

	void Start () {
        updateView();
    }

    public void updateView()
    {
        for (int i=0; i< MaxLoginRewardDay; i++)
        {
            Transform transformPrefab = Instantiate(prefabLoginRewardUnit.transform);
            transformPrefab.SetParent(frameUnits);
            transformPrefab.GetComponent<LoginRewardUnit>().updateUnit(i);
       }
    }

    public void OnClickOK()
    {
        Destroy(this.gameObject);
    }
}
