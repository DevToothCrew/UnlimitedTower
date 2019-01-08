using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultPopup : MonoBehaviour {

    [SerializeField]
    private Animator animator = null;

    private static GachaResultPopup instance = null;

    private void Awake()
    {

        // set instance
        instance = instance ?? this;
        
        // set animator
        animator = GetComponentInChildren<Animator>();

        gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator waitAndDisable(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        instance.animator.SetTrigger("SetInvisible");
        StartCoroutine(waitAndDisable(1.0f));
    }

    static public void Popup()
    {
        instance.gameObject.SetActive(true);
        instance.animator.SetTrigger("SetVisible");
    }

}
