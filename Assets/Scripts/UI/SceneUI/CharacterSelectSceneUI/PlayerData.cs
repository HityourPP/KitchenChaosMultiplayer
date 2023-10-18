using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>,INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    //这里无法直接使用string类型，替换方法为使用固定字符串
    public FixedString64Bytes playerName;   
    public FixedString64Bytes playerId;   
    
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId
               && colorId == other.colorId
               && playerName == other.playerName
               && playerId == other.playerId;
    }
    //INetworkSerializable 接口用于定义自定义的可序列化类型。
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }
}
