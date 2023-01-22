using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public int curMapIndex;
    public int curBackIndex;
    public float backGroundSize;
    public bool isStart;
    public GameObject player;
    public GameObject[] maps;
    public GameObject[] obstarcles;
    public Transform[] backGround;
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
        BackCheck();
        StartCheck();
    }

    void MapCheck() { 
        float distance = player.transform.position.x - maps[curMapIndex].transform.position.x;

        if (mapInfos[curMapIndex].length / 2 - distance < 16)
        {
            PlaceMap();
            PlaceItem();
        }
    }

    void PlaceMap()
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

        maps[curMapIndex].transform.position = new Vector3(maps[pastMapIndex].transform.position.x + mapInfos[pastMapIndex].length / 2 + mapInfos[curMapIndex].length / 2, 0, 0);
    }

    void PlaceItem()
    {
        if (!isStart)
            return;

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

    void BackCheck()
    {
        float distance = backGround[curBackIndex].position.x - player.transform.position.x;

        if (distance < 6)
        {
            curBackIndex = curBackIndex == 0 ? 1 : 0;
            backGround[curBackIndex].position = curBackIndex == 0 ? new Vector3(backGround[1].position.x + backGroundSize, 0, 10) : new Vector3(backGround[0].position.x + backGroundSize, 0, 10);
        }
    }

    void StartCheck()
    {
        foreach(GameObject obstarcle in obstarcles)
        {
            obstarcle.SetActive(isStart);
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
