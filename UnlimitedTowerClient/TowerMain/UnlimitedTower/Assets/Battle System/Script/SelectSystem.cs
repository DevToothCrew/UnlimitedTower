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

    [Header("- Character State")]
    public Text attackDamageText;
    public Text defenceText;
    public Text speedText;

    private RaycastHit hit;
    private Ray ray;
    private CheckSelectAnimation temp;
    private bool isStart = false;

    private void Start()
    {
        attackDamageText = GameObject.Find("StatueAttackText").GetComponent<Text>();
        defenceText = GameObject.Find("StatueDefenceText").GetComponent<Text>();
        speedText = GameObject.Find("StatueSpeedText").GetComponent<Text>();
    }

    // 추후 최적화 작업, timeScale도 바꿔야함
    void Update()
    {
        if (!isStart)
        {
            selectCharacterImage = GameObject.Find("Character Portrait Image").GetComponent<Image>();
            for (int i = 0; i < 10; i++)
            {
                if (BattleSystem.Inst.characterisPlace[i] == true)
                    chsing[i] = BattleSystem.Inst.playerCharacterControl[i].select.GetComponent<CheckSelectAnimation>();
                if (BattleSystem.Inst.characterisPlace[i + 10] == true)
                    chsing[i + 10] = BattleSystem.Inst.enemyCharacterControl[i].select.GetComponent<CheckSelectAnimation>();
            }
            selectHpBar = GameObject.Find("Hp Bar").GetComponent<Image>();
            selectHpText = GameObject.Find("Hp Text").GetComponent<Text>();
            isStart = true;
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

                    temp.gameObject.SetActive(true);
                    temp.AniStart();

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
                        // 데이터 들어오면 주석 해제
                        // attackDamageText.text = BattleSystem.Inst.stageStateData.info_list[selectIndex].damage.ToString();
                        // defenceText.text = BattleSystem.Inst.stageStateData.info_list[selectIndex].defence.ToString();
                        // speedText.text = BattleSystem.Inst.stageStateData.info_list[selectIndex].speed.ToString();
                    }
                }
            }

            if (selectIndex != -1)
            {
                if (selectIndex < 10)
                {
                    selectHpBar.fillAmount = (float)BattleSystem.Inst.playerCharacterControl[selectIndex].nowHp / BattleSystem.Inst.playerCharacterControl[selectIndex].maxHp;
                    selectHpText.text = BattleSystem.Inst.playerCharacterControl[selectIndex]?.nowHp.ToString();
                    // 데이터 들어오면 주석 해제
                    // selectCharacterImage.sprite = BattleSystem.Inst.prefabList.prefabList[BattleSystem.Inst.stageStateData.info_list[selectIndex].index]?.sprite;
                }
                else
                {
                    selectHpBar.fillAmount = (float)BattleSystem.Inst.enemyCharacterControl[selectIndex - 10].nowHp / BattleSystem.Inst.enemyCharacterControl[selectIndex - 10].maxHp;
                    selectHpText.text = BattleSystem.Inst.enemyCharacterControl[selectIndex - 10]?.nowHp.ToString();
                    // 데이터 들어오면 주석 해제
                    // selectCharacterImage.sprite = BattleSystem.Inst.prefabList.prefabList[BattleSystem.Inst.stageStateData.info_list[selectIndex - 10].index]?.sprite;
                }
            }
        }
    }
}
