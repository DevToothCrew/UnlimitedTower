using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterChangeTest : MonoBehaviour {
    public List<TextMesh[]> ChangeMeshList = new List<TextMesh[]>();
    public TextMesh[] ChangeMesh = new TextMesh[15];
    public TextMesh[] OriginalMesh = new TextMesh[15];
    public SkinnedMeshRenderer[] ThisMesh = new SkinnedMeshRenderer[15];

    [System.Serializable]
    public class TextMesh
    {
        public string text;
        public Mesh ChangeMesh;
    }

    public void Start()
    {
        for (int i = 0; i < 15; i++)
        {
            ThisMesh[i] = transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
        }

        for (int i = 0; i < 15; i++)
        {
            OriginalMesh[i].ChangeMesh = transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            LegChange();
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            OriginalLegChange();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            BodyChange();
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            OriginalBodyChange();
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            HeadChange();
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            OriginalHeadChange();
        }
    }

    [ContextMenu("HeadChange")]
    public void HeadChange()
    {
        ThisMesh[4].sharedMesh = ChangeMesh[4].ChangeMesh;
    }

    public void OriginalHeadChange()
    {
        ThisMesh[4].sharedMesh = OriginalMesh[4].ChangeMesh;
    }

    [ContextMenu("BodyChange")]
    public void BodyChange()
    {
        ThisMesh[0].sharedMesh = ChangeMesh[0].ChangeMesh;
        ThisMesh[1].sharedMesh = ChangeMesh[1].ChangeMesh;
        ThisMesh[2].sharedMesh = ChangeMesh[2].ChangeMesh;
        ThisMesh[3].sharedMesh = ChangeMesh[3].ChangeMesh;
        ThisMesh[5].sharedMesh = ChangeMesh[5].ChangeMesh;
        ThisMesh[8].sharedMesh = ChangeMesh[8].ChangeMesh;
        ThisMesh[12].sharedMesh = ChangeMesh[12].ChangeMesh;
        ThisMesh[13].sharedMesh = ChangeMesh[13].ChangeMesh;
        ThisMesh[14].sharedMesh = ChangeMesh[14].ChangeMesh;
    }

    public void OriginalBodyChange()
    {
        ThisMesh[0].sharedMesh = OriginalMesh[0].ChangeMesh;
        ThisMesh[1].sharedMesh = OriginalMesh[1].ChangeMesh;
        ThisMesh[2].sharedMesh = OriginalMesh[2].ChangeMesh;
        ThisMesh[3].sharedMesh = OriginalMesh[3].ChangeMesh;
        ThisMesh[5].sharedMesh = OriginalMesh[5].ChangeMesh;
        ThisMesh[8].sharedMesh = OriginalMesh[8].ChangeMesh;
        ThisMesh[12].sharedMesh = OriginalMesh[12].ChangeMesh;
        ThisMesh[13].sharedMesh = OriginalMesh[13].ChangeMesh;
        ThisMesh[14].sharedMesh = OriginalMesh[14].ChangeMesh;
    }

    [ContextMenu("LegChange")]
    public void LegChange()
    {
        ThisMesh[6].sharedMesh = ChangeMesh[6].ChangeMesh;
        ThisMesh[7].sharedMesh = ChangeMesh[7].ChangeMesh;
        ThisMesh[9].sharedMesh = ChangeMesh[9].ChangeMesh;
        ThisMesh[10].sharedMesh = ChangeMesh[10].ChangeMesh;
        ThisMesh[11].sharedMesh = ChangeMesh[11].ChangeMesh;
    }

    public void OriginalLegChange()
    {
        ThisMesh[6].sharedMesh = OriginalMesh[6].ChangeMesh;
        ThisMesh[7].sharedMesh = OriginalMesh[7].ChangeMesh;
        ThisMesh[9].sharedMesh = OriginalMesh[9].ChangeMesh;
        ThisMesh[10].sharedMesh = OriginalMesh[10].ChangeMesh;
        ThisMesh[11].sharedMesh = OriginalMesh[11].ChangeMesh;
    }
}
