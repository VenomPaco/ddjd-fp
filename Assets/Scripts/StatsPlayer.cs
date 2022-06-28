
using UnityEngine;

public class StatsPlayer : Stats
{
    private Inventory _inventory;

    public StatsDictionary levelUpBoosts;
    
    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
    }

    public override float GetStatValue(StatName stat)
    {
        return baseValues[stat] + _inventory.GetEquipmentStatBonus(stat) + levelUpBoosts[stat];
    }
}