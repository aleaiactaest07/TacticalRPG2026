using System.Collections.Generic;
using UnityEngine;

public class FieldArmy : MonoBehaviour
{
    private List<CampaignUnit> units;
    public Region currentRegion {get; private set;}

    /// <summary>
    /// updates the current region the unit is in.
    /// </summary>
    /// <param name="newRegion"></param>
    public void UpdateCurrentRegion(Region newRegion)
    {
        currentRegion = newRegion;    
    }

    /// <summary>
    /// sums up the individual maintenance costs of every unit in the army.
    /// </summary>
    /// <returns></returns>
    public float getUnitMaintenance()
    {
        float totalMaintenance = 0f;

        foreach(var unit in units)
        {
            totalMaintenance += unit.GetUnitMaintenance();
        }

        return totalMaintenance;
    }
}

/// <summary>
/// stores the details of a single unit in an army for use on the campaign map
/// </summary>
public class CampaignUnit
{
    private UnitArmor armor;
    private float unitHealth; //health of the unit after various battles
    private UnitBase attachedUnit; //the associated unitBase for this CampaignUnit

    /// <summary>
    /// setter for the unit's armor.
    /// </summary>
    /// <param name="armor"></param>
    public void updateArmor(UnitArmor armor)
    {
        this.armor = armor;
    }

    /// <summary>
    /// setter for the new unit's health (percentage from 0 -> 1)
    /// </summary>
    /// <param name="newHealth"></param>
    public void updateUnitHealth(float newHealth)
    {
        this.unitHealth = newHealth;
    }

    public CampaignUnit(UnitArmor startingArmor, UnitBase attachedUnit)
    {
        this.unitHealth = 1f; //constructing a unit 
    }

    public float GetUnitMaintenance()
    {
        return (float)attachedUnit.UnitMaintenance * unitHealth;
    }
}