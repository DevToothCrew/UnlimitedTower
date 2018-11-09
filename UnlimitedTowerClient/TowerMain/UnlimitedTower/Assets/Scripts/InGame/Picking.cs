using UnityEngine;

public class Picking : MonoSingleton<Picking> {

 
    public GameObject SelectedCharObject = null;

    public GameObject SelectionStatusQuadPrefab;
    public GameObject SelectionTargetQuadPrefab;
    public CharController charCtrl;


    public GameObject HeroTargetObject;

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
                if (!SelectedCharObject) 
                {
                    //아직 선택하지 않았다면
                    CreateSelectionQuad(ref charTransform);
                    //Target();

                }
                else 
                {
                    //이미 선택된 객체가 있을 경우
                    if (SelectedCharObject != charTransform.gameObject ) 
                    {
                        //옛날에 선택했던것과 다르다면

                        DeleteSelectionQuad();
                        CreateSelectionQuad(ref charTransform);


                    }
                    //Target();
                }
            }
            else 
            {
                // 선택퇸 오브젝트가 있지만 피킹 광선에 벗어날 경우.
                if (SelectedCharObject)
                {

                    DeleteSelectionQuad();
                    SelectedCharObject = null;                   
                }
            }

            SelectHeroTarget();

        }       
    }
    void SelectHeroTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Enemy"))
            {

                Transform charTransform = hit.transform;
                if (!HeroTargetObject)
                {
                    //아직 선택하지 않았다면
                    CreateSelectionTargetQuad(ref charTransform);

                }
                else
                {
                    //이미 선택된 객체가 있을 경우
                    if (HeroTargetObject != charTransform.gameObject)
                    {
                        //옛날에 선택했던것과 다르다면

                        DeleteSelectionTargetQuad();
                        CreateSelectionTargetQuad(ref charTransform);


                    }  
                }
            }
        }
    }




    void AddSelectionQuad()
    {
        Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
        charCtrl.SelectionQuqdObject = Instantiate(SelectionStatusQuadPrefab);
        charCtrl.SelectionQuqdObject.transform.SetParent(charCtrl.transform, false);
        charCtrl.SelectionQuqdObject.transform.position = new Vector3(charCtrl.SelectionQuqdObject.transform.position.x, 0.001f, charCtrl.SelectionQuqdObject.transform.position.z);
        charCtrl.SelectionQuqdObject.transform.eulerAngles = new Vector3(90, 0, 0);


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

        charCtrl.SelectionQuqdObject.transform.localScale = scale;
    }
    void DeleteSelectionQuad()
    {
        charCtrl = SelectedCharObject.GetComponent<CharController>();
        if (charCtrl.SelectionQuqdObject)
        {
            Destroy(charCtrl.SelectionQuqdObject);
        }
    }
    void CreateSelectionQuad(ref Transform charTransform)
    {
        SelectedCharObject = charTransform.gameObject;
        charCtrl = SelectedCharObject.GetComponent<CharController>();
        if (charCtrl.SelectionQuqdObject == null)
        {
            AddSelectionQuad();
        }
    }
  

    void Target()
    {
        if (Input.GetMouseButtonDown(0) && charCtrl.charType == CHAR_TYPE.ENEMY)
        {
           
        }

             
    }



    void CreateSelectionTargetQuad(ref Transform charTransform)
    {
        HeroTargetObject = charTransform.gameObject;
        charCtrl = HeroTargetObject.GetComponent<CharController>();
        if (charCtrl.SelectionHeroTargetQuadObject == null)
        {
            DeleteSelectionQuad();
            AddSelectionTargetQuad();
        }
    }
    void AddSelectionTargetQuad()
    {
        Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
        charCtrl.SelectionHeroTargetQuadObject = Instantiate(SelectionTargetQuadPrefab);
        charCtrl.SelectionHeroTargetQuadObject.transform.SetParent(charCtrl.transform, false);
        charCtrl.SelectionHeroTargetQuadObject.transform.position = new Vector3(charCtrl.SelectionQuqdObject.transform.position.x, 0.001f, charCtrl.SelectionQuqdObject.transform.position.z);
        charCtrl.SelectionHeroTargetQuadObject.transform.eulerAngles = new Vector3(90, 0, 0);






        switch (charCtrl.charSize)
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

        charCtrl.SelectionHeroTargetQuadObject.transform.localScale = scale;
    }
    void DeleteSelectionTargetQuad()
    {
        charCtrl = HeroTargetObject.GetComponent<CharController>();
        if (charCtrl.SelectionHeroTargetQuadObject)
        {
            Destroy(charCtrl.SelectionHeroTargetQuadObject);
        }
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


