using UnityEngine;
using UnityEngine.UI;

public class NodeButton : MonoBehaviour {

    public GameObject nodeList;
    public GameObject backButton;
    public Text nodeName;
    public Text nodeSpeed;

    private void OnEnable()
    {
        nodeList.SetActive(false);
        backButton.SetActive(false);
    }

    public void OnClickNodeButton()
    {
        nodeList.SetActive(true);
        backButton.SetActive(true);
    }

    public void OnClickNode(int nodeNum)
    {
        switch((NODE_LIST)nodeNum)
        {
            case NODE_LIST.ITAM:
                nodeName.text = "Node - " + NODE_LIST.ITAM.ToString();
                nodeSpeed.text = "10 ms";
                break;

            case NODE_LIST.EOSYS:
                nodeName.text = "Node - " + NODE_LIST.EOSYS.ToString();
                nodeSpeed.text = "50 ms";
                break;

            case NODE_LIST.EOSeoul:
                nodeName.text = "Node - " + NODE_LIST.EOSeoul.ToString();
                nodeSpeed.text = "90 ms";
                break;

            default:
                break;
        }

        nodeList.SetActive(false);
        backButton.SetActive(false);
    }
}

public enum NODE_LIST
{
    ITAM = 0,
    EOSYS = 1,
    EOSeoul = 2,
}
