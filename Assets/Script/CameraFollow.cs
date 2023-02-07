using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float offset;

    void Update()
    {
        transform.position = new Vector3(target.position.x + offset, transform.position.y, transform.position.z);
    }
}
