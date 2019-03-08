using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieCameraMove : MonoSingleton<DieCameraMove>
{
    public GameObject camera_Obj;
    public Camera camera_C;
    public readonly Vector3 _DISTANCE = new Vector3(-11.1f, 10.0f, -11.1f);
    public Vector3 startPos;

    private void Start()
    {
        camera_Obj = GameObject.Find("Main Camera");
        camera_C = camera_Obj.GetComponent<Camera>();
        startPos = camera_Obj.transform.position;
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            camera_C.orthographicSize += 5.0f;
        }
    }

    public void Test(int target)
    {
        StartCoroutine(Move(target));
    }

    IEnumerator Move(int target)
    {
        yield return new WaitForSeconds(1.0f);

        startPos = camera_Obj.transform.position;
        float startSize = camera_C.orthographicSize;
        Vector3 temp = camera_Obj.transform.position;
        Vector3 dump = temp;
        for (int i = 0; i < 25; i += BattleSystem.Inst.TimeScale)
        {
                camera_Obj.transform.position = Vector3.Lerp(temp, _DISTANCE + BattleSystem.Inst.characterControl[target].transform.position, i * 0.04f);
            if (camera_C.orthographicSize > 1.0f)
            camera_C.orthographicSize -= 0.16f;
            yield return null;
        }

        yield return new WaitForSeconds(4.0f);

        temp = camera_Obj.transform.position;
        for (int i = 0; i < 25; i += BattleSystem.Inst.TimeScale)
        {
            camera_Obj.transform.position = Vector3.Lerp(temp, startPos, i * 0.04f);
            if (camera_C.orthographicSize < 6.0f)
                camera_C.orthographicSize += 0.16f;
            yield return null;
        }
        camera_C.orthographicSize = startSize;
        camera_Obj.transform.position = startPos;
    }
}
