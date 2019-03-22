using UnityEngine;
using UnityEngine.UI;

public class NodeObject : MonoBehaviour
{
    public Text nameText;
    public Text speedText;

    public void OnEnable()
    {
        SetSpeed();
    }

    public void SetSpeed()
    {
        int speedValue = Random.Range(10, 300);
        Color speedTextColor;

        if(speedValue < 100)
        {
            speedTextColor = new Color(0, 255, 0);
        }
        else if(speedValue < 200)
        {
            speedTextColor = new Color(255, 255, 0);
        }
        else
        {
            speedTextColor = new Color(255, 0, 0);
        }

        speedText.text = speedValue + " ms";
        speedText.color = speedTextColor;
    }

    public void SetNodeText(NodeInfo nodeInfo)
    {
        nameText.text = nodeInfo.nodeName.ToString();
        speedText.text = nodeInfo.nodeSpeed.ToString() + " ms";
    }
}
