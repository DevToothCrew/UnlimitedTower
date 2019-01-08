using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {
    public GameObject child;
    public int MaxHp;
    public int NowHp;
    public bool isDie;

    private void Start()
    {
        child = transform.GetChild(0).gameObject;
    }

    public void Attack(SendValue sendValue)
    {
        child.SendMessage("Attack", sendValue);
    }
}
