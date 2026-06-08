using System;
using UnityEngine;


[Serializable]
public class WaveDefinition
{
    [Header("Wave Info")]
    public string waveName;

    [Header("Enemy Counts")]
    public int gruntCount;
    public int shooterCount;
    public int tankCount;

    [Header("Rewards")]
    public WaveReward reward;
}