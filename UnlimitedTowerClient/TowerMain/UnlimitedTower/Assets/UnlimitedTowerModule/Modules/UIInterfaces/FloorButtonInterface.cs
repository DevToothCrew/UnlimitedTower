using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FloorButtonInterface : MonoBehaviour
{
    private readonly string _PREFAB_PATH = "Tower/";

    [SerializeField]
    private Text uiFloor;

    [SerializeField]
    private Text uiUserId;

    [SerializeField]
    private Text uiScore;

    private int _floor;
    public int floor {
        set
        {
            _floor = value;
            uiFloor.text = value.ToString() + "F";
        }
        get
        {
            return _floor;
        }
    }

    private string _userId;
    public string userId
    {
        set
        {
            _userId = value;
            uiUserId.text = value;
        }
        get
        {
            return _userId;
        }
    }

    private string _score;
    public string score
    {
        set
        {
            _score = value;
            uiScore.text = int.Parse(value).ToString("N0"); 
        }
        get
        {
            return _score;
        }
    }
}
