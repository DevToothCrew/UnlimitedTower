using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NodeButton : MonoBehaviour {

    public GameObject nodeList;
    public GameObject backButton;

    public List<NodeInfo> nodeInfoList;

    public Text nodeName;

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
                nodeName.text = NODE_LIST.ITAM.ToString();
                break;

            case NODE_LIST.EOSYS:
                nodeName.text = NODE_LIST.EOSYS.ToString();
                break;

            case NODE_LIST.EOSeoul:
                nodeName.text = NODE_LIST.EOSeoul.ToString();
                break;

            default:
                break;
        }

        nodeList.SetActive(false);
        backButton.SetActive(false);
    }
}
