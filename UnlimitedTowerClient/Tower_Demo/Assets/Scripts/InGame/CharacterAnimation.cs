using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour {

    private bool flag = false;



    IEnumerator IdleAni()
    {

        // yield return new WaitForSeconds(1.0f);
        //while(true)
        //{
        //    Debug.Log("대기 애니메이션 실행..");
        //    yield return null;
        //}

       // yield return null;
        Debug.Log("대기 애니메이션 실행..");
        yield break;

    }


    IEnumerator AttackAni()
    {
        while(!flag)
        {
            Debug.Log("공격 애니메이션 실행");
            yield return new WaitForSeconds(2.0f);
            flag = true;
        }
        yield return StartCoroutine("IdleAni");
        
    }

    void Start()
    {
      


    }

    void Update()
    {
        
    }
}
