using TMPro;
using UnityEngine;

/// <summary>
/// Stores functions pertaining to campaign UI.
/// </summary>
public class CampaignUI : MonoBehaviour
{
    #region UI elements
    [SerializeField] TMP_Text regionText;
    #endregion
    public void updateHighlightedRegionUI(Region region)
    {
        regionText.text = $"{region.RegionName}";
    }
}