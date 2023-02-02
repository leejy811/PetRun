using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Item : MonoBehaviour, IPoolObject
{
    public string itemType;

    public float floatSpeed;
    public float floatRange;
    public float startY;
    public bool isEnable;

    public GameManager manager;

    AudioSource audioSource;

    void Update()
    {
        if (isEnable)
        {
           if (transform.position.y > startY + floatRange || transform.position.y < startY - floatRange)
                floatSpeed *= -1;

            transform.position = new Vector3(transform.position.x, transform.position.y + floatSpeed * Time.deltaTime, transform.position.z);

            Return();
        }
    }

    void Return()
    {
        if (manager != null)
        {
            float distanceX = transform.position.x - manager.player.transform.position.x;

            if (distanceX < -7)
            {
                manager.ReturnPool(this);
            }
        }
    }

    public void OnCreatedInPool()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void OnGettingFromPool()
    {
        isEnable = true;
    }

    public void Init()
    {
        startY = transform.position.y;
    }

    public void PlaySound()
    {
        audioSource.Play();
    }

    
}
