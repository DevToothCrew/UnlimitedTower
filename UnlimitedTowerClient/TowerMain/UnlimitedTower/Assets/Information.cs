using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Information : MonoBehaviour
{
    public float AttackDelay;
    public float AttackAfterDelay;
    public float Height;
    public GameObject Arrow;
    public GameObject select;
    public Collider CheckBox;
    private RaycastHit hit;
    private Ray ray;

    public void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.collider.Equals(CheckBox))
        {
            select.SetActive(true);
            select.SendMessage("AniStart");

            if (Input.GetMouseButtonDown(0))
            {
                select.SendMessage("Click");
            }
        }
        else
        {
            select.SetActive(false);
        }

        
    }
}
