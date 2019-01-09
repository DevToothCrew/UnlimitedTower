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

    private void OnEnable()
    {
        select = transform.GetChild(0).GetChild(0).gameObject;
        checkBox = GetComponent<BoxCollider>();
        child = transform.GetChild(0).gameObject;
    }

    public void Attack(SendValue sendValue)
    {
        child.SendMessage("Attack", sendValue);
    }
}
