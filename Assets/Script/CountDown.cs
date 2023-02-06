using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : BackFollow
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("CountSpeed", 0);
    }

    void Update()
    {
        float distance = transform.position.x - player.transform.position.x;

        if (Mathf.Abs(distance) > 8)
            gameObject.SetActive(false);
    }

    void OnEnable()
    {
        anim.SetFloat("CountSpeed", 1);
    }
}
