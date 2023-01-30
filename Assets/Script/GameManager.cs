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
    public Transform[] backGround;
    public MapInfo[] mapInfos;

    PoolManager poolManager;
    string[] mapType = { "Idle", "Jump", "Slide"};

    void Awake()
    {
        poolManager = GetComponent<PoolManager>();
    }

    void Update()
    {
        MapCheck();
        BackCheck();
    }

    void MapCheck() { 
        float distance = player.transform.position.x - mapInfos[curMapIndex].transform.position.x;

        if (mapInfos[curMapIndex].length / 2 - distance < 16)
        {
            int ranIndex = Random.Range(0, 100);
            int ranMapType;

            if (ranIndex >= 90)
                ranMapType = 0;
            else if (ranIndex >= 45)
                ranMapType = 1;
            else
                ranMapType = 2;

            PlaceMap();
            PlaceObstcle(ranMapType);
            PlaceItem(ranMapType);
        }
    }

    void PlaceMap()
    {
        curMapIndex = curMapIndex == 0 ? 1 : 0;

        mapInfos[curMapIndex].transform.position = curMapIndex == 0 ? new Vector3(mapInfos[1].transform.position.x + mapInfos[1].length / 2 + mapInfos[1].length / 2, 0, 0) : new Vector3(mapInfos[0].transform.position.x + mapInfos[0].length / 2 + mapInfos[0].length / 2, 0, 0);
    }

    void PlaceObstcle(int mapTypeIndex)
    {
        if (!isStart || mapType[mapTypeIndex] == "Idle")
            return;

        Obstacle newObstacle;

        if (mapType[mapTypeIndex] == "Jump")
            newObstacle = poolManager.GetFromPool<Obstacle>("Jump");
        else
            newObstacle = poolManager.GetFromPool<Obstacle>("Slide");

        newObstacle.transform.position = new Vector3(mapInfos[curMapIndex].transform.position.x, mapInfos[curMapIndex].transform.position.y + newObstacle.placePos.y, -1);
        newObstacle.gameManager = this;
    }

    void PlaceItem(int mapTypeIndex)
    {
        if (!isStart)
            return;

        if (mapType[mapTypeIndex] == "Jump")
            foreach (Transform spawnPos in mapInfos[curMapIndex].jumpItemSpawnPos)
                SetItemPosition(spawnPos);
        else if (mapType[mapTypeIndex] == "Slide")
            foreach (Transform spawnPos in mapInfos[curMapIndex].SlideItemSpawnPos)
                SetItemPosition(spawnPos);
        else if (mapType[mapTypeIndex] == "Idle")
        {
            int ranType = Random.Range(0, 2);
            if (ranType == 0)
                foreach (Transform spawnPos in mapInfos[curMapIndex].jumpItemSpawnPos)
                    SetItemPosition(spawnPos);
            else
                foreach (Transform spawnPos in mapInfos[curMapIndex].SlideItemSpawnPos)
                    SetItemPosition(spawnPos);
        }
    }

    void SetItemPosition(Transform spawnPos)
    {
        Item newItem = Spawn();
        if (newItem != null)
        {
            newItem.transform.position = spawnPos.position + mapInfos[curMapIndex].transform.position;
            newItem.Init();
            newItem.manager = this;
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

    Item Spawn()
    {
        int ran = Random.Range(0, 100);

        if (ran > 20)
        {
            return null;
        }
        else if (ran > 10)
        {
            Item newItem = poolManager.GetFromPool<Item>("Bone");
            return newItem;
        }
        else
        {
            Item newItem = poolManager.GetFromPool<Item>("Chur");
            return newItem;
        }
    }

    public void ReturnPool(Item clone)
    {
        poolManager.TakeToPool<Item>(clone.itemType, clone);
        clone.isEnable = false;
    }

    public void ReturnPool(Obstacle clone)
    {
        poolManager.TakeToPool<Obstacle>(clone.obstacleType, clone);
        clone.isEnable = false;
    }
}
