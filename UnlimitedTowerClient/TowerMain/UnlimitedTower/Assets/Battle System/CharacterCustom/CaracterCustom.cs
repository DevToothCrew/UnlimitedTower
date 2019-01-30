using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CaracterCustom : MonoBehaviour
{
    /// <summary>
    /// 0은 백수
    /// 1은 워리어
    /// 2는 아처
    /// 3은 엘프
    /// 4는 마법사
    /// 5는 사제
    /// 키 입력으로 ++ -- 해야해서 임시로 int형 선언
    /// </summary>
    public int JobIndex;
    public int HeadIndex;
    public int HairIndex;
    public int isMan;
    public int isChildren;

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

    public GameObject DefultCharacter;

    public HeadParts[] AdultManHead = new HeadParts[9];
    public HeadParts[] AdultWomanHead = new HeadParts[9];
    public HeadParts[] ChildrenManHead = new HeadParts[9];
    public HeadParts[] ChildrenWomanHead = new HeadParts[9];

    public ParticleSystem RingEffect;
    public GameObject AdultBodyEffect;
    public GameObject AdultHeadEffect;
    public GameObject ChildrenBodyEffect;
    public GameObject ChildrenHeadEffect;

    private Color[] JobColor = new Color[6];

    private void Awake()
    {
        JobColor[1] = new Color(0.188f, 0.188f, 0.2f, 1.0f);
        JobColor[2] = new Color(0.4f, 0.152f, 0, 1.0f);
        JobColor[3] = new Color(0.6f, 0.4f, 0, 1.0f);
        JobColor[4] = new Color(0.388f, 0.494f, 0.741f, 1.0f);
        JobColor[5] = new Color(1, 0.309f, 0.309f, 1.0f);
        Refresh();
        StartCoroutine(ColorChange());
    }

    [System.Serializable]
    public struct HeadParts
    {
        public Material material;
        public Mesh mesh;
    }

    [System.Serializable]
    public struct CharacterMeshList
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
            ValueLead(OriginalList, Original);
            ValueLead(CopyList, Copy);

            for (int i = 0; i < OriginalList.Count; i++)
            {
                OriginalList[i].SetPositionAndRotation(CopyList[i].position, CopyList[i].rotation);
                OriginalList[i].localScale = CopyList[i].localScale;
            }
        }

        // Transform 재귀함수
        public void ValueLead(List<Transform> transformList, Transform tf)
        {
            transformList.Add(tf);
            if (tf.childCount > 0)
            {
                for (int i = 0; i < tf.childCount; i++)
                    ValueLead(transformList, tf.GetChild(i));
            }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (JobIndex > 1)
            {
                JobIndex--;
                Refresh();
                BodyChangeEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (JobIndex < 5)
            {
                JobIndex++;
                Refresh();
                BodyChangeEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (HeadIndex > 0)
            {
                HeadIndex--;
                Refresh();
                HeadChangeEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            if (HeadIndex < 2)
            {
                HeadIndex++;
                Refresh();
                HeadChangeEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            if (HairIndex > 0)
            {
                HairIndex--;
                Refresh();
                HeadChangeEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            if (HairIndex < 2)
            {
                HairIndex++;
                Refresh();
                HeadChangeEffect();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            isMan = isMan == 0 ? 1 : 0;
            Refresh();
            BodyChangeEffect();
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            isChildren = isChildren == 0 ? 1 : 0;
            Refresh();
            BodyChangeEffect();
        }

        if (Input.GetMouseButton(0))
        {
            DefultCharacter.transform.Rotate(0, -Input.GetAxis("Mouse X") * 10, 0);
        }
    }

    private void HeadChangeEffect()
    {
        if (isChildren == 0)
        {
            AdultHeadEffect.SetActive(false);
            AdultHeadEffect.SetActive(true);
        }
        else
        {
            ChildrenHeadEffect.SetActive(false);
            ChildrenHeadEffect.SetActive(true);
        }
    }

    private void BodyChangeEffect()
    {
        if (isChildren == 0)
        {
            AdultBodyEffect.SetActive(false);
            AdultBodyEffect.SetActive(true);
        }
        else
        {
            ChildrenBodyEffect.SetActive(false);
            ChildrenBodyEffect.SetActive(true);
        }
    }

    private void Refresh()
    {
        switch (JobIndex)
        {
            case 0:
                characterSkinnedMeshList.MeshChange(WhiteHandCharacterMeshList[isMan + isChildren * 2]);
                break;
            case 1:
                characterSkinnedMeshList.MeshChange(WarriorCharacterMeshList[isMan + isChildren * 2]);
                break;
            case 2:
                characterSkinnedMeshList.MeshChange(ArcherCharacterMeshList[isMan + isChildren * 2]);
                break;
            case 3:
                characterSkinnedMeshList.MeshChange(TheifCharacterMeshList[isMan + isChildren * 2]);
                break;
            case 4:
                characterSkinnedMeshList.MeshChange(MagicianCharacterMeshList[isMan + isChildren * 2]);
                break;
            case 5:
                characterSkinnedMeshList.MeshChange(ClericCharacterMeshList[isMan + isChildren * 2]);
                break;
        }
        if (isMan == 1)
        {
            if (isChildren == 0)
                characterSkinnedMeshList.HeadChange(AdultManHead[HeadIndex * 3 + HairIndex]);
            else
                characterSkinnedMeshList.HeadChange(ChildrenManHead[HeadIndex * 3 + HairIndex]);
        }
        else
        {
            if (isChildren == 0)
                characterSkinnedMeshList.HeadChange(AdultWomanHead[HeadIndex * 3 + HairIndex]);
            else
                characterSkinnedMeshList.HeadChange(ChildrenWomanHead[HeadIndex * 3 + HairIndex]);
        }
    }
    
    IEnumerator ColorChange()
    {
        while (true)
        {
            RingEffect.startColor = Color.Lerp(RingEffect.startColor, JobColor[JobIndex], 0.2f);
            yield return null;
        }
    }
}
