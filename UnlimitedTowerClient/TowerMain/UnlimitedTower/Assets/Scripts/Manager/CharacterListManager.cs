using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListManager : MonoSingleton<CharacterListManager>
{

    public GameObject CharacterListScroll;
    public GameObject CharacterContentList;





    public void ShowAllCharacterImage()
    {
        for (int i = 0; i < CharacterContentList.transform.childCount; i++)
        {
            GameObject characterObject = CharacterContentList.transform.GetChild(i).gameObject;

            if (characterObject.GetComponent<CharContent>())
            {
                CharacterContentList.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }







    public void ShowServantImage()
    {
        for (int i = 0; i < CharacterContentList.transform.childCount; i++)
        {
            GameObject servantObject = CharacterContentList.transform.GetChild(i).gameObject;

            if (servantObject.GetComponent<CharContent>())
            {

                if (servantObject.GetComponent<CharContent>().charType == CHAR_TYPE.SERVANT)
                {
                    CharacterContentList.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    CharacterContentList.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public void ShowMonsterImage()
    {
        for (int i = 0; i < CharacterContentList.transform.childCount; i++)
        {
            GameObject servantObject = CharacterContentList.transform.GetChild(i).gameObject;

            if (servantObject.GetComponent<CharContent>())
            {

                if (servantObject.GetComponent<CharContent>().charType == CHAR_TYPE.MONSTER)
                {
                    CharacterContentList.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    CharacterContentList.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
