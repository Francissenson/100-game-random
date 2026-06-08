using System;
using UnityEngine;

[Serializable]
public class RoomDefinition
{
    [Header("Room")]
    public string sceneName;

    public RoomType roomType;
}