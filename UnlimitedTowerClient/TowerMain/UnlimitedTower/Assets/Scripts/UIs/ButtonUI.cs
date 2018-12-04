using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour {

    public GameObject[] Button = new GameObject[7];

    public bool onClick = false;

    public void OnClickButton(int index)
    {
        for(int i = 0; i < 7; i++)
        {
            Button[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        }

        Button[index].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1);
    }

    public void OnClickCloseButton()
    {
        for (int i = 0; i < 7; i++)
        {
            Button[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1);
        }
    }
}
