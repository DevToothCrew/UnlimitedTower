using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListManager : MonoSingleton<CharacterListManager>
{

    public GameObject CharacterListScroll;
    public GameObject ServantContentList;
    public GameObject MonsterContentList;	





    public void ShowAllCharacterImage()
    {
        for (int i = 0; i < ServantContentList.transform.childCount; i++)
        {
            GameObject characterObject = ServantContentList.transform.GetChild(i).gameObject;

            if (characterObject.GetComponent<CharContent>())
            {
                ServantContentList.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }







    public void ShowServantImage()
    {
        for (int i = 0; i < ServantContentList.transform.childCount; i++)
        {
            GameObject servantObject = ServantContentList.transform.GetChild(i).gameObject;

            if (servantObject.GetComponent<CharContent>())
            {

                if (servantObject.GetComponent<CharContent>().charType == CHAR_TYPE.SERVANT)
                {
                    ServantContentList.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    ServantContentList.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public void ShowMonsterImage()
    {
        for (int i = 0; i < ServantContentList.transform.childCount; i++)
        {
            GameObject servantObject = ServantContentList.transform.GetChild(i).gameObject;

            if (servantObject.GetComponent<CharContent>())
            {

                if (servantObject.GetComponent<CharContent>().charType == CHAR_TYPE.MONSTER)
                {
                    ServantContentList.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    ServantContentList.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
