using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFollow : MonoBehaviour
{
    public Player player;
    public float speedRatio;

    void FixedUpdate()
    {
        if (player.isFall) return;

        transform.position = new Vector3(transform.position.x + player.speed * speedRatio * Time.smoothDeltaTime, 0, 10);
    }
}
