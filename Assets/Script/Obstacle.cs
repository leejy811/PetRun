using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Obstacle : MonoBehaviour, IPoolObject
{
    public string obstacleType;
    public bool isEnable;

    public Vector3 placePos;
    public GameManager gameManager;

    void Update()
    {
        Return();
    }

    void Return()
    {
        if (gameManager != null && isEnable)
        {
            float distanceX = transform.position.x - gameManager.player.transform.position.x;

            if (distanceX < -5)
                gameManager.ReturnPool(this);
        }
    }

    public void OnCreatedInPool()
    {
        
    }

    public void OnGettingFromPool()
    {
        isEnable = true;
    }
}
