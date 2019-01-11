using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {
    public GameObject child;
    public BoxCollider checkBox;
    public GameObject select;
    public int index;
    public bool isPlayer;
    public int MaxHp;
    public int NowHp;
    public bool isDie;

    private void Start()
    {
        checkBox = GetComponent<BoxCollider>();
        select = transform.GetChild(0).GetChild(0).gameObject;
        child = transform.GetChild(0).gameObject;
    }

    private void FixedUpdate()
    {
        if (NowHp <= 0)
        {
            if (!isDie)
            {
                child.GetComponent<Animator>().SetTrigger("isDie");
                isDie = true;
            }
        }
    }

    private void OnEnable()
    {
        if (transform.childCount != 0)
        select = transform.GetChild(0).GetChild(0).gameObject;
        checkBox = GetComponent<BoxCollider>();
        if (transform.childCount != 0)
            child = transform.GetChild(0).gameObject;
    }

    public void Attack(SendValue sendValue)
    {
        child.SendMessage("Attack", sendValue);
    }
}
