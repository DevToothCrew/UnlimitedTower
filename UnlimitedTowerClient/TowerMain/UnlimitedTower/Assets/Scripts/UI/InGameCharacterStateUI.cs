using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameCharacterStateUI : MonoBehaviour {


    [System.Serializable]
    public class UserStateInfo
    {
        public Sprite thumbnail;

        public int level;
        public string name;

        // will not use as array, cuz of a limitation of slot.
        public Sprite slot1, slot2, slot3;

        [System.Serializable]
        public class State
        {
            public Sprite image;
            public string value;
        }

        // will not use as array, cuz of a limitation of slot.
        public State state1, state2, state3, state4, state5, state6;
    }

    [HideInInspector]
    public UserStateInfo userStateInfo;

    [Space(20)]
    [Header("Connection with UIs")]
    [Space(10)]
    public Image uiThumbnailImage;
    public Text uiLevelText;
    public Text uiNameText;
    public Image[] uiSlotImages;
    public Image[] uiStatusImages;
    public Text[] uiStatusTexts;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
