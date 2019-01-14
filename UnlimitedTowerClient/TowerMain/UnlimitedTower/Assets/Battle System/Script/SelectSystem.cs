using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSystem : MonoSingleton<SelectSystem>
{
    public CheckSelectAnimation[] chsing = new CheckSelectAnimation[20];
    public int selectIndex = -1;
    public bool isPlayer;
    public Image selectHpBar;
    public Text selectHpText;
    public Image selectCharacterImage;
    private RaycastHit hit;
    private Ray ray;
    private CheckSelectAnimation temp;

    private void Start()
    {
        selectCharacterImage = GameObject.Find("Character Portrait Image").GetComponent<Image>();
        for (int i = 0; i < 10; i++)
        {
            chsing[i] = BattleSystem.Inst.PlayerCharacterControl[i].select.GetComponent<CheckSelectAnimation>();
        }
        for (int i = 0; i < 10; i++)
        {
            chsing[i + 10] = BattleSystem.Inst.EnemyCharacterControl[i].select.GetComponent<CheckSelectAnimation>();
        }
        selectHpBar = GameObject.Find("Hp Bar").GetComponent<Image>();
        selectHpText = GameObject.Find("Hp Text").GetComponent<Text>();
    }
    
    // 추후 최적화 작업, timeScale도 바꿔야함
    void Update()
    {
        if (BattleSystem.Inst.TimeScale != 0)
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

            if (selectIndex != -1)
            {
                if (selectIndex < 10)
                {
                    selectHpBar.fillAmount = (float)BattleSystem.Inst.PlayerCharacterControl[selectIndex].NowHp / BattleSystem.Inst.PlayerCharacterControl[selectIndex].MaxHp;
                    selectHpText.text = BattleSystem.Inst.PlayerCharacterControl[selectIndex].NowHp.ToString();
                }
                else
                {
                    selectHpBar.fillAmount = (float)BattleSystem.Inst.EnemyCharacterControl[selectIndex - 10].NowHp / BattleSystem.Inst.EnemyCharacterControl[selectIndex - 10].MaxHp;
                    selectHpText.text = BattleSystem.Inst.EnemyCharacterControl[selectIndex - 10].NowHp.ToString();
                }
                selectCharacterImage.sprite = BattleSystem.Inst.prefabList.prefabList[201 + selectIndex].sprite;
            }
        }
    }
}
