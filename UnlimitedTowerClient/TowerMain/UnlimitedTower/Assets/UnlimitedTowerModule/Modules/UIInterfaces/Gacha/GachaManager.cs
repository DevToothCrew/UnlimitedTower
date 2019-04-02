using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour {

    static public GachaManager Instance = null;

    [SerializeField]
    private GachaBoxParticleController particleController;

    [SerializeField]
    private RawImage particleFxPanel;

    [SerializeField]
    private RawImage glowingFxPanel;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private GameObject gachaResultPopup;

    [SerializeField]
    private Dictionary<GRADE_TYPE, Texture> GradeTextureDic = new Dictionary<GRADE_TYPE, Texture>();

    [SerializeField]
    private Dictionary<EQUIPMENT_OPTION_TYPE, Texture> EquipmentOptionTextureDic = new Dictionary<EQUIPMENT_OPTION_TYPE, Texture>();

    public float glowingFxPanelAlpha = 0.0f;
    
    private void Update()
    {
        glowingFxPanel?.material?.SetFloat("_Alpha", glowingFxPanelAlpha);
    }

    private void OnEnable()
    {
        Instance = Instance ?? this;
        SetGradeTexture();
        SetEquipmentOptionTexture();
    }

    private void OnDisable()
    {
        Instance = Instance == this ? null : Instance;
    }

    public void SetButtonActivate(bool value)
    {
        startButton.interactable = value;
    }

    public void ExecuteGacha(bool withoutAnimation = false)
    {
        // bool 처리를 통해 Gacha 처리가 필요할수도

        // GachaIndex에 따른 필요 EOS를 체크한다.

        // 현재는 1EOS로 박아두기

        if (UserDataManager.Inst.GetUserEOS() < 1)
        {
            return;
        }

#if UNITY_EDITOR
        {
            Cheat.Inst.RequestGachaCheat(1);
        }
#else
        {
            PacketManager.Inst.RequestGacha(1);
        }
#endif
    }

    public void CloseGachaResult()
    {
        gachaResultPopup.SetActive(false);
        SetButtonActivate(true);
        GetComponent<Animator>()?.SetTrigger("Invisible");
    }

    public void ExitGachaScene()
    {
        GetComponent<Animator>()?.SetTrigger("Invisible");
    }

    public void ResultGacha(object getObject)
    {
        SetButtonActivate(false);

        GetComponent<Animator>()?.SetTrigger("Particle");
        particleController.BeginSummonAnimation(() =>
        {
            gachaResultPopup.SetActive(true);
            GetComponent<Animator>()?.SetTrigger("Glowing");
            GachaResultPopup.PopupAlert(getObject);
        });
    }


    public void SetGradeTexture()
    {
        if (GradeTextureDic.ContainsKey(GRADE_TYPE.LEGENDARY) == false)
        {
            GradeTextureDic.Add(GRADE_TYPE.LEGENDARY, Resources.Load<Sprite>("UI/Common/gacha_grade_1").texture);
        }
        if (GradeTextureDic.ContainsKey(GRADE_TYPE.UNIQUE) == false)
        {
            GradeTextureDic.Add(GRADE_TYPE.UNIQUE, Resources.Load<Sprite>("UI/Common/gacha_grade_2").texture);
        }
        if (GradeTextureDic.ContainsKey(GRADE_TYPE.RARE) == false)
        {
            GradeTextureDic.Add(GRADE_TYPE.RARE, Resources.Load<Sprite>("UI/Common/gacha_grade_3").texture);
        }
        if (GradeTextureDic.ContainsKey(GRADE_TYPE.UNCOMMON) == false)
        {
            GradeTextureDic.Add(GRADE_TYPE.UNCOMMON, Resources.Load<Sprite>("UI/Common/gacha_grade_4").texture);
        }
        if (GradeTextureDic.ContainsKey(GRADE_TYPE.COMMON) == false)
        {
            GradeTextureDic.Add(GRADE_TYPE.COMMON, Resources.Load<Sprite>("UI/Common/gacha_grade_5").texture);
        }
    }

    public Texture GetGradeTexture(GRADE_TYPE grade)
    {
        switch (grade)
        {
            case GRADE_TYPE.COMMON:
            case GRADE_TYPE.UNCOMMON:
            case GRADE_TYPE.RARE:
            case GRADE_TYPE.UNIQUE:
            case GRADE_TYPE.LEGENDARY:
                return GradeTextureDic[grade];

            default:
                return null;
        }
    }

    public void SetEquipmentOptionTexture()
    {
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.ATK) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.ATK, Resources.Load<Sprite>("UI/Status/Status_Atk").texture);
        }
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.MATK) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.MATK, Resources.Load<Sprite>("UI/Status/Status_MAtk").texture);
        }
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.DEF) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.DEF, Resources.Load<Sprite>("UI/Status/Status_Def").texture);
        }
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.MDEF) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.MDEF, Resources.Load<Sprite>("UI/Status/Status_MDef").texture);
        }
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.HP) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.HP, Resources.Load<Sprite>("UI/Status/Status_Hp").texture);
        }
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.STR) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.STR, Resources.Load<Sprite>("UI/Status/Status_Str").texture);
        }
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.DEX) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.DEX, Resources.Load<Sprite>("UI/Status/Status_Dex").texture);
        }
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.INT) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.INT, Resources.Load<Sprite>("UI/Status/Status_Int").texture);
        }
        if (EquipmentOptionTextureDic.ContainsKey(EQUIPMENT_OPTION_TYPE.MAX) == false)
        {
            EquipmentOptionTextureDic.Add(EQUIPMENT_OPTION_TYPE.MAX, Resources.Load<Sprite>("UI/Status/Status_Box").texture);
        }
    }

    public Texture GetEquipmentOptionTexture(EQUIPMENT_OPTION_TYPE option)
    {
        switch (option)
        {
            case EQUIPMENT_OPTION_TYPE.ATK:
            case EQUIPMENT_OPTION_TYPE.MATK:
            case EQUIPMENT_OPTION_TYPE.DEF:
            case EQUIPMENT_OPTION_TYPE.MDEF:
            case EQUIPMENT_OPTION_TYPE.HP:
            case EQUIPMENT_OPTION_TYPE.STR:
            case EQUIPMENT_OPTION_TYPE.DEX:
            case EQUIPMENT_OPTION_TYPE.INT:
            case EQUIPMENT_OPTION_TYPE.MAX:
                return EquipmentOptionTextureDic[option];

            default:
                return null;
        }
    }

}
