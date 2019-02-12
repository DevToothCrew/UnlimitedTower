using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieCameraMove : MonoSingleton<DieCameraMove>
{
    public GameObject camera_Obj;
    public Camera camera_C;
    public Vector3 distance;
    public Vector3 startPos;

    private void Start()
    {
        camera_Obj = GameObject.Find("Main Camera");
        camera_C = camera_Obj.GetComponent<Camera>();
        startPos = camera_Obj.transform.position;
        distance = new Vector3(-11.2f, 11.8f, -12.1f);
    }
    
    public void Test(int target, bool isPlayer)
    {
        StartCoroutine(Move(target, isPlayer));
    }

    IEnumerator Move(int target, bool isPlayer)
    {
        yield return new WaitForSeconds(1.0f);

        Vector3 temp = camera_Obj.transform.position;
        for (int i = 0; i < 25; i++)
        {
            if (isPlayer)
                camera_Obj.transform.position = Vector3.Lerp(temp, BattleSystem.Inst.playerCharacter[target].transform.position + distance, i * 0.04f);
            else
                camera_Obj.transform.position = Vector3.Lerp(temp, BattleSystem.Inst.enemyCharacter[target].transform.position + distance, i * 0.04f);
            if (camera_C.orthographicSize > 1.0f)
            camera_C.orthographicSize -= 0.16f;
            yield return null;
        }

        yield return new WaitForSeconds(4.0f);

        temp = camera_Obj.transform.position;
        for (int i = 0; i < 25; i++)
        {
            camera_Obj.transform.position = Vector3.Lerp(temp, startPos, i * 0.04f);
            if (camera_C.orthographicSize < 5.0f)
                camera_C.orthographicSize += 0.16f;
            yield return null;
        }
    }
}
