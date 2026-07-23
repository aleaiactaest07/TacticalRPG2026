using System.Collections.Generic;
using UnityEngine;

public class GroupUI : MonoBehaviour
{
    public static GroupUI i;
    [SerializeField] private int groupCap = 9; //cap for the number of unit groups allowed at a time (1-9)
    [SerializeField] private GameObject graphicsParent;
    [SerializeField] private GameObject groupCardPrefab;

    private List<SoldierGroup> soldierGroups;
    void Awake()
    {
        if(i==null) i = this;
        soldierGroups = new List<SoldierGroup>();
    }

    public void AddGroup()
    {
        
    }
}