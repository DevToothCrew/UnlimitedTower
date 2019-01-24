using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErdManager : MonoBehaviour {

    // singleton
    public static ErdManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        // 비어있는데이터, or Null인것 확인해서 띄워주기

        if (levelEntityTable == null)
        {
            Debug.Log("levelEntityTable가 null입니다. Erdmanager인스펙터에 levelEntityTable.asset을 등록해주세요");
        }
        if (levelEntityTable.param.Count == 0)
        {
            Debug.Log("levelEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (ServantBodyEntityTable == null)
        {
            Debug.Log("ServantBodyEntityTable가 null입니다. Erdmanager인스펙터에 ServantBodyEntityTable.asset을 등록해주세요");
        }
        if (ServantBodyEntityTable.param.Count == 0)
        {
            Debug.Log("ServantBodyEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (ServantIconEntityTable == null)
        {
            Debug.Log("ServantIconEntityTable가 null입니다. Erdmanager인스펙터에 ServantIconEntityTable.asset을 등록해주세요");
        }
        if (ServantIconEntityTable.param.Count == 0)
        {
            Debug.Log("ServantIconEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (ServantJobEntityTable == null)
        {
            Debug.Log("ServantJobEntityTable가 null입니다. Erdmanager인스펙터에 ServantJobEntityTable.asset을 등록해주세요");
        }
        if (ServantJobEntityTable.param.Count == 0)
        {
            Debug.Log("ServantJobEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (mountitemEntityTable == null)
        {
            Debug.Log("mountitemEntityTable가 null입니다. Erdmanager인스펙터에 mountitemEntityTable.asset을 등록해주세요");
        }
        if (mountitemEntityTable.param.Count == 0)
        {
            Debug.Log("mountitemEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (mountitemEnforceEntityTable == null)
        {
            Debug.Log("mountitemEnforceEntityTable가 null입니다. Erdmanager인스펙터에 mountitemEnforceEntityTable.asset을 등록해주세요");
        }
        if (mountitemEnforceEntityTable.param.Count == 0)
        {
            Debug.Log("mountitemEnforceEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (mountitemTeerEntityTable == null)
        {
            Debug.Log("mountitemTeerEntityTable가 null입니다. Erdmanager인스펙터에 mountitemTeerEntityTable.asset을 등록해주세요");
        }
        if (mountitemTeerEntityTable.param.Count == 0)
        {
            Debug.Log("mountitemTeerEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (mountitemGradeEntityTable == null)
        {
            Debug.Log("mountitemGradeEntityTable가 null입니다. Erdmanager인스펙터에 mountitemGradeEntityTable.asset을 등록해주세요");
        }
        if (mountitemGradeEntityTable.param.Count == 0)
        {
            Debug.Log("mountitemGradeEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (MountItemStatEntityTable == null)
        {
            Debug.Log("MountItemStatEntityTable가 null입니다. Erdmanager인스펙터에 MountItemStatEntityTable.asset을 등록해주세요");
        }
        if (MountItemStatEntityTable.param.Count == 0)
        {
            Debug.Log("MountItemStatEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (EtcItemEntityTable == null)
        {
            Debug.Log("EtcItemEntityTable가 null입니다. Erdmanager인스펙터에 EtcItemEntityTable.asset을 등록해주세요");
        }
        if (EtcItemEntityTable.param.Count == 0)
        {
            Debug.Log("EtcItemEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (MonsterEntityTable == null)
        {
            Debug.Log("MonsterEntityTable가 null입니다. Erdmanager인스펙터에 MonsterEntityTable.asset을 등록해주세요");
        }
        if (MonsterEntityTable.param.Count == 0)
        {
            Debug.Log("MonsterEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (MonsterEnforceEntityTable == null)
        {
            Debug.Log("MonsterEnforceEntityTable가 null입니다. Erdmanager인스펙터에 MonsterEnforceEntityTable.asset을 등록해주세요");
        }
        if (MonsterEnforceEntityTable.param.Count == 0)
        {
            Debug.Log("MonsterEnforceEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (MonsterGradeEntityTable == null)
        {
            Debug.Log("MonsterGradeEntityTable가 null입니다. Erdmanager인스펙터에 MonsterGradeEntityTable.asset을 등록해주세요");
        }
        if (MonsterGradeEntityTable.param.Count == 0)
        {
            Debug.Log("MonsterGradeEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (StatEntityTable == null)
        {
            Debug.Log("StatEntityTable가 null입니다. Erdmanager인스펙터에 StatEntityTable.asset을 등록해주세요");
        }
        if (StatEntityTable.param.Count == 0)
        {
            Debug.Log("StatEntityTable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }

        if (formindextranstable == null)
        {
            Debug.Log("formindextranstable가 null입니다. Erdmanager인스펙터에 formindextranstable.asset을 등록해주세요");
        }
        if (formindextranstable.param.Count == 0)
        {
            Debug.Log("formindextranstable가 비어있습니다. erdexcels파일안에 엑셀들을 우클릭해서 reimpot를 실행해주세요");
        }
        
    }

    
    

    public levelEntity levelEntityTable;    // 레벨별 정보
    public levelEntity.Param getlevelEntityTable_nullPossible(int level)
    {
        return levelEntityTable.param.Find((rowdata) => { return rowdata.level == level; });
    }
    
    

    // Servant
    [Space(10)]
    [Header("SERVANT")]
    public ServantBodyEntity ServantBodyEntityTable;    // 
    public ServantBodyEntity.Param getServantBodyEntityTable_nullPossible(int bodyNum)
    {
        return ServantBodyEntityTable.param.Find((rowdata) => { return rowdata.bodyNum == bodyNum; });
    }
    public GameObject[] ServantBodyPrefabs;
    
    public ServantIconEntity ServantIconEntityTable;    // 
    public ServantIconEntity.Param getServantIconEntityTable_nullPossible(int charNum,int jobNum )
    {
        ServantIconEntity.Param param = ServantIconEntityTable.param.Find((rowdata) => { return rowdata.charNum == charNum && rowdata.jobNum == jobNum; });
        if (param == null)
        {
            Debug.Log(charNum + "/" + jobNum);
        }
        return param;
    }
    public Sprite GetServantIconSprite(bool islegend, int charNum, int jobNum)
    {
        if (islegend)
        {
            charNum = getServantJobEntityTable_nullPossible(jobNum).Legend_CharNum;
            return ServantbodySprite[getServantIconEntityTable_nullPossible(charNum, jobNum).iconNum];
        }
        else
        {
            return ServantbodySprite[getServantIconEntityTable_nullPossible(charNum, jobNum).iconNum];
        }
        
    }
    [SerializeField]Sprite[] ServantbodySprite;

    public ServantJobEntity ServantJobEntityTable;    // 
    public ServantJobEntity.Param getServantJobEntityTable_nullPossible(int jobNum)
    {
        return ServantJobEntityTable.param.Find((rowdata) => { return rowdata.jobNum == jobNum; });
    }
    public Sprite[] JobIcons;




    // ITEM
    [Space(10)]
    [Header("ITEM")]

    public MountItemEntity mountitemEntityTable;    // 
    public MountItemEntity.Param getmountitemEntityTable_nullPossible(int num)
    {
        return mountitemEntityTable.param.Find((rowdata) => { return rowdata.mountItemNum == num; });
    }
    public Sprite[] MountitemSprite;

    public MountItemEnforceEntity mountitemEnforceEntityTable;    // 
    public MountItemEnforceEntity.Param getmountitemEnforceEntityTable_nullPossible(int num)
    {
        return mountitemEnforceEntityTable.param.Find((rowdata) => { return rowdata.enforceCount == num; });
    }

    public MountItemTeerEntity mountitemTeerEntityTable;    // 
    public MountItemTeerEntity.Param getmountitemTeerEntityTable_nullPossible(int num)
    {
        return mountitemTeerEntityTable.param.Find((rowdata) => { return rowdata.teerNum == num; });
    }

    public MountItemGradeEntity mountitemGradeEntityTable;    // 
    public MountItemGradeEntity.Param getmountitemGradeEntityTable_nullPossible(int num)
    {
        return mountitemGradeEntityTable.param.Find((rowdata) => { return rowdata.gradeNum == num; });
    }

    public MountItemStatEntity MountItemStatEntityTable;    // 
    public List<MountItemStatEntity.Param> getMountItemStatEntityTable_nullPossible(int num)
    {
        return MountItemStatEntityTable.param.FindAll((rowdata) => { return rowdata.mountItemNum == num; });
    }

    public EtcItemEntity EtcItemEntityTable;    // 
    public EtcItemEntity.Param getEtcItemEntityTable_nullPossible(int num)
    {
        return EtcItemEntityTable.param.Find((rowdata) => { return rowdata.etcitemNum == num; });
    }
    public Sprite[] etcitemSprite;




    // MONSTER
    [Space(10)]
    [Header("MONSTER")]
    public MonsterEntity MonsterEntityTable;    // 
    public MonsterEntity.Param getMonsterEntityTable_nullPossible(int monsterNum, int typeNum)
    {
        return MonsterEntityTable.param.Find((rowdata) => { return rowdata.monsterNum == monsterNum && rowdata.typeNum == typeNum; });
    }
    public Sprite getMonsterImage(int monsterNum, int typeNum)
    {
        MonsterEntity.Param param = getMonsterEntityTable_nullPossible(monsterNum, typeNum);
        if (param != null)
        {
            return MonsterSprite[param.apearNum];
        }
        else
        {
            return null;
        }
    }
    [SerializeField] Sprite[] MonsterSprite;
    public GameObject getMonsterPrefabs(int monsterNum, int typeNum)
    {
        MonsterEntity.Param param = getMonsterEntityTable_nullPossible(monsterNum, typeNum);
        if (param != null)
        {
            return MonsterPrefabs[param.apearNum];
        }
        else
        {
            return null;
        }
    }
    [SerializeField] GameObject[] MonsterPrefabs;

    public MonsterEnforceEntity MonsterEnforceEntityTable;    // 
    public MonsterEnforceEntity.Param getMonsterEnforceEntityTable_nullPossible(int gradeNum)
    {
        return MonsterEnforceEntityTable.param.Find((rowdata) => { return rowdata.gradeNum == gradeNum; });
    }

    public MonsterGradeEntity MonsterGradeEntityTable;    // 
    public MonsterGradeEntity.Param getMonsterGradeEntityTable_nullPossible(int gradeNum)
    {
        return MonsterGradeEntityTable.param.Find((rowdata) => { return rowdata.gradeNum == gradeNum; });
    }
    public Sprite[] monstergradeIcons;
    



    [Space(10)]
    [Header("ETC")]
    public StatEntity StatEntityTable;    // 
    public StatEntity.Param getStatEntityTable_nullPossible(StatType stattype)
    {
        return StatEntityTable.param.Find((rowdata) => { return rowdata.statType == stattype; });
    }

    public FormIndexTransTable formindextranstable;
    public int getformindextrans(int arrayindex)
    {
        return formindextranstable.param.Find((rowdata) => { return rowdata.arrayIndex == arrayindex; }).realIndex;
    }

    // 속성
    public Sprite[] TypeIcons;
}
