using System.Collections.Generic;
using UnityEngine;

public class CampaignMapManager : MonoBehaviour
{
    public static CampaignMapManager i;
    [SerializeField] List<Region> regions;
    [SerializeField] List<MapFactionBase> factionBases;
    private List<MapFaction> factions;
    private MapFaction playerFaction;

    private Region highlightedRegion;

    #region Helper Scripts
    [SerializeField] CampaignUI campaignUI;
    #endregion

    void Awake()
    {
        if(i==null)
            i = this;
    }

    void Update()
    {
        //handles the highlightedregion logic

    }

    public void UpdateRegion(Region region)
    {
        highlightedRegion = region;
        campaignUI.updateHighlightedRegionUI(region);
    }

    /// <summary>
    /// sets up a campaign, passing the factionCode of the player faction.
    /// </summary>
    /// <param name="playerFaction"></param>
    public void SetupCampaign(string playerFaction)
    {
        i = this;

        factions = new List<MapFaction>();
        mapRegions = new Dictionary<string, Region>();
        factionDictionary = new Dictionary<string, MapFaction>();

        foreach(var region in regions)
        {
            region.InitializeRegion();
            mapRegions.Add(region.RegionCode, region);
        }
        
        foreach(var faction in factionBases)
        {
            MapFaction mFaction = new MapFaction(faction); //construct a new faction from its base

            //check if this faction is the player faction
            if (playerFaction.Equals(faction.FactionTag))
            {
                this.playerFaction = mFaction; //store it as the playerFaction in memory
            }

            factions.Add(mFaction); //add it to the factions list
            factionDictionary.Add(mFaction.fBase.FactionTag, mFaction);//also add it to the faction dictionary

            //now assign owned regions to the faction
            foreach(string region in mFaction.fBase.StartingRegions)
            {
                mFaction.OccupyRegion(mapRegions[region]); //update using the dictionary
            }
        }
    }

    /// <summary>
    /// Dictionary that returns a Region from its regionCode.
    /// </summary>
    private Dictionary<string, Region> mapRegions;

    /// <summary>
    /// Dictionary that returns a MapFaction from its factionCode. 
    /// </summary>
    private Dictionary<string, MapFaction> factionDictionary;
}