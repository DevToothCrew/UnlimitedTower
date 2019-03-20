using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public void Skill_302(battleActionInfo attackInfo)
    {
        StartCoroutine(Skill_302_Co(attackInfo));
    }

    public IEnumerator Skill_302_Co(battleActionInfo attackInfo)
    {
        yield return new WaitForSeconds(2.0f);

        DamageTextSystem.Inst.DamageTextAction(attackInfo.battle_action_list[0]);
        BattleSystem.Inst.characterControl[attackInfo.battle_action_list[0].target_position].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");

        yield return new WaitForSeconds(0.2f);

        DamageTextSystem.Inst.DamageTextAction(attackInfo.battle_action_list[1]);

        yield return new WaitForSeconds(1.0f);

        BattleSystem.Inst.isAttackAfterDelay = true;
    }

    public void Skill_303(battleActionInfo attackInfo)
    {
        StartCoroutine(Skill_303_Co(attackInfo));
    }

    public IEnumerator Skill_303_Co(battleActionInfo attackInfo)
    {
        yield return new WaitForSeconds(2.0f);

        foreach (actionInfo actionInfo in attackInfo.battle_action_list)
        {
            DamageTextSystem.Inst.DamageTextAction(actionInfo);
            BattleSystem.Inst.characterControl[attackInfo.battle_action_list[0].target_position].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isHit");
        }
        yield return new WaitForSeconds(1.0f);

        BattleSystem.Inst.isAttackAfterDelay = true;
    }

    public void Skill_304(battleActionInfo attackInfo)
    {
        StartCoroutine(Skill_304_Co(attackInfo));
    }

    public IEnumerator Skill_304_Co(battleActionInfo attackInfo)
    {
        yield return new WaitForSeconds(2.0f);

        foreach (actionInfo actionInfo in attackInfo.battle_action_list)
        {
            DamageTextSystem.Inst.HealTextAction(actionInfo);
        }
        yield return new WaitForSeconds(1.0f);

        BattleSystem.Inst.isAttackAfterDelay = true;
    }
}