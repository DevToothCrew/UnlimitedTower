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
        return ServantIconEntityTable.param.Find((rowdata) => { return rowdata.charNum == charNum && rowdata.jobNum == jobNum; });
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
    public MountItemStatEntity.Param getMountItemStatEntityTable_nullPossible(int num)
    {
        return MountItemStatEntityTable.param.Find((rowdata) => { return rowdata.mountItemNum == num; });
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
    [SerializeField]  Sprite[] MonsterSprite;
    
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

    // 속성
    public Sprite[] TypeIcons;
}
