using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroupUI : MonoBehaviour
{
    [SerializeField] private int groupCap = 9; //cap for the number of unit groups allowed at a time (1-9)
    [SerializeField] private GameObject graphicsParent;
    [SerializeField] private GameObject groupCardPrefab;
    [SerializeField] private TMP_Text unitsSelectedText;
    private List<SoldierGroup> soldierGroups = new List<SoldierGroup>();

    public void AddGroup(SoldierGroup newGroup)
    {
        if (soldierGroups.Count < groupCap)
        {
            soldierGroups.Add(newGroup);
             Instantiate(groupCardPrefab, graphicsParent.transform);
        }
    }

    /// <summary>
    /// Updates the interface based on if the player has click-dragged units.
    /// </summary>
    /// <param name="n"></param>
    /// <param name="cancel"></param>
    public void updateNumberOfSelectedUnits(int n, bool cancel = false)
    {
        unitsSelectedText.gameObject.SetActive(!cancel);
        unitsSelectedText.text = $"{n} units selected";
    }
}