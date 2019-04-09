using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoSingleton<DamageManager>
{
    public TextPool textPool;
    
    private int Index = 0;
    private Camera camera_;
    private readonly Color RedColor = new Color(1, 0, 0, 1);
    private readonly Color YellowColor = new Color(1, 1, 0, 1);
    private readonly Color GreenColor = new Color(0, 1, 0, 1);
    private readonly Color OrangeColor = new Color(1, 0.55f, 0, 1);

    public enum Attribute
    {
        DEFAULT,
        CRITICAL,
        HEAL
    };

    private void Start()
    {
        textPool = transform.GetChild(0).GetComponent<TextPool>();

        camera_ = Camera.main;
    }

    public void DamageAciton(actionInfo attackInfo, bool isHeal)
    {
        DamageShow(attackInfo, isHeal);
    }

    public void DamageShow(actionInfo attackInfo, bool isHeal) // 맞은 대상의 인덱스와 플레이어 여부, 데미지, 크리티컬, 힐 여부
    {
        var temp = textPool.ObjectSpawn();

        if (!attackInfo.avoid)
        {
            temp.text.text = attackInfo.damage.ToString();
            if (isHeal)
            {
                temp.text.color = GreenColor;
                BattleManager.Inst.NowHp[attackInfo.target_position] += attackInfo.damage;
            }
            else
            {
                if (attackInfo.critical)
                    temp.text.color = YellowColor;
                else
                    temp.text.color = RedColor;

                BattleManager.Inst.NowHp[attackInfo.target_position] -= attackInfo.damage;
            }
        }
        else
        {
            temp.text.text = "Miss";
            temp.text.color = OrangeColor;
        }
        temp.obj.transform.position =
              Camera.main.WorldToScreenPoint(BattleManager.Inst.character[attackInfo.target_position].transform.position +
              new Vector3(0, BattleManager.Inst.character[attackInfo.target_position].GetComponent<CharInfo>().Height, 0));

        if (BattleManager.Inst.NowHp[attackInfo.target_position] < 0)
            BattleManager.Inst.NowHp[attackInfo.target_position] = 0;
        if (BattleManager.Inst.NowHp[attackInfo.target_position] > BattleManager.Inst.MaxHp[attackInfo.target_position])
            BattleManager.Inst.NowHp[attackInfo.target_position] = BattleManager.Inst.MaxHp[attackInfo.target_position];
        
        if (!isHeal)
        {
            if (!attackInfo.critical)
                StartCoroutine(NotCriticalAttackEffect());
            else
                StartCoroutine(CriticalAttackEffect());
        }
    }

    IEnumerator NotCriticalAttackEffect()
    {
        Vector2 temp = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        temp = temp.normalized;
    
        for (int i = 0; i < 5; i++)
        {
            camera_.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            camera_.transform.Translate(-temp.x * 0.03f, -temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 5; i++)
        {
            camera_.transform.Translate(temp.x * 0.03f, temp.y * 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
    
    IEnumerator CriticalAttackEffect()
    {
        Vector2 temp = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        temp = temp.normalized;
    
        for (int i = 0; i < 5; i++)
        {
            camera_.transform.Translate(temp.x * 0.09f, temp.y * 0.09f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            camera_.transform.Translate(-temp.x * 0.09f, -temp.y * 0.09f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 5; i++)
        {
            camera_.transform.Translate(temp.x * 0.09f, temp.y * 0.09f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
