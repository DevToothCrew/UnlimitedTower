using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public void Skill_303(battleActionInfo attackInfo)
    {
        StartCoroutine(Skill_303_Co(attackInfo));
    }

    public IEnumerator Skill_303_Co(battleActionInfo attackInfo)
    {
        yield return new WaitForSeconds(2.0f);

        foreach (actionInfo actionInfo in attackInfo.battle_action_list)
            DamageTextSystem.Inst.DamageTextAction(actionInfo);

        yield return new WaitForSeconds(2.0f);
    }

    public void Skill_304(battleActionInfo attackInfo)
    {
        StartCoroutine(Skill_304_Co(attackInfo));
    }

    public IEnumerator Skill_304_Co(battleActionInfo attackInfo)
    {
        yield return new WaitForSeconds(2.0f);

        foreach (actionInfo actionInfo in attackInfo.battle_action_list)
            DamageTextSystem.Inst.HealTextAction(actionInfo);

        yield return new WaitForSeconds(2.0f);
    }
}