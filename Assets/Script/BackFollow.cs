using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFollow : MonoBehaviour
{
    [SerializeField] protected Player player;
    private float speedRatio = 0.9f;

    void FixedUpdate()
    {
        if (player.isFall) return;

        transform.position = new Vector3(transform.position.x + player.speed * speedRatio * Time.smoothDeltaTime, transform.position.y, transform.position.z);
    }
}
