using System.Collections.Generic;
using UnityEngine;

public class SoldierGroup
{
    private List<FieldCharacter> AssignedSoldiers = new List<FieldCharacter>();
    private bool inFormation = false;

    public int groupNumber {get; private set;}

    /// <summary>
    /// The grid of how the soldierGroup is assigned.
    /// </summary>
    private Vector2 groupDimension = new Vector2();

    public void AssignSoldier(FieldCharacter soldier)
    {
        if (!AssignedSoldiers.Contains(soldier))
        {
            AssignedSoldiers.Add(soldier);
        }
    }
    public void AssignSoldiers(List<FieldCharacter> soldiers)
    {
        foreach (var soldier in soldiers)
        {
            AssignSoldier(soldier);
        }
    }

    public void UpdateGroupNumber(int newNumber)
    {
        groupNumber = newNumber;
    }

    public SoldierGroup(int groupNumber, List<FieldCharacter> soldiers)
    {
        this.groupNumber = groupNumber;
        AssignSoldiers(soldiers);
    }
}
