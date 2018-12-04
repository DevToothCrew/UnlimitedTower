using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picking : MonoSingleton<Picking> {

 
    public GameObject SelectedObject = null;
    public GameObject QuadPrefab;
    public CharController charCtrl;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        //충돌과 태그가 플레이어 것일 때
        if (Physics.Raycast(ray, out hit)) 
        {
            if ( hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Player"))
            {
                Transform charTransform = hit.transform;
                if (!SelectedObject) 
                {
                    //아직 선택하지 않았다면
                    SelectObject(ref charTransform);
                }
                else 
                {
                    //이미 선택된 객체가 있을 경우
                    if (SelectedObject != charTransform.gameObject) 
                    {
                        //옛날에 선택했던것과 다르다면
                       
                        DeleteQuad();
                    }
                }
            }
            else 
            {
                if (SelectedObject)
                {               
                    DeleteQuad();
                }
            }
        }
        else
        {
            if (SelectedObject)
            {

                DeleteQuad();
            }
        }
       
    }


    void SelectObject(ref Transform charTransform)
    {
        SelectedObject = charTransform.gameObject;
        charCtrl = SelectedObject.GetComponent<CharController>();

        InGameCharacterStateUI.Inst.gameObject.SetActive(true);
        InGameCharacterStateUI.SetStatus(charCtrl.status);

        if (charCtrl.QuadSelectionObject == null)
        {
            AddSelectionQuad();
        }
    }

    void AddSelectionQuad()
    {
        Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
        charCtrl.QuadSelectionObject = Instantiate(QuadPrefab);
        charCtrl.QuadSelectionObject.transform.SetParent(charCtrl.transform, false);
        charCtrl.QuadSelectionObject.transform.position = new Vector3(charCtrl.QuadSelectionObject.transform.position.x, 0.042f, charCtrl.QuadSelectionObject.transform.position.z);
        charCtrl.QuadSelectionObject.transform.eulerAngles = new Vector3(90, 0, 0);


        switch(charCtrl.charSize)
        {
            case SIZE_TYPE.SMALL:
                {
                    break;
                }
            case SIZE_TYPE.MIDDLE:
                {
                    scale *= 2.0f;
                   
                    break;
                }
            case SIZE_TYPE.BIG:
                {
                    scale *= 3.0f;
                    break;
                }
        }

        charCtrl.QuadSelectionObject.transform.localScale = scale;
    }
    void DeleteQuad()
    {
        charCtrl = SelectedObject.GetComponent<CharController>();
        InGameCharacterStateUI.Inst.gameObject.SetActive(false);
        if (charCtrl.QuadSelectionObject)
        {
            Destroy(charCtrl.QuadSelectionObject);
        }
        SelectedObject = null;
    }





    void ChangeShader(Transform transform, string shader)
    {
        MeshRenderer mr = GetMeshRenderer(transform);
        mr.material.shader = Shader.Find(shader);
    }

    MeshRenderer GetMeshRenderer(Transform transform)
    {   
         return transform.GetChild(0).GetComponent<MeshRenderer>();     
    }
   
}


