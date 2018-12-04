using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationDeck : MonoBehaviour {

    public int DeckNum = -1;
    public bool Active = false;
    public bool HeroDeck = false;

    // 덱과 연결된 캐릭터 목록
    public GameObject LinkedChar = null;


    public void Activate()
    {
        GetComponent<FormationDeck>().Active = true;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
}
