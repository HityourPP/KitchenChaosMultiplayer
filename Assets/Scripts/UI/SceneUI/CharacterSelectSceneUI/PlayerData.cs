using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>,INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && colorId == other.colorId;
    }
    //INetworkSerializable 接口用于定义自定义的可序列化类型。
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
    }
}
