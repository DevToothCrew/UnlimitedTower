using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoSingleton<BattleSystem>
{
    public PrefabList prefabList;
    public BattleInformation battleInformation;
    public StageStateData stageStateData;

    public List<BattleActionData> battleData;

    public GameObject[] PlayerCharacter = new GameObject[10];
    public CharacterControl[] PlayerCharacterControl = new CharacterControl[10];

    public GameObject[] EnemyCharacter = new GameObject[10];
    public CharacterControl[] EnemyCharacterControl = new CharacterControl[10];

    public int TimeScale = 1;
    public GameObject testEffect;

    public UTLobbyUIManager UTLobbyUIManager_;
    public int[] positionOrder = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9 };

    public struct BattleInformation
    {
        public int AttackerIndex;
        public int TargetIndex;
        public int Damage;
        public bool isAvoid;
        public bool isCritical;
        public bool isPlayerTurn;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        UTLobbyUIManager_ = GameObject.Find("Framework").GetComponent<UTLobbyUIManager>();
    }

    public void Start()
    {
        UTLobbyUIManager_.StageStart();

        battleInformation.AttackerIndex = -1;
        prefabList = GetComponent<PrefabList>();

        for (int i = 0; i < 20; i++)
        {
            if (i < 10)
                Instantiate(prefabList.prefabList[stageStateData.info_list[i].index].Prefab, PlayerCharacter[positionOrder[i]].transform);
            else
                Instantiate(prefabList.prefabList[stageStateData.info_list[i].index].Prefab, EnemyCharacter[positionOrder[i - 10]].transform);
        }

        // for (int i = 0; i < 20; i++)
        // {
        //     if (i == 0)
        //         Instantiate(prefabList.prefabList[5].Prefab, PlayerCharacter[positionOrder[i]].transform);
        //     else if (i < 10)
        //         Instantiate(prefabList.prefabList[201 + Random.Range(0,29)].Prefab, PlayerCharacter[positionOrder[i]].transform);
        //     else
        //         Instantiate(prefabList.prefabList[201 + Random.Range(0, 29)].Prefab, EnemyCharacter[positionOrder[i - 10]].transform);
        // }

        for (int i = 0; i < 10; i++)
        {
            if (i < 5)
            {
                PlayerCharacter[i].transform.position = new Vector3(-4.2f + 2.1f * (i % 5), 0, -4.2f);
                EnemyCharacter[i].transform.position = new Vector3(4.2f - 2.1f * (i % 5), 0, 4.2f);
            }
            else
            {
                PlayerCharacter[i].transform.position = new Vector3(-4.2f + 2.1f * (i % 5), 0, -2.1f);
                EnemyCharacter[i].transform.position = new Vector3(4.2f - 2.1f * (i % 5), 0, 2.1f);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            PlayerCharacterControl[i] = PlayerCharacter[i].GetComponent<CharacterControl>();
            EnemyCharacterControl[i] = EnemyCharacter[i].GetComponent<CharacterControl>();
            PlayerCharacterControl[i].index = i;
            EnemyCharacterControl[i].index = i;
            PlayerCharacterControl[i].isPlayer = true;
            EnemyCharacterControl[i].isPlayer = false;
        }

        for (int i = 0; i < 20; i++)
        {
            if (i < 10)
            {
                PlayerCharacterControl[i].MaxHp = stageStateData.info_list[positionOrder[i]].now_hp; // 나중에 최대체력 변경
                PlayerCharacterControl[i].NowHp = stageStateData.info_list[positionOrder[i]].now_hp;
                PlayerCharacterControl[i].MaxHp = 300;
                PlayerCharacterControl[i].NowHp = 300;
            }
            else
            {
                EnemyCharacterControl[i - 10].MaxHp = stageStateData.info_list[positionOrder[i - 10]].now_hp;  // 나중에 최대체력 변경
                EnemyCharacterControl[i - 10].NowHp = stageStateData.info_list[positionOrder[i - 10]].now_hp;
                EnemyCharacterControl[i - 10].MaxHp = 300;
                EnemyCharacterControl[i - 10].NowHp = 300;
            }
        }
    }

    [ContextMenu("AttackTest")]
    public void AttackTest()
    {
        StartCoroutine(BattleTestTarget());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackTest();
        }
    }

    IEnumerator BattleTestTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            battleInformation.AttackerIndex = i;
            battleInformation.TargetIndex = i;
            battleInformation.Damage = 10;
            battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
            battleInformation.isAvoid = false;
            battleInformation.isPlayerTurn = true;

            PlayerCharacterControl[battleInformation.AttackerIndex].Attack(new SendValue(
                battleInformation.AttackerIndex,
                battleInformation.TargetIndex,
                battleInformation.Damage,
                battleInformation.isCritical,
                battleInformation.isAvoid,
                battleInformation.isPlayerTurn));
            yield return new WaitForSeconds(7);
            battleInformation.AttackerIndex = i;
            battleInformation.TargetIndex = i;
            battleInformation.Damage = 10;
            battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
            battleInformation.isAvoid = false;
            battleInformation.isPlayerTurn = false;

            EnemyCharacterControl[battleInformation.AttackerIndex].Attack(new SendValue(
                battleInformation.AttackerIndex,
                battleInformation.TargetIndex,
                battleInformation.Damage,
                battleInformation.isCritical,
                battleInformation.isAvoid,
                battleInformation.isPlayerTurn));
            yield return new WaitForSeconds(7);
        }
    }

    IEnumerator BattleTestRandom()
    {
        for (int i = 0; i < 10; i++)
        {
            battleInformation.AttackerIndex = Random.Range(0, 10);
            battleInformation.TargetIndex = Random.Range(0, 10);
            battleInformation.Damage = Random.Range(100, 200);
            battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
            battleInformation.isAvoid = Random.Range(0, 5) == 1 ? true : false;
            battleInformation.isPlayerTurn = true;

            PlayerCharacterControl[battleInformation.AttackerIndex].Attack(new SendValue(
                battleInformation.AttackerIndex,
                battleInformation.TargetIndex,
                battleInformation.Damage,
                battleInformation.isCritical,
                battleInformation.isAvoid,
                battleInformation.isPlayerTurn));
            yield return new WaitForSeconds(7);
            {
                battleInformation.AttackerIndex = Random.Range(0, 10);
                battleInformation.TargetIndex = Random.Range(0, 10);
                battleInformation.Damage = Random.Range(100, 200);
                battleInformation.isCritical = Random.Range(0, 2) == 1 ? true : false;
                battleInformation.isAvoid = Random.Range(0, 5) == 1 ? true : false;
                battleInformation.isPlayerTurn = false;

                EnemyCharacterControl[battleInformation.AttackerIndex].Attack(new SendValue(
                    battleInformation.AttackerIndex,
                    battleInformation.TargetIndex,
                    battleInformation.Damage,
                    battleInformation.isCritical,
                    battleInformation.isAvoid,
                    battleInformation.isPlayerTurn));
            }
            yield return new WaitForSeconds(7);
        }
    }

    IEnumerator BattleStart()
    {
        for (int i = 0; i < battleData.Count; i++)
        {
            for (int j = 0; j < battleData[i].info_list.Count; j++)
            {
                if (battleData[i].info_list[j].index < 10)
                {
                    battleInformation.AttackerIndex = battleData[i].info_list[j].index;
                    battleInformation.TargetIndex = battleData[i].info_list[j].action_list[0].target_index;
                    battleInformation.Damage = battleData[i].info_list[j].action_list[0].damage;
                    battleInformation.isCritical = battleData[i].info_list[j].action_list[0].critical;
                    battleInformation.isAvoid = battleData[i].info_list[j].action_list[0].avoid;
                    battleInformation.isPlayerTurn = true;

                    PlayerCharacterControl[battleInformation.AttackerIndex].Attack(new SendValue(
                        battleInformation.AttackerIndex,
                        battleInformation.TargetIndex,
                        battleInformation.Damage,
                        battleInformation.isCritical,
                        battleInformation.isAvoid,
                        battleInformation.isPlayerTurn));
                }
                else
                {
                    battleInformation.AttackerIndex = battleData[i].info_list[j].index - 10;
                    battleInformation.TargetIndex = battleData[i].info_list[j].action_list[0].target_index - 10;
                    battleInformation.Damage = battleData[i].info_list[j].action_list[0].damage;
                    battleInformation.isCritical = battleData[i].info_list[j].action_list[0].critical;
                    battleInformation.isAvoid = battleData[i].info_list[j].action_list[0].avoid;
                    battleInformation.isPlayerTurn = false;

                    EnemyCharacterControl[battleInformation.AttackerIndex].Attack(new SendValue(
                        battleInformation.AttackerIndex,
                        battleInformation.TargetIndex,
                        battleInformation.Damage,
                        battleInformation.isCritical,
                        battleInformation.isAvoid,
                        battleInformation.isPlayerTurn));
                }
                yield return new WaitForSeconds(7);
            }
        }
    }
}
/*
 public interface IVisibleListener
{
    void IVisibleListener_OnVisible(bool bIsVisible);
}
     
     */
