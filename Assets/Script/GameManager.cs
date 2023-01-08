using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class GameManager : MonoBehaviour
{
    public int curMapIndex;
    public GameObject player;
    public GameObject[] maps;
    MapInfo[] mapInfos;
    PoolManager poolManager;

    void Awake()
    {
        poolManager = GetComponent<PoolManager>();
        mapInfos = new MapInfo[maps.Length];
        
        for (int i = 0; i < maps.Length; i++)
        {
            mapInfos[i] = maps[i].GetComponent<MapInfo>();
        }
    }

    void Update()
    {
        MapCheck();
    }

    void MapCheck() { 
        float distance = player.transform.position.x - maps[curMapIndex].transform.position.x;

        if (mapInfos[curMapIndex].length / 2 - distance < 16)
        {
            int ranIndex = 0;
            int pastMapIndex = 0;

            while (true)
            {
                ranIndex = Random.Range(0, maps.Length);

                if (ranIndex != curMapIndex)
                {
                    pastMapIndex = curMapIndex;
                    curMapIndex = ranIndex;
                    break;
                }
            }

            maps[curMapIndex].transform.position = new Vector3(maps[pastMapIndex].transform.position.x + mapInfos[pastMapIndex].length/2 + mapInfos[curMapIndex].length/2, 0, 0);

            foreach (Transform spawnPos in mapInfos[curMapIndex].itemSpawnPos)
            {
                Item newItem = Spawn();
                if (newItem != null)
                {
                    newItem.transform.position = spawnPos.position;
                    newItem.Init();
                    newItem.manager = this;
                }
            }
        }
    }

    Item Spawn()
    {
        int ran = Random.Range(0, 100);

        if (ran > 40)
        {
            return null;
        }
        else if (ran > 20)
        {
            Item newItem = poolManager.GetFromPool<Item>("Bone");
            return newItem;
        }
        else if (ran > 0)
        {
            Item newItem = poolManager.GetFromPool<Item>("Chur");
            return newItem;
        }
        else
        {
            Item newItem = poolManager.GetFromPool<Item>("Heart");
            return newItem;
        }
    }

    public void ReturnPool(Item clone)
    {
        poolManager.TakeToPool<Item>(clone.itemType, clone);
        clone.isEnable = false;
    }
}
