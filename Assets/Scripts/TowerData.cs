using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/TowerData")]
public class TowerData : ScriptableObject
{
    public float damage;
    public bool isAreaDamage;
    public enum TargetType
    {
        First,
        Last,
        Close
    }
}