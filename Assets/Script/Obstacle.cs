using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Obstacle : MonoBehaviour, IPoolObject
{
    public string obstacleType;

    public bool isEnable;

    public Vector3 placePos;

    //컴포넌트 관련
    public GameManager gameManager;
    SpriteRenderer spriteRenderer;

    //이미지 관련
    [SerializeField] Sprite[] hurdleSprites;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Return();
    }

    void Return()
    {
        if (gameManager != null && isEnable)
        {
            float distanceX = transform.position.x - gameManager.player.transform.position.x;

            if (distanceX < -7)
                gameManager.ReturnPool(this);
        }
    }

    public void OnCreatedInPool()
    {
        
    }

    public void OnGettingFromPool()
    {
        if (obstacleType == "Jump")
        {
            int ranIndex = Random.Range(0, hurdleSprites.Length);
            spriteRenderer.sprite = hurdleSprites[ranIndex];
        }

        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        if (collider.enabled == false)
            collider.enabled = true;

        isEnable = true;
    }
}
