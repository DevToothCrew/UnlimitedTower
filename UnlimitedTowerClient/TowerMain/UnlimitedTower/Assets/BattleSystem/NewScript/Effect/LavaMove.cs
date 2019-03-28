using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMove : MonoBehaviour {
    public GameObject[] lava = new GameObject[6];

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            lava[i] = transform.GetChild(i).gameObject;
        }
    }

    private void OnEnable()
    {
        lava[0].transform.position = new Vector3(0, -2.5f, 0);
        lava[1].transform.position = new Vector3(30.72f, -2.5f, 0);
        StartCoroutine(Move01());
        StartCoroutine(Move02());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Move01()
    {
        for (; ; )
        {
            if (lava[0].transform.position.x < -30)
            {
                lava[0].transform.position = lava[1].transform.position + new Vector3(30.72f, 0, 0);
            }
            else if (lava[1].transform.position.x < -30)
            {
                lava[1].transform.position = lava[0].transform.position + new Vector3(30.72f, 0, 0);
            }

            lava[0].transform.Translate(-0.05f * BattleManager.Inst.TimeScale, 0, 0);
            lava[1].transform.Translate(-0.05f * BattleManager.Inst.TimeScale, 0, 0);

            if (lava[0].transform.position.y < -0.1f)
            {
                lava[0].transform.Translate(0, 0, -0.001f * BattleManager.Inst.TimeScale);
                lava[1].transform.Translate(0, 0, -0.001f * BattleManager.Inst.TimeScale);
            }
            else
            {
                lava[0].transform.position = new Vector3(lava[0].transform.position.x, -0.1f, lava[0].transform.position.z);
                lava[1].transform.position = new Vector3(lava[1].transform.position.x, -0.1f, lava[1].transform.position.z);
            }

            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    IEnumerator Move02()
    {
        for (; ; )
        {
            if (lava[2].transform.position.y < -20)
            {
                lava[2].transform.position = lava[4].transform.position + new Vector3(0, 15.2025f, 0);
                lava[3].transform.position = lava[5].transform.position + new Vector3(0, 15.2025f, 0);
            }
            else if (lava[4].transform.position.y < -20)
            {
                lava[4].transform.position = lava[2].transform.position + new Vector3(0, 15.2025f, 0);
                lava[5].transform.position = lava[3].transform.position + new Vector3(0, 15.2025f, 0);
            }

            lava[2].transform.Translate(0, -0.05f * BattleManager.Inst.TimeScale, 0);
            lava[3].transform.Translate(0, -0.05f * BattleManager.Inst.TimeScale, 0);
            lava[4].transform.Translate(0, -0.05f * BattleManager.Inst.TimeScale, 0);
            lava[5].transform.Translate(0, -0.05f * BattleManager.Inst.TimeScale, 0);

            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

}
