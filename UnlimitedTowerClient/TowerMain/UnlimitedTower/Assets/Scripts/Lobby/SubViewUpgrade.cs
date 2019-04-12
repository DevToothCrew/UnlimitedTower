using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubViewUpgrade : MonoBehaviour {
    public Text textTitle;

    //선택한 오브젝트 정보(이미지) : 몬스터 or 장비
    public Image FrameSelectedObject;
    public Image ImageSelectedObject;

    //강화에 사용되는 오브젝트 정보(이미지) : 같은 종류/등급 몬스터 or 장비 강화 주문서
    public Image FrameInsertObject;
    public Image ImageInsertObject;

    public Text textUgt;
    public Text textSuccessPer;

    //강화 성공시 만들어질 오브젝트 정보(이미지)
    public Image FrameResultSlot;
    public Image imageResultSlot;

    public Button buttonUpgrade;

    public GameObject prefabPartyUnit;
    public GameObject prefabItemUnit;

    public ScrollListManager scrollList;

    public List<int> scrollListData = new List<int>();
    SORT_TYPE sortType = 0;

    private PartyInfoVC partyInfo;
    private InventoryVC inventoryInfo;

    private UPGRADE_TYPE upgradeType = 0;
    private int selected_object_idx;    //선택된 오브젝트 Idx
    private int inserted_object_idx;    //강화에 사용할 오브젝트 Idx

    void Start()
    {

    }

    public UPGRADE_TYPE GetUpgradeType()
    {
        return upgradeType;
    }

    public void SetUpgradeType(UPGRADE_TYPE type)
    {
        upgradeType = type;

        if (upgradeType == UPGRADE_TYPE.MONSTER)
        {
            partyInfo = PartyInfoVC.Inst;
            scrollList.prefabUnit = prefabPartyUnit;
        }
        else//upgradeType == UPGRADE_TYPE.EQUIPMENT
        {
            inventoryInfo = InventoryVC.Inst;
            scrollList.prefabUnit = prefabItemUnit;
        }

        initScrollList();
        updateView();
    }

    //스크롤 생성
    void initScrollList()
    {
        setData();

        scrollList.Init(this, 20, scrollListData.Count, getOrder());
    }

    void setData()
    {
        scrollListData.Clear();

        switch (upgradeType)
        {
            case UPGRADE_TYPE.MONSTER:
                for (int i = 0; i < partyInfo.MonsterList.Count; i++)
                {
                    scrollListData.Add(i);
                }
                break;
            case UPGRADE_TYPE.EQUIPMENT:
                for (int i = 0; i < inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu].Count; i++)
                {
                    scrollListData.Add(i);
                }
                break;
            default:
                Debug.Log("Warning : 잘못된 타입 분해!");
                break;

        }
    }


    //스크롤 정렬
    private int[] getOrder()
    {
        int[] data_order;
        int total_list_num = scrollListData.Count;

        data_order = new int[total_list_num];

        for (int i = 0; i < data_order.Length; i++)
        {
            data_order[i] = 0;
        }

        switch (sortType)
        {
            case 0:
                for (int i = 0; i < total_list_num - 1; i++)
                {
                    for (int j = i + 1; j < total_list_num; j++)
                    {
                        if (upgradeType == UPGRADE_TYPE.MONSTER)
                        {
                            if (partyInfo.MonsterList[scrollListData[i]].level * 100 + partyInfo.MonsterList[scrollListData[i]].exp < partyInfo.MonsterList[scrollListData[j]].level * 100 + partyInfo.MonsterList[scrollListData[j]].exp)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT)
                        {
                            if (inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][scrollListData[i]].grade < inventoryInfo.EquipmentList[(int)inventoryInfo.selectedMenu][scrollListData[j]].grade)
                            {
                                data_order[i]++;
                            }
                            else
                            {
                                data_order[j]++;
                            }
                        }
                        else
                        {
                            Debug.Log("잘못된 분해 타입!");
                        }
                    }
                }
                break;
            default:
                break;
        }


        return data_order;
    }

    void updateView()
    {
        //선택중인 오브젝트 정보
        FrameSelectedObject.sprite = null;
        ImageSelectedObject.sprite = null;

        if (inserted_object_idx > 0)//강화에 사용될 오브젝트를 선택했을때
        {
            //강화에 사용될 오브젝트 정보
            //FrameInsertObject.sprite = ;
            //ImageInsertObject.sprite = ;

            //강화 비용, 성공률
            textUgt.text = string.Format("{0}", 0);
            textSuccessPer.text = string.Format("{0}%", 0);

            //강화 성공시 오브젝트 정보
            FrameResultSlot.sprite = null;
            imageResultSlot.sprite = null;

            buttonUpgrade.interactable = true;
        }
        else
        {
            FrameInsertObject.sprite = null;
            ImageInsertObject.sprite = null;
            textUgt.text = "";
            textSuccessPer.text = "";
            FrameResultSlot.sprite = null;
            imageResultSlot.sprite = null;

            buttonUpgrade.interactable = false;
        }
        
    }

    public void InsertUnit(int scroll_unit_idx)
    {
        //다른 오브젝트로 바꿀 수 있기 때문에 필요 없을듯.
        //if (inserted_object_idx > 0)
        //{
        //    Debug.Log("Warning : 이미 필요한 아이템 삽입됨!");
        //}

        UserDataManager u_data = UserDataManager.Inst;
        int unit_idx = 0;

        if (checkInsertedObject(scroll_unit_idx))
        {
            Debug.Log("Warning : 이미 분해 슬롯에 등록된 유닛 입니다.");
        }
        else
        {
            if (upgradeType == UPGRADE_TYPE.MONSTER)
            {
                UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(scroll_unit_idx);
                if (monsterData == null)
                {
                    Debug.Log("Invalid Request Monster ID : " + scroll_unit_idx);
                    return;
                }

                if (monsterData.state != 1)
                {
                    Debug.Log("Invalid Monster State : " + monsterData.state);
                    return;
                }

                if (monsterData.partyIndex != 0)
                {
                    Debug.Log("Invalid Monster Index : " + monsterData.partyIndex);
                    return;
                }
            }
            else//Equip
            {
                UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(scroll_unit_idx);
                if (equipmentData == null)
                {
                    Debug.Log("Invalid Request Servant ID : " + scroll_unit_idx);
                    return;
                }

                if (equipmentData.state != 1)
                {
                    Debug.Log("Invalid Equip State : " + equipmentData.state);
                    return;
                }

                if (equipmentData.isEquiped)
                {
                    Debug.Log("Invalid Equip isEquiped : " + equipmentData.isEquiped);
                    return;
                }
            }

            unit_idx = scroll_unit_idx;
            inserted_object_idx = unit_idx;
            updateView();
        }

    }

    bool checkInsertedObject(int unit_idx)
    {
        return unit_idx == inserted_object_idx;
    }

    public void ResetView()
    {
        inserted_object_idx = 0;
        updateView();
    }

    //강화 완료(서버에서 응답) 후 화면 전체 갱신
    public void updateViewFinishRequest()
    {
        ResetView();

        if (PartyInfoVC.checkInst())
        {
            PartyInfoVC patyInfo = PartyInfoVC.Inst;
            patyInfo.setData();
            patyInfo.resetScroll(patyInfo.currentScrollType);
            patyInfo.updateDetailInfo(patyInfo.scrollList.getFirstItemOrder());
        }
        else if (InventoryVC.checkInst())
        {
            InventoryVC inventory = InventoryVC.Inst;
            inventory.setData();
            inventory.resetScroll(inventory.currentScrollType);
            inventory.updateDetailInfo(inventory.scrollList.getFirstItemOrder());
        }

        setData();
        scrollList.SetItemOrder(getOrder());

    }

    public void OnClickClose()
    {
        if (PartyInfoVC.checkInst())
        {
            PartyInfoVC.Inst.frameScroll.SetActive(true);
        }
        else if (InventoryVC.checkInst())
        {
            InventoryVC.Inst.frameScroll.SetActive(true);
        }

        Destroy(this.gameObject);
    }

    public void OnClickButtonUpgrade()
    {
        if (upgradeType == UPGRADE_TYPE.MONSTER)  // Monster
        {
            UserMonsterData monsterData = UserDataManager.Inst.GetMonsterInfo(inserted_object_idx);
            if (monsterData == null)
            {
                Debug.Log("Invalid Request Monster ID : " + inserted_object_idx);
                return;
            }

            if (monsterData.state != 1)
            {
                Debug.Log("Invalid Monster State : " + monsterData.state);
                return;
            }

            if (monsterData.partyIndex != 0)
            {
                Debug.Log("Invalid Monster Index : " + monsterData.partyIndex);
                return;
            }

            //TODO: 서버요청
        }
        else if (upgradeType == UPGRADE_TYPE.EQUIPMENT) // Equip
        {
            UserEquipmentData equipmentData = UserDataManager.Inst.GetEquipmentInfo(inserted_object_idx);
            if (equipmentData == null)
            {
                Debug.Log("Invalid Request Equipment ID : " + inserted_object_idx);
                return;
            }

            if (equipmentData.state != 1)
            {
                Debug.Log("Invalid Equipment State : " + equipmentData.state);
                return;
            }

            if (equipmentData.equipServantIndex != 0)
            {
                Debug.Log("Invalid Equipment Already Equip Servant Index : " + equipmentData.equipServantIndex);
                return;
            }

            //TODO: 서버요청
        }
    }
}
