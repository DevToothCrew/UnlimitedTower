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
    private bool isStart = false;
    public int[] positionOrder = { 3, 1, 0, 2, 4, 8, 6, 5, 7, 9 };

    // 추후 최적화 작업, timeScale도 바꿔야함
    void Update()
    {
        if (!isStart)
        {
            selectCharacterImage = GameObject.Find("Character Portrait Image").GetComponent<Image>();
            for (int i = 0; i < 10; i++)
            {
                if (BattleSystem.Inst.characterisVoid[i] == true)
                    chsing[i] = BattleSystem.Inst.playerCharacterControl[i].select.GetComponent<CheckSelectAnimation>();
                if (BattleSystem.Inst.characterisVoid[i + 10] == true)
                    chsing[i + 10] = BattleSystem.Inst.enemyCharacterControl[i].select.GetComponent<CheckSelectAnimation>();
            }
            selectHpBar = GameObject.Find("Hp Bar").GetComponent<Image>();
            selectHpText = GameObject.Find("Hp Text").GetComponent<Text>();
        }

        if (BattleSystem.Inst.TimeScale != 0)
        {
            for (int i = 0; i < 20; i++)
                if (i != selectIndex)
                    chsing[i]?.gameObject.SetActive(false);

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
                    selectHpBar.fillAmount = (float)BattleSystem.Inst.playerCharacterControl[selectIndex].nowHp / BattleSystem.Inst.playerCharacterControl[selectIndex].maxHp;
                    selectHpText.text = BattleSystem.Inst.playerCharacterControl[selectIndex].nowHp.ToString();
                    selectCharacterImage.sprite = BattleSystem.Inst.prefabList.prefabList[BattleSystem.Inst.stageStateData.info_list[selectIndex].index].sprite;
                }
                else
                {
                    selectHpBar.fillAmount = (float)BattleSystem.Inst.enemyCharacterControl[selectIndex - 10].nowHp / BattleSystem.Inst.enemyCharacterControl[selectIndex - 10].maxHp;
                    selectHpText.text = BattleSystem.Inst.enemyCharacterControl[selectIndex - 10].nowHp.ToString();
                    selectCharacterImage.sprite = BattleSystem.Inst.prefabList.prefabList[BattleSystem.Inst.stageStateData.info_list[selectIndex - 10].index].sprite;
                }
            }
        }
    }
}
