using UnityEngine;

public class Picking : MonoSingleton<Picking> {

 
    public GameObject SelectedCharObject = null;
    public GameObject SelectionStatusQuad;
    public GameObject SelectionTargetQuad;
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
                if (!SelectedCharObject) 
                {
                    //아직 선택하지 않았다면
                    // ChangeShader(transform, DEFINE.OUTLINE_SHADER);

              
                    SelectedCharObject = transform.gameObject;
                    charCtrl = SelectedCharObject.GetComponent<CharController>();

                    if (charCtrl.SelectionQuqdObject == null)
                    {
                        AddSelectionQuad();
                    }

                    Target();

                }
                else 
                {
                    //이미 선택된 객체가 있을 경우
                    if (SelectedCharObject != transform.gameObject) 
                    {
                        //옛날에 선택했던것과 다르다면
                        //ChangeShader(SelectedCharObject.transform, DEFINE.BASIC_SHADER);
                        //ChangeShader(hit.transform, DEFINE.OUTLINE_SHADER);

                        charCtrl = SelectedCharObject.GetComponent<CharController>();
                        if (charCtrl.SelectionQuqdObject)
                        {
                            DeleteSelectionQuad();
                        }

                        SelectedCharObject = transform.gameObject;
                        charCtrl = SelectedCharObject.GetComponent<CharController>();
                        if (charCtrl.SelectionQuqdObject == null)
                        {
                            AddSelectionQuad();
                        }
                       
                    }
                    Target();
                }
            }
            else 
            {
                if (SelectedCharObject)
                {
                    // ChangeShader(SelectedCharObject.transform, DEFINE.BASIC_SHADER);
                    charCtrl = SelectedCharObject.GetComponent<CharController>();
                    if (charCtrl.SelectionQuqdObject)
                    {
                        DeleteSelectionQuad();
                    }
                    SelectedCharObject = null;                   
                }
            }
        }
       
    }
    void SelectTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Enemy"))
                {



                }
            }
        }
    }


    void AddSelectionQuad()
    {
        Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
        charCtrl.SelectionQuqdObject = Instantiate(SelectionStatusQuad);
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
        Destroy(charCtrl.SelectionQuqdObject);
    }
  

    void Target()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Material mt = Resources.Load("Selection/TargetSelection_Material", typeof(Material)) as Material; ;
                                      
            charCtrl.SelectionQuqdObject.GetComponent<MeshRenderer>().material = mt;
            charCtrl.SelectTarget = true;
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


