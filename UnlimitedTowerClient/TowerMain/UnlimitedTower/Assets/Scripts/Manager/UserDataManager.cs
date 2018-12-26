using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataManager : MonoSingleton<UserDataManager> {

    // TODO : userLoginFlag : 배틀씬에서 단독으로 실행할 때 쓰는 용도
    public bool userLoginFlag;
    public bool createPlayerFlag;

    public UserInfo userInfo = new UserInfo();
    public Dictionary<int, Servant> servantDic = new Dictionary<int, Servant>();
    public Dictionary<int, Monster> monsterDic = new Dictionary<int, Monster>();
    public Dictionary<int, Item> itemDic = new Dictionary<int, Item>();
    public Dictionary<int, Party> partyDic = new Dictionary<int, Party>();

    public void Awake()
    {

    }

    private void LoadCharImage(string imageFath, int deckNum, GameObject original)
    {
        GameObject deck = FormationManager.Inst.Decks[deckNum];
        Sprite sprite = Resources.Load<Sprite>(imageFath);

        if (deck.GetComponent<FormationDeck>())
        {
            deck.GetComponent<FormationDeck>().LinkedChar = original;
            deck.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

            FormationManager.Inst.DeckTexts[deckNum].SetActive(false);
            if(original == null)
            {
                Debug.Log("히로 로딩 완료");
            }
        }
        else
        {
            Debug.Log("Do not exist FormatiionDeck Component");
        }    
    }
    
    public void SetUserLoginFlag(bool flag)
    {
        userLoginFlag = flag;
    }

    public void SetCreatePlayerFlag(bool flag)
    {
        createPlayerFlag = flag;
    }

    public void LoadCharData(GameObject charContentList, ref Dictionary<int, Character> charDic, CHAR_TYPE charType)
    {
        string imageFath = null;
        if(charType == CHAR_TYPE.SERVANT)
        {
            imageFath = "UI/CharaterImage/";
        }
        else
        {
            imageFath = "UI/MonsterImage/";
        }

        // 캐릭터 개수만큼 캐릭터 목록을 다시 불러온다.
        foreach (KeyValuePair<int, Character> dic in charDic)
        {          
            var instance = Instantiate(Resources.Load("Prefabs/CharContent") as GameObject);
            // 이걸 이미지로 검사해야하는가?
            if (instance.transform.GetChild(0).GetComponent<Image>())
            {         
                instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(imageFath + charDic[dic.Key].Name);
                instance.transform.SetParent(charContentList.transform.transform);
                instance.GetComponent<CharContent>().charDicKey = dic.Key;
                if (charType == CHAR_TYPE.SERVANT)
                {
                    instance.GetComponent<CharContent>().charType = CHAR_TYPE.SERVANT;
                }
                else
                {
                    instance.GetComponent<CharContent>().charType = CHAR_TYPE.MONSTER;
                }

                if (charDic[dic.Key].OnFormation)
                {
                    Color color = instance.transform.GetChild(0).GetComponent<Image>().color;
                    color.r = color.g = color.b = 0.35f;
                    instance.transform.GetChild(0).GetComponent<Image>().color = color;
                    instance.transform.GetChild(1).gameObject.SetActive(true);

                    //  포메이션 세팅.
                    if (charDic[dic.Key].FormationIndex != -1)
                    {
                        int deckNum = charDic[dic.Key].FormationIndex;
                        LoadCharImage(imageFath + charDic[dic.Key].Name, deckNum, instance);                 
                    }
                }
            }
        }
    }

    public void AddServantImage(Servant getServant)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/CharContent") as GameObject);

        if (instance.transform.GetChild(0))
        {
            instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/CharaterImage/" + getServant.name);
            instance.GetComponent<CharContent>().charDicKey = getServant.index;
            instance.transform.SetParent(CharacterListManager.Inst.ServantContentList.transform.transform);
            instance.GetComponent<CharContent>().charType = CHAR_TYPE.SERVANT;

        }
    }

    public void AddMonsterImage(Monster getMonster)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/CharContent") as GameObject);

        if (instance.transform.GetChild(0))
        {
            instance.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/MonsterImage/" + getMonster.name);
            instance.GetComponent<CharContent>().charDicKey = getMonster.index;
            instance.transform.SetParent(CharacterListManager.Inst.MonsterContentList.transform.transform);
            instance.GetComponent<CharContent>().charType = CHAR_TYPE.MONSTER;
        }    
    }

    public void SetUserInfo(UserInfo getUserInfo)
    {
        userInfo = getUserInfo;
    }

    public void SetServantDic(Dictionary<int, Servant> getServantDic)
    {
        servantDic = getServantDic;
    }

    public void SetMonsterDic(Dictionary<int, Monster> getMonsterDic)
    {
        monsterDic = getMonsterDic;
    }

    public void SetItemDic(Dictionary<int, Item> getItemDic)
    {
        itemDic = getItemDic;
    }

    public void SetPartyDic(Dictionary<int, Party> getPartyDic)
    {
        partyDic = getPartyDic;
    }

    public void SetServant(Servant getServant)
    {
        if (servantDic.ContainsKey(getServant.index) == false)
        {
            servantDic.Add(getServant.index, getServant);
        }
        else
        {
            Debug.Log("Invalid SetServant Info : " + getServant.index);
        }
    }

    public void SetMonster(Monster getMonster)
    {
        if (monsterDic.ContainsKey(getMonster.index) == false)
        {
            monsterDic.Add(getMonster.index, getMonster);
        }
        else
        {
            Debug.Log("Invalid SetMonster Info : " + getMonster.index);
        }
    }

    public void SetItem(Item getItem)
    {
        if (itemDic.ContainsKey(getItem.index) == false)
        {
            itemDic.Add(getItem.index, getItem);
        }
        else
        {
            Debug.Log("Invalid SetItem Info : " + getItem.index);
        }
    }

    public void SetSceneState(SCENE_STATE state)
    {
        userInfo.sceneState = state;
    }

    public int GetServantCount()
    {
        return servantDic.Count;
    }
}



