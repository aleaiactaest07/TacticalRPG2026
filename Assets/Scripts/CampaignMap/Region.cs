using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))] //require a spriterenderer on every Region.
public class Region : MonoBehaviour
{
    [SerializeField] private string regionCode;
    [SerializeField] private string regionName;
    [SerializeField] List<Region> borderingRegions; //the list of regions that are accessible to this one.
    public FieldArmy occupyingArmy {get; private set;}
    private SpriteRenderer regionRenderer;
    public string RegionCode => regionCode;
    public string RegionName => regionName;
    public MapFaction owner {get; private set;}
    
    /// <summary>
    /// sets up the region for use in the campaign map.
    /// </summary>
    public void InitializeRegion()
    {
        regionRenderer = GetComponent<SpriteRenderer>();

        regionRenderer.color = Color.forestGreen;
    }

    void Start()
    {
        InitializeRegion();
    }

    /// <summary>
    /// updates the color of the region's SpriteRenderer.
    /// </summary>
    /// <param name="color"></param>
    public void UpdateRegionColor(Color color)
    {
        regionRenderer.color = color;
    }

    /// <summary>
    /// setter for who owns this region.
    /// </summary>
    /// <param name="faction"></param>
    public void UpdateOwner(MapFaction faction)
    {
        owner.LoseRegion(this);
        owner = faction;

        UpdateRegionColor(owner.fBase.MapColor);
    }

    public void OnMouseEnter()
    {
        CampaignMapManager.i.UpdateRegion(this); //update the current highlighted region
    }
}

public enum RegionTerrain
{
    Grassland,
    Coastal,
    Mountain,
    Desert,
    Wetland,
    Village,
    City
}