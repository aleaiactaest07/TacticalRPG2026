using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// The central handler for on-screen battle UI elements. Some are encapsulated into their own scripts (such as the GroupUI).
/// Currently some mechanical functionality is handled here, which calls for refactor later.
/// </summary>
public class BattleUI : MonoBehaviour
{
    Vector2 currentMousePos => Mouse.current.position.ReadValue();
    public static BattleUI i;
    private Vector2 selectorOrigin;
    List<FieldCharacter> selectedUnits = new List<FieldCharacter>();

    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
    }

    private bool isSelecting;

    [SerializeField] Image selectorBox;
    [SerializeField] private GroupUI groupUI;
    public void setSelecting(bool selecting)
    {
        isSelecting = selecting;
        selectorBox.gameObject.SetActive(selecting);

        if (selecting)
        {
            //place the origin
            placeSelectorOrigin(currentMousePos);
        }
        else
        {
            //the selection box was released, handle selection of units here. Find all tiles bound between the origin and mouse position and select all units in that region
            selectedUnits = highlightAllUnitsInRegion(MouseController.i.HandleDragRange(selectorOrigin, currentMousePos));

            if (selectedUnits.Count == 1)
            {
                MouseController.i.SetSelectedSingleUnit(selectedUnits[0]);
            }

            if (selectedUnits.Count > 0)
            {
                groupUI.updateNumberOfSelectedUnits(selectedUnits.Count);
            }
            else
            {
                groupUI.updateNumberOfSelectedUnits(0, true);
            }
        }
    }

    /// <summary>
    /// Places the anchored corner of the selector box (default is bottom left) in screenspace
    /// </summary>
    /// <param name="origin"></param>
    public void placeSelectorOrigin(Vector2 origin)
    {
        selectorOrigin = origin;
        selectorBox.rectTransform.position = origin;
    }

    /// <summary>
    /// Updates the opposite corner of the selector transform
    /// </summary>
    public void updateSelectorCorner()
    {
        Vector2 currentPosition = currentMousePos;
        Vector2 bottomLeft = new Vector2(Mathf.Min(selectorOrigin.x, currentPosition.x), Mathf.Min(selectorOrigin.y, currentPosition.y));
        Vector2 topRight = new Vector2(Mathf.Max(selectorOrigin.x, currentPosition.x), Mathf.Max(selectorOrigin.y, currentPosition.y));

        selectorBox.rectTransform.position = bottomLeft;
        selectorBox.rectTransform.sizeDelta = topRight - bottomLeft;
    }

    void Update()
    {
        if (isSelecting)
        {
            updateSelectorCorner();
        }
    }

    /// <summary>
    /// Given a list of overlaytiles, stores all player-controlled field characters in that region and enables the overlay highlight.
    /// </summary>
    /// <param name="tiles"></param>
    /// <returns></returns>
    private List<FieldCharacter> highlightAllUnitsInRegion(List<OverlayTile> tiles)
    {
        List<FieldCharacter> highlightedUnits = new List<FieldCharacter>();
        foreach (OverlayTile tile in tiles)
        {
            if (tile.RestingObject != null && tile.RestingObject is FieldCharacter)
            {
                FieldCharacter character = (FieldCharacter)tile.RestingObject;
                if (character.PlayerControlled)
                {
                    tile.ShowTile(); //highlight the tile if there is a resting object
                    highlightedUnits.Add(character);
                }
            }
        }

        return highlightedUnits;
    }

    /// <summary>
    /// Was prompted by the BattleKeyboardManager to attempt to group selected units into a new group. If there are no selected units, this will do nothing.
    /// </summary>
    public void AttemptGrouping()
    {
        if (selectedUnits.Count > 0)
        {
            groupUI.AddGroup(new SoldierGroup(selectedUnits));
            selectedUnits.Clear();
            groupUI.updateNumberOfSelectedUnits(0, true);

            Debug.Log("Attempting to group selected units.");
        }
    }
}