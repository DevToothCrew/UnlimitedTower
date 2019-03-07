using UnityEngine;
using UnityEngine.UI;

public class NodeObject : MonoBehaviour {

    public Image objectImage;

    public void OnEnable()
    {
        objectImage.color = new Color(255, 255, 255, 0);
    }

    public void NodeObjectOn()
    {
        objectImage.color = new Color(255, 255, 255, 100);
    }

    public void NodeObjectOff()
    {
        objectImage.color = new Color(255, 255, 255, 0);
    }
}
