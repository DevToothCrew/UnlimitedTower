using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginRewardVC : MonoSingleton<LoginRewardVC> {
    public Text textTitle;
    public Transform frameUnits;
    public GameObject prefabLoginRewardUnit;

    public Sprite spriteFrameGotReward;
    public Sprite spriteFrameReward;
    public Sprite spriteGotDay;
    public Sprite spriteDay;

    private const int MaxLoginRewardDay = 28;

    private List<LoginRewardUnit> loginRewardUnits = new List<LoginRewardUnit>();

    //임시 데이터 

	void Start () {
        initUnits();
    }

    void initUnits()
    {
        loginRewardUnits.Clear();
        for (int i = 0; i < MaxLoginRewardDay; i++)
        {
            Transform transformPrefab = Instantiate(prefabLoginRewardUnit.transform);
            transformPrefab.SetParent(frameUnits);
            loginRewardUnits.Add(transformPrefab.GetComponent<LoginRewardUnit>());
            loginRewardUnits[i].updateUnit(i);
        }
    }

    public void updateView()
    {
        for (int i = 0; i < MaxLoginRewardDay; i++)
        {
            loginRewardUnits[i].updateUnit(i);
        }
    }

    public void OnClickOK()
    {
        Destroy(this.gameObject);
    }
}
