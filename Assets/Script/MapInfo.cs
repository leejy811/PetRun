using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    public enum MapType { Idle, Jump, Slide}
    public MapType mapType;
    public float length;
}
