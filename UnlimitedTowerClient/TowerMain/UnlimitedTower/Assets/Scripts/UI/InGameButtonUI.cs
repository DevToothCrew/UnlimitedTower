using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO : if deleted
public class InGameButtonUI : MonoBehaviour {

    // Use this for initialization

    public float SPEED = 2.0f;


    bool AnimationSpeedDouble = false;
    public void OnClickAnimationSpeedDouble()
    {
        if (!AnimationSpeedDouble)
        {
            Debug.Log("홠성화");
            AnimationSpeedDouble = true;
            transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Bold;
            DEFINE.ANIMATION_SPEED = SPEED*2;
            for(int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
            {
                BattleManager.Inst.enemyObjects[i].GetComponent<Animator>().speed = DEFINE.ANIMATION_SPEED;
                BattleManager.Inst.playerObjects[i].GetComponent<Animator>().speed = DEFINE.ANIMATION_SPEED;
            }
          
        }
        else
        {
            Debug.Log("비홠성화");
            AnimationSpeedDouble = false;
            transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Normal;
            DEFINE.ANIMATION_SPEED = SPEED;
            for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
            {
                BattleManager.Inst.enemyObjects[i].GetComponent<Animator>().speed = DEFINE.ANIMATION_SPEED;
                BattleManager.Inst.playerObjects[i].GetComponent<Animator>().speed = DEFINE.ANIMATION_SPEED;
            }
        }
    }
    public void OnClickBattleStart()
    {
        if(BattleManager.Inst.BattleState != BATTLE_STATE.BATTLE  )
        {
            BattleManager.Inst.StartBattle();
            transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Bold;
            BattleManager.Inst.BattleState = BATTLE_STATE.BATTLE;
        }
    }


}
