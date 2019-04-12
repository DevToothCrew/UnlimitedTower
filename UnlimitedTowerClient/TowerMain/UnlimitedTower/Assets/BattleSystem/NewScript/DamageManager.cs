using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoSingleton<DamageManager>
{
    public TextPool textPool;

    private Camera camera_;

    private readonly Color2[] TextColor = {
        new Color2(new Color(1.0f, 1.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f)),
        new Color2(new Color(1.0f, 0.65f, 0.68f), new Color(1.0f, 0.0f, 0.0f)),
        new Color2(new Color(0.0f, 0.67f, 1.0f), new Color(0.0f, 0.21f, 1.0f)),
        new Color2(new Color(0.8f, 0.65f, 0.37f), new Color(0.53f, 0.42f, 0.0f)),
        new Color2(new Color(0.56f, 1.0f, 0.49f), new Color(0.19f, 0.69f, 0.0f)),
        new Color2(new Color(1.0f, 0.98f, 0.22f), new Color(1.0f, 1.0f, 1.0f)),
        new Color2(new Color(0.66f, 0.21f, 1.0f), new Color(0.29f, 0.0f, 0.39f))
    };

    private readonly Color[] HealColor = { new Color(0.0f, 1.0f, 0.0f), new Color(0.0f, 0.0f, 0.0f) };
    private readonly Color[] CriticalColor = { new Color(1.0f, 1.0f, 0.0f), new Color(0.0f, 0.0f, 0.0f) };
    private readonly Color[] MissColor = { new Color(1.0f, 1.0f, 0.0f), new Color(0.0f, 0.0f, 0.0f) };

    public class Color2
    {
        public Color textColor;
        public Color outColor;

        public Color2(Color textColor, Color outColor)
        {
            this.textColor = textColor;
            this.outColor = outColor;
        }
    }

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

    public void DamageShow(actionInfo attackInfo, bool isHeal, ELEMENT_TYPE type) // 맞은 대상의 인덱스와 플레이어 여부, 데미지, 크리티컬, 힐 여부
    {
        var damageText = textPool.ObjectSpawn();

        if (!attackInfo.avoid)
        {
            if (isHeal)
            {
                damageText.text.text = attackInfo.damage.ToString();
                damageText.text.color = HealColor[0];
                damageText.outLine.effectColor = HealColor[1];
                BattleManager.Inst.NowHp[attackInfo.target_position] += attackInfo.damage;
            }
            else
            {
                if (type == ELEMENT_TYPE.Non)
                {
                    if (attackInfo.critical)
                    {
                        damageText.text.color = CriticalColor[0];
                        damageText.outLine.effectColor = CriticalColor[1];
                        damageText.text.text = "Critical\n" + attackInfo.damage.ToString();
                    }
                    else
                    {
                        damageText.text.text = attackInfo.damage.ToString();
                        damageText.text.color = TextColor[0].textColor;
                        damageText.outLine.effectColor = TextColor[0].outColor;
                    }
                }
                else
                {
                    if (type == ELEMENT_TYPE.Fire && BattleManager.Inst.GetCharState(attackInfo.target_position).elementType == 4 ||
                            type == ELEMENT_TYPE.Wind && BattleManager.Inst.GetCharState(attackInfo.target_position).elementType == 3 ||
                            type == ELEMENT_TYPE.Earth && BattleManager.Inst.GetCharState(attackInfo.target_position).elementType == 2 ||
                            type == ELEMENT_TYPE.Water && BattleManager.Inst.GetCharState(attackInfo.target_position).elementType == 1 ||
                            type == ELEMENT_TYPE.Light && BattleManager.Inst.GetCharState(attackInfo.target_position).elementType == 6 ||
                            type == ELEMENT_TYPE.Dark && BattleManager.Inst.GetCharState(attackInfo.target_position).elementType == 5)
                    {
                        damageText.text.color = TextColor[(int)type].textColor;
                        damageText.outLine.effectColor = TextColor[(int)type].outColor;

                        if (attackInfo.critical)
                            damageText.text.text = "Critical\n" + attackInfo.damage.ToString();
                        else
                            damageText.text.text = "Weak\n" + attackInfo.damage.ToString();
                    }
                    else
                    {
                        damageText.text.color = TextColor[0].textColor;
                        damageText.outLine.effectColor = TextColor[0].outColor;

                        if (attackInfo.critical)
                            damageText.text.text = "Critical\n" + attackInfo.damage.ToString();
                        else
                            damageText.text.text = attackInfo.damage.ToString();
                    }
                }
                BattleManager.Inst.NowHp[attackInfo.target_position] -= attackInfo.damage;
            }
        }
        else
        {
            damageText.text.text = "Miss";
            damageText.text.color = MissColor[0];
            damageText.outLine.effectColor = MissColor[1];
        }
        damageText.obj.transform.position =
              Camera.main.WorldToScreenPoint(BattleManager.Inst.character[attackInfo.target_position].transform.position +
              new Vector3(0, BattleManager.Inst.character[attackInfo.target_position].GetComponent<CharInfo>().Height, 0));

        if (BattleManager.Inst.NowHp[attackInfo.target_position] < 0)
            BattleManager.Inst.NowHp[attackInfo.target_position] = 0;
        if (BattleManager.Inst.NowHp[attackInfo.target_position] > BattleManager.Inst.status[attackInfo.target_position].maxHp)
            BattleManager.Inst.NowHp[attackInfo.target_position] = BattleManager.Inst.status[attackInfo.target_position].maxHp;

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
