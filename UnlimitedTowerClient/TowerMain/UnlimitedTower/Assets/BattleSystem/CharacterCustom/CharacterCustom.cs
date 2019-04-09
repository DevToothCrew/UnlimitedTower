using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class CharacterCustom : MonoBehaviour
{
    /// <summary>
    /// 0은 백수
    /// 1은 워리어
    /// 2는 도적
    /// 3은 사제
    /// 4는 아처
    /// 5는 마법사
    /// 키 입력으로 ++ -- 해야해서 임시로 int형 선언
    /// 아래 값들을 변경한 후 Refrash()
    /// </summary>
    [Header("- Value Index")]
    public int jobIndex;
    public int headIndex;
    public int hairIndex;
    public int isMan;
    public int isChildren;
    
    [Header("- Change Mesh")]
    [FormerlySerializedAs("WhiteHandCharacterMeshList")]
    public List<CharacterMeshList> WhiteHandCharacterMeshList = new List<CharacterMeshList>();

    [FormerlySerializedAs("WarriorCharacterMeshList")]
    public List<CharacterMeshList> WarriorCharacterMeshList = new List<CharacterMeshList>();

    [FormerlySerializedAs("ArcherCharacterMeshList")]
    public List<CharacterMeshList> ArcherCharacterMeshList = new List<CharacterMeshList>();

    [FormerlySerializedAs("TheifCharacterMeshList")]
    public List<CharacterMeshList> TheifCharacterMeshList = new List<CharacterMeshList>();

    [FormerlySerializedAs("MagicianCharacterMeshList")]
    public List<CharacterMeshList> MagicianCharacterMeshList = new List<CharacterMeshList>();

    [FormerlySerializedAs("ClericCharacterMeshList")]
    public List<CharacterMeshList> ClericCharacterMeshList = new List<CharacterMeshList>();

    [FormerlySerializedAs("CharacterSkinnedMeshList")]
    public CharacterSkinnedMeshList characterSkinnedMeshList = new CharacterSkinnedMeshList();

    public GameObject defultCharacter;
    public GameObject defultCharacterParent;

    [Header("- Head Mash")]
    public HeadParts[] adultManHead = new HeadParts[9];
    public HeadParts[] adultWomanHead = new HeadParts[9];
    public HeadParts[] childrenManHead = new HeadParts[9];
    public HeadParts[] childrenWomanHead = new HeadParts[9];

    [Header("- Bullet")]
    public GameObject ArcherArrow;
    public GameObject MagicianBullet;

    public RuntimeAnimatorController[] animatorController = new RuntimeAnimatorController[6];
    
    private readonly CharacterInfo[] characterInformation = new CharacterInfo[6];

    // 데이터 저장용
    [System.Serializable]
    public class CharacterInfo
    {
        public float AttackDelay;
        public float AttackAfterDelay;
        public float Height;
        public float AttackRange;

        public CharacterInfo(float AttackDelay, float AttackAfterDelay, float Height, float AttackRange)
        {
            this.AttackDelay = AttackDelay;
            this.AttackAfterDelay = AttackAfterDelay;
            this.Height = Height;
            this.AttackRange = AttackRange;
        }
    }
    
    private void Awake()
    {
        WarriorCharacterMeshList[0].frame = GameObject.Find("WarriorFrame")?.transform.GetChild(1).GetChild(0).GetChild(0);
        WarriorCharacterMeshList[1].frame = GameObject.Find("WarriorFrame")?.transform.GetChild(0).GetChild(0).GetChild(0);
        WarriorCharacterMeshList[2].frame = GameObject.Find("WarriorFrame")?.transform.GetChild(1).GetChild(1).GetChild(0);
        WarriorCharacterMeshList[3].frame = GameObject.Find("WarriorFrame")?.transform.GetChild(0).GetChild(1).GetChild(0);

        ArcherCharacterMeshList[0].frame = GameObject.Find("ArcherFrame")?.transform.GetChild(1).GetChild(0).GetChild(0);
        ArcherCharacterMeshList[1].frame = GameObject.Find("ArcherFrame")?.transform.GetChild(0).GetChild(0).GetChild(0);
        ArcherCharacterMeshList[2].frame = GameObject.Find("ArcherFrame")?.transform.GetChild(1).GetChild(1).GetChild(0);
        ArcherCharacterMeshList[3].frame = GameObject.Find("ArcherFrame")?.transform.GetChild(0).GetChild(1).GetChild(0);

        TheifCharacterMeshList[0].frame = GameObject.Find("TheifFrame")?.transform.GetChild(1).GetChild(0).GetChild(0);
        TheifCharacterMeshList[1].frame = GameObject.Find("TheifFrame")?.transform.GetChild(0).GetChild(0).GetChild(0);
        TheifCharacterMeshList[2].frame = GameObject.Find("TheifFrame")?.transform.GetChild(1).GetChild(1).GetChild(0);
        TheifCharacterMeshList[3].frame = GameObject.Find("TheifFrame")?.transform.GetChild(0).GetChild(1).GetChild(0);

        MagicianCharacterMeshList[0].frame = GameObject.Find("MagicianFrame")?.transform.GetChild(1).GetChild(0).GetChild(0);
        MagicianCharacterMeshList[1].frame = GameObject.Find("MagicianFrame")?.transform.GetChild(0).GetChild(0).GetChild(0);
        MagicianCharacterMeshList[2].frame = GameObject.Find("MagicianFrame")?.transform.GetChild(1).GetChild(1).GetChild(0);
        MagicianCharacterMeshList[3].frame = GameObject.Find("MagicianFrame")?.transform.GetChild(0).GetChild(1).GetChild(0);

        ClericCharacterMeshList[0].frame = GameObject.Find("ClericFrame")?.transform.GetChild(1).GetChild(0).GetChild(0);
        ClericCharacterMeshList[1].frame = GameObject.Find("ClericFrame")?.transform.GetChild(0).GetChild(0).GetChild(0);
        ClericCharacterMeshList[2].frame = GameObject.Find("ClericFrame")?.transform.GetChild(1).GetChild(1).GetChild(0);
        ClericCharacterMeshList[3].frame = GameObject.Find("ClericFrame")?.transform.GetChild(0).GetChild(1).GetChild(0);

        WhiteHandCharacterMeshList[0].frame = GameObject.Find("WhiteHandFrame")?.transform.GetChild(1).GetChild(0).GetChild(0);
        WhiteHandCharacterMeshList[1].frame = GameObject.Find("WhiteHandFrame")?.transform.GetChild(0).GetChild(0).GetChild(0);
        WhiteHandCharacterMeshList[2].frame = GameObject.Find("WhiteHandFrame")?.transform.GetChild(1).GetChild(1).GetChild(0);
        WhiteHandCharacterMeshList[3].frame = GameObject.Find("WhiteHandFrame")?.transform.GetChild(0).GetChild(1).GetChild(0);

        characterInformation[0] = new CharacterInfo(0.65f, 1.0f, 1.5f, 1.0f);
        characterInformation[1] = new CharacterInfo(0.9f, 1.0f, 1.5f, 0.7f);
        characterInformation[2] = new CharacterInfo(0.9f, 1.0f, 1.5f, 0.7f);
        characterInformation[3] = new CharacterInfo(0.9f, 1.0f, 1.5f, 0.6f);
        characterInformation[4] = new CharacterInfo(2.0f, 0.7f, 1.5f, 0.0f);
        characterInformation[5] = new CharacterInfo(0.8f, 0.7f, 1.5f, 0.0f);

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            if(UserDataManager.Inst.GetMainCharInfo() != null)
            {
                DBServantData servantData = CSVData.Inst.GetServantData(UserDataManager.Inst.GetMainCharInfo().mainCharID);
                jobIndex = servantData.job;
                headIndex = servantData.head;
                hairIndex = servantData.hair;
                isMan = servantData.gender;
                isChildren = servantData.body;
            }
        }

        Refresh();
    }

    [System.Serializable]
    public struct HeadParts
    {
        public Material material;
        public Mesh mesh;
    }

    [System.Serializable]
    public class CharacterMeshList
    {
        public string Name;
        public Avatar avatar;
        public Transform frame;
        public Material material;
        public Mesh Head;
        public Mesh Neck;
        public Mesh Body;
        public Mesh LeftHand;
        public Mesh LeftWrist;
        public Mesh LeftArm;
        public Mesh LeftShoulder;
        public Mesh RightHand;
        public Mesh RightWrist;
        public Mesh RightArm;
        public Mesh RightShoulder;
        public Mesh LeftLeg;
        public Mesh LeftFoot;
        public Mesh RightLeg;
        public Mesh RightFoot;
    }

    [System.Serializable]
    public class CharacterSkinnedMeshList
    {
        public Animator animator;
        public Transform frame;
        public SkinnedMeshRenderer Head;
        public SkinnedMeshRenderer Neck;
        public SkinnedMeshRenderer Body;
        public SkinnedMeshRenderer LeftHand;
        public SkinnedMeshRenderer LeftWrist;
        public SkinnedMeshRenderer LeftArm;
        public SkinnedMeshRenderer LeftShoulder;
        public SkinnedMeshRenderer RightHand;
        public SkinnedMeshRenderer RightWrist;
        public SkinnedMeshRenderer RightArm;
        public SkinnedMeshRenderer RightShoulder;
        public SkinnedMeshRenderer LeftLeg;
        public SkinnedMeshRenderer LeftFoot;
        public SkinnedMeshRenderer RightLeg;
        public SkinnedMeshRenderer RightFoot;

        // 본, 메쉬 포함 직업바꿀때 전체 변경
        public void MeshChange(CharacterMeshList MeshList)
        {
            ValueCopy(frame, MeshList.frame);
            animator.avatar = MeshList.avatar;
            Head.sharedMesh = MeshList.Head;
            Neck.sharedMesh = MeshList.Neck;
            Body.sharedMesh = MeshList.Body;
            LeftHand.sharedMesh = MeshList.LeftHand;
            LeftWrist.sharedMesh = MeshList.LeftWrist;
            LeftArm.sharedMesh = MeshList.LeftArm;
            LeftShoulder.sharedMesh = MeshList.LeftShoulder;
            RightHand.sharedMesh = MeshList.RightHand;
            RightWrist.sharedMesh = MeshList.RightWrist;
            RightArm.sharedMesh = MeshList.RightArm;
            RightShoulder.sharedMesh = MeshList.RightShoulder;
            LeftLeg.sharedMesh = MeshList.LeftLeg;
            LeftFoot.sharedMesh = MeshList.LeftFoot;
            RightLeg.sharedMesh = MeshList.RightLeg;
            RightFoot.sharedMesh = MeshList.RightFoot;

            Head.material = MeshList.material;
            Neck.material = MeshList.material;
            Body.material = MeshList.material;
            LeftHand.material = MeshList.material;
            LeftWrist.material = MeshList.material;
            LeftArm.material = MeshList.material;
            LeftShoulder.material = MeshList.material;
            RightHand.material = MeshList.material;
            RightWrist.material = MeshList.material;
            RightArm.material = MeshList.material;
            RightShoulder.material = MeshList.material;
            LeftLeg.material = MeshList.material;
            LeftFoot.material = MeshList.material;
            RightLeg.material = MeshList.material;
            RightFoot.material = MeshList.material;
        }

        // 머리는 메터리얼까지 추가해서 변경
        public void HeadChange(HeadParts headParts)
        {
            Head.sharedMesh = headParts.mesh;
            Head.material = headParts.material;
        }

        // 몸은 성인남성 성인여성 남자아이 여자아이
        public void BodyChange(CharacterMeshList MeshList)
        {
            Neck.sharedMesh = MeshList.Neck;
            Body.sharedMesh = MeshList.Body;
            LeftHand.sharedMesh = MeshList.LeftHand;
            LeftWrist.sharedMesh = MeshList.LeftWrist;
            LeftArm.sharedMesh = MeshList.LeftArm;
            LeftShoulder.sharedMesh = MeshList.LeftShoulder;
            RightHand.sharedMesh = MeshList.RightHand;
            RightWrist.sharedMesh = MeshList.RightWrist;
            RightArm.sharedMesh = MeshList.RightArm;
            RightShoulder.sharedMesh = MeshList.RightShoulder;
            LeftLeg.sharedMesh = MeshList.LeftLeg;
            LeftFoot.sharedMesh = MeshList.LeftFoot;
            RightLeg.sharedMesh = MeshList.RightLeg;
            RightFoot.sharedMesh = MeshList.RightFoot;
        }

        // 크기 카피
        public void ValueCopy(Transform Original, Transform Copy)
        {
            List<Transform> OriginalList = new List<Transform>();
            List<Transform> CopyList = new List<Transform>();
            OriginalList.AddRange(Original.GetComponentsInChildren<Transform>());
            CopyList.AddRange(Copy.GetComponentsInChildren<Transform>());
            for (int i = 0; i < OriginalList.Count; i++)
            {
                OriginalList[i].SetPositionAndRotation(CopyList[i].position, CopyList[i].rotation);
                OriginalList[i].localScale = CopyList[i].localScale;
            }
        }
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) && SceneManager.GetActiveScene().name.CompareTo("Customize") == 0)
        {
            defultCharacter.transform.Rotate(0, -Input.GetAxis("Mouse X") * 10, 0);
        }
    }

    public GameObject Create(int jobIndex, int headIndex, int hairIndex, int isMan, int isChildren)
    {
        this.jobIndex = jobIndex;
        this.headIndex = headIndex;
        this.hairIndex = hairIndex;
        this.isMan = isMan;
        this.isChildren = isChildren;
        Refresh();
        CharInfo CharacterInformationTemp = defultCharacter.GetComponent<CharInfo>();
        CharacterInformationTemp.AttackDelay = characterInformation[jobIndex].AttackDelay;
        CharacterInformationTemp.AttackAfterDelay = characterInformation[jobIndex].AttackAfterDelay;
        CharacterInformationTemp.Height = characterInformation[jobIndex].Height;
        CharacterInformationTemp.AttackRange = characterInformation[jobIndex].AttackRange;

        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            if (jobIndex == 4)
            {
                defultCharacter.GetComponent<Bullet>().bullet = BulletGroup.Inst.bullet["ArcherArrow"];
            }
            else if (jobIndex == 5)
            {
                defultCharacter.GetComponent<Bullet>().bullet = BulletGroup.Inst.bullet["MagicianFireBall"];
            }
        }
            

        return defultCharacter;
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        switch (jobIndex)
        {
            case 0:
                characterSkinnedMeshList.MeshChange(WhiteHandCharacterMeshList[(isMan == 1 ? 1 : 0) + (isChildren == 1 ? 0 : 1) * 2]);
                break;
            case 1:
                characterSkinnedMeshList.MeshChange(WarriorCharacterMeshList[(isMan == 1 ? 1 : 0) + (isChildren == 1 ? 0 : 1) * 2]);
                break;
            case 2:
                characterSkinnedMeshList.MeshChange(TheifCharacterMeshList[(isMan == 1 ? 1 : 0) + (isChildren == 1 ? 0 : 1) * 2]);
                break;
            case 3:
                characterSkinnedMeshList.MeshChange(ClericCharacterMeshList[(isMan == 1 ? 1 : 0) + (isChildren == 1 ? 0 : 1) * 2]);
                break;
            case 4:
                characterSkinnedMeshList.MeshChange(ArcherCharacterMeshList[(isMan == 1 ? 1 : 0) + (isChildren == 1 ? 0 : 1) * 2]);
                break;
            case 5:
                characterSkinnedMeshList.MeshChange(MagicianCharacterMeshList[(isMan == 1 ? 1 : 0) + (isChildren == 1 ? 0 : 1) * 2]);
                break;
        }
        if ((isMan == 1 ? 1 : 0) == 1)
        {
            if ((isChildren == 1 ? 0 : 1) == 0)
                characterSkinnedMeshList.HeadChange(adultManHead[(hairIndex - 1) * 3 + (headIndex - 1)]);
            else
                characterSkinnedMeshList.HeadChange(childrenManHead[(hairIndex - 1) * 3 + (headIndex - 1)]);
        }
        else
        {
            if ((isChildren == 1 ? 0 : 1) == 0)
                characterSkinnedMeshList.HeadChange(adultWomanHead[(hairIndex - 1) * 3 + (headIndex - 1)]);
            else
                characterSkinnedMeshList.HeadChange(childrenWomanHead[(hairIndex - 1) * 3 + (headIndex - 1)]);
        }
        defultCharacter.GetComponent<Animator>().runtimeAnimatorController = animatorController[jobIndex];
    }

    public void ManIndexChange()
    {
        isMan = 1;
        Refresh();
    }

    public void WomanIndexChange()
    {
        isMan = 0;
        Refresh();
    }

    public void ChildrenIndexChange()
    {
        isChildren = 1;
        Refresh();
    }

    public void AdultIndexChange()
    {
        isChildren = 0;
        Refresh();
    }

    public void FaceIndexUp()
    {
        if (hairIndex < 2)
        {
            hairIndex++;
            Refresh();
        }
    }

    public void FaceIndexDown()
    {
        if (hairIndex > 0)
        {
            hairIndex--;
            Refresh();
        }
    }

    public void HairIndexUp()
    {
        if (headIndex < 2)
        {
            headIndex++;
            Refresh();
        }
    }

    public void HairIndexDown()
    {
        if (headIndex > 0)
        {
            headIndex--;
            Refresh();
        }
    }
}
