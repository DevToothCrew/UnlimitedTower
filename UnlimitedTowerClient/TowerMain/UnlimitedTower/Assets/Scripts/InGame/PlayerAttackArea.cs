using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackArea : MonoBehaviour {

    // Use this for initialization
   // private CS_Status m_csStatus;
    private void Awake()
    {
        //m_csStatus = transform.root.GetComponent<CS_Status>();
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.transform.root.name+ " Collision" + other.transform.root.name);
        //// 정확히는 여기서 하면 안되지만 일단은 여기서 하는게..
        //other.transform.root.SendMessage("Damage", m_csStatus.m_nDam);
    }
}
