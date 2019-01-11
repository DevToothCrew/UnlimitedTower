using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour {
    public Image[] image = new Image[5];
    private void Start()
    {
        for (int i = 0; i < 5; i++)
            image[i] = transform.GetChild(i).GetChild(0).GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(OFF());
    }

    IEnumerator OFF()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
