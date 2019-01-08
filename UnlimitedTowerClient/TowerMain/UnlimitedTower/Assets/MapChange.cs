using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChange : MonoBehaviour {
    public GameObject[] MapPack = new GameObject[3];

    private void Start()
    {
        MapPack[0] = transform.GetChild(0).gameObject;
        MapPack[1] = transform.GetChild(1).gameObject;
        MapPack[2] = transform.GetChild(2).gameObject;
    }

    void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MapPack[0].SetActive(true);
            MapPack[1].SetActive(false);
            MapPack[2].SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MapPack[0].SetActive(false);
            MapPack[1].SetActive(true);
            MapPack[2].SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MapPack[0].SetActive(false);
            MapPack[1].SetActive(false);
            MapPack[2].SetActive(true);
        }
    }
}
