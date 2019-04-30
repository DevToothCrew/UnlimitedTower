using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoidClick : MonoBehaviour
{
    public GameObject colorObj;
    Vector3 mpos;
    // Start is called before the first frame update 
    void Start()
    {

    }

    // Update is called once per frame 
    void Update()
    {
        mpos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(ScreenShotAndSpoid());
    }
    IEnumerator ScreenShotAndSpoid()
    {
        //스크린샷을 찍어, 그것을 Texture2D로 반환시키고 그곳에서 색을 추출!! 
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();

        //추출된 색 
        Color color = tex.GetPixel((int)mpos.x, (int)mpos.y);
        Debug.Log(color);
    }
}