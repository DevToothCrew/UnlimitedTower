using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoSingleton<SkillManager>
{
    //public Text skillText;
    public Dictionary<string, GameObject> effect = new Dictionary<string, GameObject>();
    public List<EffectList> effectList = new List<EffectList>();

    [System.Serializable]
    public class EffectList
    {
        public string name;
        public GameObject obj;
    }

    private void Awake()
    {
        //skillText = GameObject.Find("Skill Text").GetComponent<Text>();

        foreach (EffectList effect in effectList)
        {
            this.effect.Add(effect.name, effect.obj);
        }
    }

    #region Skill 200001
    // 디펜스 ( 1턴 동안 30% 데미지 감소 효과를 얻습니다아엉ㄹ러아ㅏ아아아앙ㄹ나아ㅓㄹ아ㅓㅇㄹ니ㅏ러뛰시ㅓ이뚱니랑널니아러니댜러냉리어ㅏㄹ )
    public void Skill_200001(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200001_Co(battleInfo));
    }

    public IEnumerator Skill_200001_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.animator[battleInfo.my_position].SetTrigger("isDefence");
        //skillText.text = "디펜스";
        yield return new WaitForSeconds(0.3f);

        effect["Defence"].transform.position = BattleManager.Inst.character[battleInfo.my_position].transform.position;
        effect["Defence"].GetComponent<DefenceEffect>().EffectAction();

        yield return new WaitForSeconds(3.0f);

        BattleManager.Inst.isAfterDelay = true;
    }
    #endregion

    #region Skill 200002
    // 배쉬 ( 적 1인에게 물리 공격력의 210% 물리피해를 입힙니다 )
    public void Skill_200002(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200002_Co(battleInfo));
    }

    public IEnumerator Skill_200002_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo);
        //skillText.text = "배쉬";
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.5f);
        TestSkillEffect(battleInfo.action_info_list[0].target_position);
    }
    #endregion

    #region Skill 200003
    // 패스트 어택 ( 스킬 발동 시 가장 먼저 적을 공격합니다 )
    public void Skill_200003(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200003_Co(battleInfo));
    }

    public IEnumerator Skill_200003_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo);
        //skillText.text = "패스트 어택";
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.5f);
        TestSkillEffect(battleInfo.action_info_list[0].target_position);
    }
    #endregion

    #region Skill 200004
    // 크리티컬 스트라이크 ( 적 1인에게 100% 확률로 치명타가 발생하는 물리 피해를 가합니다 )
    public void Skill_200004(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200004_Co(battleInfo));
    }

    public IEnumerator Skill_200004_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo);
        //skillText.text = "크리티컬 스트라이크";
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.5f);
        TestSkillEffect(battleInfo.action_info_list[0].target_position);
    }
    #endregion

    #region Skill 200005
    // 힐
    public void Skill_200005(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200005_Co(battleInfo));
        //skillText.text = "힐";
    }

    public IEnumerator Skill_200005_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.animator[battleInfo.my_position].SetTrigger("isHeal");

        yield return new WaitForSeconds(0.5f);

        DamageManager.Inst.DamageShow(battleInfo.action_info_list[0], true, (ELEMENT_TYPE)BattleManager.Inst.GetCharState(battleInfo.my_position).elementType);
        Instantiate(effect["Heal"], BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform.position +
            new Vector3(0, BattleManager.Inst.charInfo[battleInfo.action_info_list[0].target_position].Height * 0.5f, 0),
            Quaternion.identity,
            BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform);

        yield return new WaitForSeconds(2.0f);

        BattleManager.Inst.isAfterDelay = true;
    }
    #endregion

    #region Skill 200006
    // 매직 스트라이크 ( 적 1인에게 마법 공격력의 180% 마법피해를 줍니다)
    public void Skill_200006(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200006_Co(battleInfo));
    }

    public IEnumerator Skill_200006_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo);
        //skillText.text = "매직 스트라이크";
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.5f);
        TestSkillEffect(battleInfo.action_info_list[0].target_position);
    }
    #endregion

    #region Skill 200007
    // 멀티 샷 ( 랜덤한 적 2인에게 각각 물리 공격력의 125%의 물리 피해를 줍니다 )
    public void Skill_200007(characterActionData battleInfo)
    {
        // 아직 엄슴
        //skillText.text = "아직 안만듬";
        StartCoroutine(Skill_200007_Co(battleInfo));
    }

    IEnumerator Skill_200007_Co(characterActionData battleInfo)
    {
        Transform attacker;
        Transform target;

        attacker = BattleManager.Inst.character[battleInfo.my_position].transform;
        target = BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform;

        attacker.transform.LookAt(target);

        BattleManager.Inst.animator[battleInfo.my_position].SetTrigger("isDoubleAttack");

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Skill_200007_Co_Arrow(attacker, target));

        target = BattleManager.Inst.character[battleInfo.action_info_list[1].target_position].transform;

        attacker.transform.LookAt(target);

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(Skill_200007_Co_Arrow(attacker, target));

        BattleManager.Inst.animator[battleInfo.my_position].SetTrigger("isIdle");

        attacker.rotation = battleInfo.my_position < 10 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        yield return new WaitForSeconds(0.7f);

        DamageManager.Inst.DamageShow(battleInfo.action_info_list[0], false, (ELEMENT_TYPE)BattleManager.Inst.GetCharState(battleInfo.my_position).elementType);
        if (BattleManager.Inst.status[battleInfo.action_info_list[0].target_position].NowHp > 0)
            BattleManager.Inst.animator[battleInfo.action_info_list[0].target_position].SetTrigger("isHit");
        else
            BattleManager.Inst.animator[battleInfo.action_info_list[0].target_position].SetTrigger("isDie");

        yield return new WaitForSeconds(0.2f);

        DamageManager.Inst.DamageShow(battleInfo.action_info_list[1], false, (ELEMENT_TYPE)BattleManager.Inst.GetCharState(battleInfo.my_position).elementType);
        if (BattleManager.Inst.status[battleInfo.action_info_list[1].target_position].NowHp > 0)
            BattleManager.Inst.animator[battleInfo.action_info_list[1].target_position].SetTrigger("isHit");
        else
            BattleManager.Inst.animator[battleInfo.action_info_list[1].target_position].SetTrigger("isDie");

        yield return new WaitForSeconds(1.0f);

        BattleManager.Inst.isAfterDelay = true;
    }

    IEnumerator Skill_200007_Co_Arrow(Transform attaker, Transform target)
    {
        GameObject arrow = Instantiate(attaker.GetComponent<Bullet>().bullet, attaker.position + transform.position + new Vector3(0, 0.4f, 0), attaker.rotation);
        Vector3 startPos = arrow.transform.position;
        Vector3 endPos = target.position - (target.position - startPos).normalized * 0.2f;

        for (int i = 0; i < 50; i += BattleManager.Inst.TimeScale)
        {
            arrow.transform.position = Vector3.Lerp(startPos, endPos, i * 0.02f);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        Destroy(arrow);
    }
    #endregion

    #region Skill 200008
    // 가이디드 애로우 ( 적 1인에게 물리공격력의 150%의 물리 피해를 줍니다 )
    public void Skill_200008(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200008_Co(battleInfo));
    }

    public IEnumerator Skill_200008_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + BattleManager.Inst.charInfo[battleInfo.my_position].AttackAfterDelay);
        TestSkillEffect(battleInfo.action_info_list[0].target_position);
    }
    #endregion

    #region Skill 200009
    // 파이어 스트라이크 ( 적 1인에게 마법공격력의 220% 불속성 피해를 줍니다 )
    public void Skill_200009(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200009_Co(battleInfo));
    }

    public IEnumerator Skill_200009_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo, ELEMENT_TYPE.Fire);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.5f);
        Instantiate(effect["200009"], BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform.position +
            new Vector3(0, BattleManager.Inst.charInfo[battleInfo.action_info_list[0].target_position].Height * 0.5f, 0),
            Quaternion.identity,
            BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform);
    }
    #endregion

    #region Skill 200010
    // 프로즌 스트라이크 ( 적 1인에게 마법공격력의 220% 물속성 피해를 줍니다 )
    public void Skill_200010(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200010_Co(battleInfo));
    }

    public IEnumerator Skill_200010_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo, ELEMENT_TYPE.Water);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.3f);
        Instantiate(effect["200010"], BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform.position +
            new Vector3(0, BattleManager.Inst.charInfo[battleInfo.action_info_list[0].target_position].Height * 0.5f, 0),
            Quaternion.identity,
            BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform);
    }
    #endregion

    #region Skill 200011
    // 어스 스트라이크 ( 적 1인에게 마법공격력의 220% 땅속성 피해를 줍니다 )
    public void Skill_200011(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200011_Co(battleInfo));
    }

    public IEnumerator Skill_200011_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo, ELEMENT_TYPE.Earth);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.3f);
        Instantiate(effect["200011"], BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform.position +
            new Vector3(0, BattleManager.Inst.charInfo[battleInfo.action_info_list[0].target_position].Height * 0.5f, 0),
            Quaternion.identity,
            BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform);
    }
    #endregion

    #region Skill 200012
    // 윈드 스트라이크 ( 적 1인에게 마법공격력의 220% 바람속성 피해를 줍니다 )
    public void Skill_200012(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200012_Co(battleInfo));
    }

    public IEnumerator Skill_200012_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo, ELEMENT_TYPE.Wind);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.3f);
        Instantiate(effect["200012"], BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform.position +
            new Vector3(0, BattleManager.Inst.charInfo[battleInfo.action_info_list[0].target_position].Height * 0.5f, 0),
            Quaternion.identity,
            BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform);
    }
    #endregion

    #region Skill 200013
    // 홀리 스트라이크 ( 적 1인에게 마법공격력의 220% 성속성 피해를 줍니다 )
    public void Skill_200013(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200013_Co(battleInfo));
    }

    public IEnumerator Skill_200013_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo, ELEMENT_TYPE.Light);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.3f);
        Instantiate(effect["200013"], BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform.position +
            new Vector3(0, BattleManager.Inst.charInfo[battleInfo.action_info_list[0].target_position].Height * 0.5f, 0),
            Quaternion.identity,
            BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform);
    }
    #endregion

    #region Skill 200014
    // 데스 스트라이크 ( 적 1인에게 마법공격력의 220% 암속성 피해를 줍니다 )
    public void Skill_200014(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200014_Co(battleInfo));
    }

    public IEnumerator Skill_200014_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo, ELEMENT_TYPE.Dark);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.3f);
        Instantiate(effect["200014"], BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform.position +
            new Vector3(0, BattleManager.Inst.charInfo[battleInfo.action_info_list[0].target_position].Height * 0.5f, 0),
            Quaternion.identity,
            BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform);
    }
    #endregion

    #region Skill 200015
    // 더 밸런스 ( 적 1인에게 물리 공격력의 100% + 마법 공격력의 100%의 고정 피해를 줍니다. )
    public void Skill_200015(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200015_Co(battleInfo));
    }

    public IEnumerator Skill_200015_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay + 1.5f);
        TestSkillEffect(battleInfo.action_info_list[0].target_position);
    }
    #endregion

    #region Skill 200016
    // 아머 브레이크 ( 적 1인에게 물리 공격력의 130% 물리 피해를 주고 방어력 30% 감소 디버프를 겁니다. )
    public void Skill_200016(characterActionData battleInfo)
    {
        StartCoroutine(Skill_200016_Co(battleInfo));
    }

    public IEnumerator Skill_200016_Co(characterActionData battleInfo)
    {
        BattleManager.Inst.character[battleInfo.my_position].GetComponent<BasicAttack>().Attack(battleInfo);
        yield return new WaitForSeconds(BattleManager.Inst.charInfo[battleInfo.my_position].AttackDelay);
        Instantiate(effect["200016"], BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform.position +
           new Vector3(0, BattleManager.Inst.charInfo[battleInfo.action_info_list[0].target_position].Height * 0.5f, 0),
           Quaternion.identity,
           BattleManager.Inst.character[battleInfo.action_info_list[0].target_position].transform);
    }
    #endregion




    public void TestSkillEffect(int index)
    {
        Instantiate(effect["Skill"], BattleManager.Inst.character[index].transform.position +
            new Vector3(0, BattleManager.Inst.charInfo[index].Height * 0.5f, 0),
            Quaternion.identity,
            BattleManager.Inst.character[index].transform);
    }
}
