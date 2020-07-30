using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;
public class SyncSerializer : MonoBehaviour
{
    public static object Deserialize(byte[] bytes)
    {
        return MessagePackSerializer.Deserialize<SyncData>(bytes);
    }

    public static byte[] Serialize(object obj)
    {
        SyncData syncData = (SyncData)obj;

        byte[] byte1s = MessagePackSerializer.Serialize<SyncData>(syncData);
        SyncData newPos = (SyncData)Deserialize(byte1s);

        return byte1s;
    }
}
