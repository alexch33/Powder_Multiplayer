using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MessagePack;

[MessagePackObject]
public class SyncData : MonoBehaviour
{
    [Key(0)]
    public Vector2Int[] positions { get; set; }

    [Key(1)]
    public int[] scores { get; set; }

    [Key(2)]
    public bool[,] mapData { get; set; }
}
