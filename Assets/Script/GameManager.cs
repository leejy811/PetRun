using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int curMapIndex;
    public GameObject player;
    public GameObject[] maps;
    MapInfo[] mapInfos;

    void Awake()
    {
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
        }
    }
}
