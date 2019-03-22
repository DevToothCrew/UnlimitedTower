using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    #region Skill 200005
    // 힐
    public void Skill_200005(battleActionInfo battleInfo)
    {
        StartCoroutine(Skill_200005_Co(battleInfo));
    }

    public IEnumerator Skill_200005_Co(battleActionInfo battleInfo)
    {
        yield return new WaitForSeconds(2.0f);

        DamageManager.Inst.DamageAciton(battleInfo.battle_action_list[0], true);
        
        yield return new WaitForSeconds(2.0f);

        BattleManager.Inst.isAfterDelay = true;
    }
    #endregion

}
