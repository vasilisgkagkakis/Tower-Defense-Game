using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("Combat")]
    public float damage;
    public bool isAreaDamage;
    public float fireInterval = 1f; // Seconds between shots
    
    [Header("Economy")]
    public int baseCost = 100;           // Cost to buy this turret
    public int[] upgradeCosts = {50};    // Cost for each upgrade level
    
    [Header("Turret Info")]
    public string turretName = "Basic Turret";
    public string description = "A basic defensive turret";
    
    public enum TargetType
    {
        First,
        Last,
        Close
    }
    
    // Calculate sell value (75% of total investment)
    public int CalculateSellValue(int totalInvestment)
    {
        return Mathf.RoundToInt(totalInvestment * 0.75f);
    }
}