using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class SerializationWrapper<T>
{
    public List<T> items;

    public SerializationWrapper(List<T> items)
    {
        this.items = items;
    }
}

[Serializable]
public class CharacterData
{
    public Vector3 position;
    public Quaternion rotation;
    public int classID;
    public bool isLeader;

    public CharacterData(Vector3 position, Quaternion rotation, int classID, bool isLeader)
    {
        this.position = position;
        this.rotation = rotation;
        this.classID = classID;
        this.isLeader = isLeader;
    }
}