using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    public GameObject[] pool;
    public int index;

    public void Awake()
    {
        pool = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            pool[i] = transform.GetChild(i).gameObject;
            pool[i].SetActive(false);
        }
    }

    public GameObject ObjectSpawn()
    {
        this.index++;
        int index = this.index % pool.Length;
        pool[index].SetActive(true);
        Material material = pool[index].GetComponent<Renderer>().material;
        material.color = new Color(material.color.r, material.color.g, material.color.b, 1);
        return pool[index];
    }
}
