using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListManager : MonoSingleton<CharacterListManager>
{

    public GameObject CharacterListScroll;
    public GameObject ServantContentList;
    public GameObject MonsterContentList;

    // Use this for initialization
    void Awake () {
        //]
        //CharacterListScroll = GameObject.Find("CharacterListScroll");
        //ServantContentList = GameObject.Find("ServantContentList");
        //MonsterContentList = GameObject.Find("MonsterContentList");

    }
	
}
