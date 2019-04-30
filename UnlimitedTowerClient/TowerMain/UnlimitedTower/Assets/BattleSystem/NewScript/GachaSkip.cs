using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaSkip : MonoBehaviour
{
    public GachaBoxParticleController skipScirpt;
    public int[] CheetKey = new int[3];
    public int CheetIndex;
    public bool adminMode;

    // Update is called once per frame
    void Update () {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CheetKey[CheetIndex % 3] = (int)KeyCode.A;
                DebugLog.Log(false,"" + CheetKey[CheetIndex % 3]);
                CheetIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                CheetKey[CheetIndex % 3] = (int)KeyCode.D;
                DebugLog.Log(false,"" + CheetKey[CheetIndex % 3]);
                CheetIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                CheetKey[CheetIndex % 3] = (int)KeyCode.M;
                DebugLog.Log(false,"" + CheetKey[CheetIndex % 3]);
                CheetIndex++;
            }
            else
            {
                CheetKey = new int[3];
            }
        }

        if (CheetKey[0] + CheetKey[1] + CheetKey[2] == 306)
        {
            adminMode = true;
        }
        if (adminMode && Input.GetKeyDown(KeyCode.Space))
        {
            skipScirpt.FinishAnimation();
        }
    }
}
