using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChange : MonoSingleton<MapChange>
{
    public GameObject[] MapPack;
    private Camera main_Camera;
    private readonly Color[] backgroundColor = 
        {
        new Color(0.0f,0.0f,0.0f,1.0f),
        new Color(0.29f,1.0f,1.0f,1.0f),
        new Color(1.0f,1.0f,1.0f,1.0f),
        new Color(0.71f,0.71f,0.71f,1.0f),
        new Color(0.84f, 0.98f, 1.0f, 1.0f),
        new Color(0.7f, 0.64f, 0.57f, 1.0f),
        new Color(0.84f, 0.98f, 1.0f, 1.0f),
        new Color(0.24f, 0.22f, 0.19f, 1.0f)
        };
    private readonly Vector3[] cameraPosition =
    {
        new Vector3(-10, 8, -10),
        new Vector3(-10, 10, -10),
        new Vector3(-10, 8, -10),
        new Vector3(-10, 10, -10),
        new Vector3(-10, 8, -10),
        new Vector3(-10, 8, -10),
        new Vector3(-10, 10, -10),
        new Vector3(-10, 10, -10)
    };

    // 임시로 만든 맵 변경, 추후 삭제

    private void Start()
    {
        main_Camera = Camera.main;
        MapPack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            MapPack[i] = transform.GetChild(i).gameObject;
    }

    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            MapEneble(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            MapEneble(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            MapEneble(7);
        }
    }

    public void MapEneble(int index)
    {
        foreach (GameObject a in MapPack)
        {
            a.SetActive(false);
        }
        MapPack[index].SetActive(true);
        main_Camera.backgroundColor = backgroundColor[index];
        main_Camera.transform.position = cameraPosition[index];
    }
}
