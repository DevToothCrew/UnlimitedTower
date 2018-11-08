using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picking : MonoSingleton<Picking> {

 
    public GameObject selectedObject = null;
    public GameObject circlePrefab;
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
                Transform transform = hit.transform;
                if (!selectedObject) 
                {
                    //아직 선택하지 않았다면
                   // ChangeShader(transform, DEFINE.OUTLINE_SHADER);


                    selectedObject = transform.gameObject;
                    charCtrl = selectedObject.GetComponent<CharController>();

                    if (charCtrl.circleObject == null)
                    {
                        AddSelectionQuad();
                    }
                }
                else 
                {
                    //이미 선택된 객체가 있을 경우
                    if (selectedObject != transform.gameObject) 
                    {
                        //옛날에 선택했던것과 다르다면
                        //ChangeShader(selectedObject.transform, DEFINE.BASIC_SHADER);
                        //ChangeShader(hit.transform, DEFINE.OUTLINE_SHADER);

                        charCtrl = selectedObject.GetComponent<CharController>();
                        if (charCtrl.circleObject)
                        {
                            DeleteCircle();
                        }

                        selectedObject = transform.gameObject;
                        charCtrl = selectedObject.GetComponent<CharController>();
                        if (charCtrl.circleObject == null)
                        {
                            AddSelectionQuad();
                        }
                    }
                }
            }
            else 
            {
                if (selectedObject)
                {
                    // ChangeShader(selectedObject.transform, DEFINE.BASIC_SHADER);
                    charCtrl = selectedObject.GetComponent<CharController>();
                    if (charCtrl.circleObject)
                    {
                        Destroy(charCtrl.circleObject);
                    }
                    selectedObject = null;                   
                }
            }
        }
       
    }

    void AddSelectionQuad()
    {
        charCtrl.circleObject = Instantiate(circlePrefab);
        charCtrl.circleObject.transform.SetParent(charCtrl.transform, false);
        charCtrl.circleObject.transform.position = new Vector3(charCtrl.circleObject.transform.position.x, 0.001f, charCtrl.circleObject.transform.position.z);
        charCtrl.circleObject.transform.eulerAngles = new Vector3(90, 0, 0);
    }
    void DeleteCircle()
    {
        Destroy(charCtrl.circleObject);
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


