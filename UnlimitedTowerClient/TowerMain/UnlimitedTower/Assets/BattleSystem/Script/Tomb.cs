using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomb : MonoBehaviour {
    private Material material;

	void Start () {
        transform.Translate(0, 10, 0);
        material = GetComponent<Renderer>().material;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(4.0f);

        for (int i = 0; i < 25; i += BattleSystem.Inst.TimeScale)
        {
            transform.parent.GetChild(0).Translate(0, -0.1f * BattleSystem.Inst.TimeScale, 0);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        for (int i = 0; i < 25; i += BattleSystem.Inst.TimeScale)
        {
            transform.Translate(0, BattleSystem.Inst.TimeScale * -0.4f, 0);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}