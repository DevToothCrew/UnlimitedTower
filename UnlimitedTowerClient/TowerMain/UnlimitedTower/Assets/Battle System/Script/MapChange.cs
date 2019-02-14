using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChange : MonoBehaviour {
    public GameObject[] MapPack;

    // 임시로 만든 맵 변경, 추후 삭제

    private void Start()
    {
        MapPack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            MapPack[i] = transform.GetChild(i).gameObject;
    }

    void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MapEneble(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MapEneble(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MapEneble(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MapEneble(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            MapEneble(4);
        }
    }

    private void MapEneble(int index)
    {
        foreach (GameObject a in MapPack)
        {
            a.SetActive(false);
        }
        MapPack[index].SetActive(true);
    }
}
