using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGroup : MonoSingleton<BulletGroup> {
    public Dictionary<string, GameObject> bullet = new Dictionary<string, GameObject>();
    public List<BulletData> bulletList = new List<BulletData>();

    [System.Serializable]
    public class BulletData
    {
        public string name;
        public GameObject bullet;
    }

    public void Awake()
    {
        foreach (BulletData bulletData in bulletList)
        {
            bullet.Add(bulletData.name, bulletData.bullet);
        }
    }
}