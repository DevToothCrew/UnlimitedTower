using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharContentList : MonoBehaviour
{

    void LoadCharContentList()
    {
        //
        // 캐릭터 개수만큼 캐릭터 목록을 다시 불러온다.
        //  
        foreach (KeyValuePair<int, Character> dic in UserDataManager.Inst.characterDic)
        {
            var instance = Instantiate(Resources.Load("Prefabs/CharElement") as GameObject);
            if (instance.GetComponent<Image>())
            {
                instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + UserDataManager.Inst.characterDic[dic.Key].Name);
                instance.transform.SetParent(gameObject.transform.transform);
                instance.GetComponent<CharContent>().CharDicKey = dic.Key;

                // 이미 배치된 캐릭터라면 캐릭터 리스트의 이미지를 검게한다.
                if (UserDataManager.Inst.characterDic[dic.Key].OnFormation)
                {
                    Color color = instance.GetComponent<Image>().color;
                    color.r = color.g = color.b = 0.35f;
                    instance.GetComponent<Image>().color = color;
                }
            }
        }
    }

	
}
