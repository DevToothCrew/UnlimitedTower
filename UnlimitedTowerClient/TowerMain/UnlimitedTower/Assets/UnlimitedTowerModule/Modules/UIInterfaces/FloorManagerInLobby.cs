using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FloorManagerInLobby : MonoBehaviour {

    static public FloorManagerInLobby instance = null;

    // Interface prefab
    public GameObject prefabFloorButtonInterface;

    // Scrollable target content
    public Transform targetContent;

    //dic of towers
    private Dictionary<int, FloorButtonInterface> towersDic = new Dictionary<int, FloorButtonInterface>();

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private static int _debugIndex = 0;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddFloor(_debugIndex, "User" + _debugIndex, UnityEngine.Random.Range(0, int.MaxValue) + "");
            _debugIndex++;
        }
    }

    public void AddFloor(int floor, string userId, string score)
    {
        //create object into scrolable content 
        {
            var go = Instantiate(prefabFloorButtonInterface, targetContent);
            var fbi = go.GetComponent<FloorButtonInterface>();
            fbi.floor = floor;
            fbi.userId = userId;
            fbi.score = score;

            towersDic.Add(floor, fbi);
        }
        
        // Sorting in hierarchy
        foreach(Transform child in targetContent)
        {
            var fbi = child.GetComponent<FloorButtonInterface>();
            child.SetSiblingIndex(fbi.floor);
        }
    }

}
