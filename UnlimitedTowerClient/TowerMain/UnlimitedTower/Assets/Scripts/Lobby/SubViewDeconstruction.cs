using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubViewDeconstruction : MonoBehaviour {
    public Text textTitle;

    public DecontructionSlot[] slotList = new DecontructionSlot[9];

    public Text textEos;
    public Text textUgt;

    public Button buttonGrind;
    public Button buttonClear;

    void Start()
    {
        updateView();
    }

    void updateView()
    {
        textEos.text = string.Format("{0}", 0);
        textUgt.text = string.Format("{0}", 0);
    }

    void updateSlot()
    {

    }

    public void OnClickButtonGrind()
    {

    }

    public void OnClickButtonClear()
    {

    }

}
