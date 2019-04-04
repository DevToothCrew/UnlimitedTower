using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPool : MonoBehaviour
{
    public DamageText[] pool;
    public int index;

    public struct DamageText
    {
        public GameObject obj;
        public Text text;
        public Outline outLine;
    }

    public void Awake()
    {
        pool = new DamageText[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            pool[i].obj = transform.GetChild(i).gameObject;
            pool[i].text = transform.GetChild(i).GetChild(0).GetComponent<Text>();
            pool[i].outLine = transform.GetChild(i).GetChild(0).GetComponent<Outline>();
            pool[i].obj.SetActive(false);
        }
    }

    public DamageText ObjectSpawn()
    {
        index++;
        if (index >= pool.Length)
            index = 0;
        pool[index].obj.SetActive(true);
        return pool[index];
    }
}
