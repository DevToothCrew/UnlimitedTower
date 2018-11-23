using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO : if deleted
public class InGameButtonUI : MonoBehaviour {
    
    [SerializeField]
    private float animationSpeed = 2.0f;

    private bool animationSpeedDouble = false;

    public void OnClickAnimationSpeedDouble()
    {
     
        if (!animationSpeedDouble)
        {
            Debug.Log("활성화");
            animationSpeedDouble = true;
            transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Bold;
            DEFINE.ANIMATION_SPEED = animationSpeed * 2;
            for(int i=0; i<DEFINE.PARTY_MAX_NUM; i++)
            {
                if(BattleManager.Inst.enemyObjects[i])
                {
                    BattleManager.Inst.enemyObjects[i].GetComponent<Animator>().speed = DEFINE.ANIMATION_SPEED;
                }

                if (BattleManager.Inst.playerObjects[i])
                {
                    BattleManager.Inst.playerObjects[i].GetComponent<Animator>().speed = DEFINE.ANIMATION_SPEED;
                }

              
               
            }
          
        }
        else
        {
            Debug.Log("비활성화");
            animationSpeedDouble = false;
            transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Normal;
            DEFINE.ANIMATION_SPEED = animationSpeed;
            for (int i = 0; i < DEFINE.PARTY_MAX_NUM; i++)
            {
                if (BattleManager.Inst.enemyObjects[i])
                {
                    BattleManager.Inst.enemyObjects[i].GetComponent<Animator>().speed = DEFINE.ANIMATION_SPEED;
                }

                if (BattleManager.Inst.playerObjects[i])
                {
                    BattleManager.Inst.playerObjects[i].GetComponent<Animator>().speed = DEFINE.ANIMATION_SPEED;
                }
            }
        }
    }

    public void OnClickBattleStart()
    {
        if(BattleManager.Inst.BattleState != BATTLE_STATE.BATTLE  )
        {
            //BattleManager.Inst.SetTurnSpeed();
            BattleManager.Inst.StartBattle();
            transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Bold;
            BattleManager.Inst.BattleState = BATTLE_STATE.BATTLE;
        }
    }

}
