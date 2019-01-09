using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSystem : MonoBehaviour
{
    private RaycastHit hit;
    private Ray ray;
    public CheckSelectAnimation[] chsing = new CheckSelectAnimation[20];
    private CheckSelectAnimation temp;
    public int selectIndex = -1;
    public bool isPlayer;

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            chsing[i] = BattleSystem.Inst.PlayerCharacterControl[i].select.GetComponent<CheckSelectAnimation>();
        }
        for (int i = 0; i < 10; i++)
        {
            chsing[i + 10] = BattleSystem.Inst.EnemyCharacterControl[i].select.GetComponent<CheckSelectAnimation>();
        }
    }

    void Update()
    {
        for (int i = 0; i < 20; i++)
            if (i != selectIndex)
                chsing[i].gameObject.SetActive(false);

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Character"))
            {
                temp = hit.transform.GetComponent<CharacterControl>().select.GetComponent<CheckSelectAnimation>();
                isPlayer = hit.transform.GetComponent<CharacterControl>().isPlayer;

                if (isPlayer)
                {
                    temp.gameObject.SetActive(true);
                    temp.AniStart();
                }
                else
                {
                    temp.gameObject.SetActive(true);
                    temp.AniStart();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (isPlayer)
                    {
                        selectIndex = hit.transform.GetComponent<CharacterControl>().index;
                        chsing[selectIndex].Click();
                    }
                    else
                    {
                        selectIndex = hit.transform.GetComponent<CharacterControl>().index + 10;
                        chsing[selectIndex].Click();
                    }
                }
            }
        }

    }
}
