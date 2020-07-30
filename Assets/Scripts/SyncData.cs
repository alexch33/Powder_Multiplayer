using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SyncData : MonoBehaviour
{
    public Vector2Int[] positions;

    public int[] scores;

    public BitArray mapData;

    public static object Deserialize(byte[] bytes)
    {
        SyncData data = new SyncData();

        int players = (bytes.Length - 20 * 10 / 8) / 12;

        data.positions = new Vector2Int[players];
        data.scores = new int[players];

        for (int i = 0; i < players; i++)
        {
            data.positions[i].x = BitConverter.ToInt32(bytes, 8 * i);
            data.positions[i].y = BitConverter.ToInt32(bytes, 8 * i + 4);
            data.scores[i] = BitConverter.ToInt32(bytes, 8 * players + 4 * i);
        }

        byte[] mapBytes = new byte[20 * 10 / 8];
        Array.Copy(bytes, players * 12,mapBytes, 0, mapBytes.Length);

        data.mapData = new BitArray(mapBytes);

        return data;
    }

    public static byte[] Serialize(object obj)
    {
        SyncData data = (SyncData)obj;

        byte[] result = new byte[
            8 * data.positions.Length +
            4 * data.scores.Length +
            Mathf.CeilToInt(data.mapData.Count / 8f)
        ];

        for (int i = 0; i < data.positions.Length; i++)
        {
            BitConverter.GetBytes(data.positions[i].x).CopyTo(result, 8 * i);
            BitConverter.GetBytes(data.positions[i].y).CopyTo(result, 8 * i + 1);
        }

        for (int i = 0; i < data.scores.Length; i++)
        {
            BitConverter.GetBytes(data.scores[i]).CopyTo(result, 8 * data.scores.Length + 4 * i);
        }

        data.mapData.CopyTo(result, 8 * data.positions.Length + 4 * data.scores.Length);

        return result;
    }
}
