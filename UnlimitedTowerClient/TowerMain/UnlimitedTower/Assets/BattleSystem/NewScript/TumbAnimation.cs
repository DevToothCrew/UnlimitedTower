using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbAnimation : MonoBehaviour
{
    public GameObject tumb;

    public void DieTumb(int index)
    {
        StartCoroutine(Tumb(index));
    }

    public IEnumerator Tumb(int index)
    {
        yield return new WaitForSeconds(4.0f);

        GameObject temp = Instantiate(tumb, BattleManager.Inst.character[index].transform.position + new Vector3(0, 10, 0), BattleManager.Inst.character[index].transform.rotation);
        Renderer[] model = new Renderer[BattleManager.Inst.character[index].transform.childCount - 2];
        model = BattleManager.Inst.character[index].transform.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < model.Length; i++)
        {
            model[i].material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        }

        for (int i = 0; i < 25; i++)
        {
            foreach (Renderer mater in model)
            {
                if (mater?.material != null)
                {
                    mater.material.color = new Color(1, 1, 1, 1 - 0.04f * i);
                    if (mater?.material?.color.a < 0)
                        mater.material.color = new Color(1, 1, 1, 0);
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
        BattleManager.Inst.character[index].SetActive(false);

        for (int i = 0; i < 25; i++)
        {
            temp.transform.Translate(0, -0.4f, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
