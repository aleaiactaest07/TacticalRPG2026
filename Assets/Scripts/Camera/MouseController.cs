using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    #region fields
    public static MouseController i;
    private PathFinder pathFinder;

    private OverlayTile hoveredTile; //the tile that the mouse is currently hovering over
    [SerializeField] private OverlayTile clickedTile; //the last tile that was left clicked
    public event Action<BattleState> updateBattleState;

    //unit movement fields
    public FieldCharacter characterToMove; //the last l-clicked fieldCharacter. Right clicking a tile will move it.
    private OverlayTile characterToMoveSource; //the tile which the characterToMove resides.
    #endregion

    void Awake()
    {
        i = this;
        pathFinder = new PathFinder();
    }
    public void HandleUpdate(BattleState battleState)
    {
        if (Mouse.current == null || Camera.main == null)
        {
            return;
        }

        OverlayTile focusedTile = GetOverlayTileFromMousePos();

        if (focusedTile != null)
        {
            transform.position = focusedTile.transform.position;
            hoveredTile = focusedTile;

            HandleFocusedTile(battleState);
        }
    }
    private void HandleFocusedTile(BattleState battleState)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            clickedTile = hoveredTile;
            clickedTile?.ShowTile();

            //see if there is a resting object.
            if (clickedTile.RestingObject != null)
            {
                //if so, make it display in the worldObjectPreviewUI
                WorldObjectPreviewUI.i.displayObject(clickedTile.RestingObject);

                //if it is a fieldCharacter party-controlled, and the battlestate is SelectPartyMember, select it and update the battlemanager's state.
                updateBattleState?.Invoke(BattleState.UnitSelected);

                if (clickedTile.RestingObject is FieldCharacter fieldCharacter && fieldCharacter.PlayerControlled) //check if the resting object is a field character. If so, store its casted version as characterToMove
                {
                    characterToMove = fieldCharacter;
                    characterToMoveSource = clickedTile;
                }
                else
                {
                    //another tile was clicked on
                    characterToMove = null;
                    characterToMoveSource = null;
                }
            }
            else
            {
                WorldObjectPreviewUI.i.hideMenu();
            }
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame && hoveredTile != null)
        {
            //check if there is a chraracterToMove, and if this square is empty. Then move the unit if there is a path.
            //TODO: if the characterToMove is a ranged unit, then check line of sight logic or something. Would just be an else statement or something
            if (characterToMove != null && hoveredTile.RestingObject == null)
            {
                //make the move order for the characterToMove and make sure to handle the restingObject logic.
                StartCoroutine(MoveCharacter());
            }
        }
        else if (battleState == BattleState.CheckingLOS && clickedTile != null && hoveredTile != null)
        {
            MapManager.i.UpdateLOSIndicator(clickedTile, hoveredTile);
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame && battleState == BattleState.UnitSelected)
        {
            updateBattleState?.Invoke(BattleState.CheckingLOS);
        }
    }
    public OverlayTile GetOverlayTileFromMousePos()
    {
        var focusedTileHit = GetFocusedOnTile();
        if(focusedTileHit.HasValue)
        {
            GameObject overlayTile = focusedTileHit.Value.collider.gameObject;
            hoveredTile = overlayTile.GetComponent<OverlayTile>();

            return hoveredTile;
        }

        return null;
    }

    public OverlayTile GetOverlayTileFromPosition(Vector2 position)
    {
        var focusedTileHit = GetTileFromPos(position);
        
        if(focusedTileHit.HasValue)
        {
            GameObject overlayTile = focusedTileHit.Value.collider.gameObject;
            hoveredTile = overlayTile.GetComponent<OverlayTile>();

            return hoveredTile;
        }

        return null;
    }
    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        return GetTileFromPos(mousePosition);
    }

    /// <summary>
    /// Helper function for GetFocusedOnTile.
    /// </summary>
    /// <param name="mousePos"></param>
    /// <returns></returns>
    private RaycastHit2D? GetTileFromPos(Vector2 mousePos)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    /// <summary>
    /// Moves a field character on the grid. Uses a coroutine to display the A* pathfinding arrows correctly.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveCharacter()
    {
        List<OverlayTile> path = pathFinder.FindPath(characterToMoveSource, hoveredTile);

        //draw the pathfinding arrows and remove them after
        MapManager.i.drawPathfindingArrows(path);

        clickedTile.ClearRestingObject();
        hoveredTile.SetRestingObject(characterToMove);

        //move the character
        yield return characterToMove.setMoveOrders(path);

        //remove the pathfinding arrows
        MapManager.i.destroyPathfindingArrows();
        
        //null out leftover data
        clickedTile = null;
        characterToMove = null;
        characterToMoveSource = null;
    }
    
    
    /// <summary>
    /// Checks a square from clickdrag selection and returns the list of all overlay tiles.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public List<OverlayTile> HandleDragRange(Vector2 origin, Vector2 destination)
    {
        if (MapManager.i == null || MapManager.i.map == null)
        {
            return null;
        }

        Vector2 min = Vector2.Min(origin, destination);
        Vector2 max = Vector2.Max(origin, destination);

        List<OverlayTile> rectangleTiles = new List<OverlayTile>();

        foreach (OverlayTile tile in MapManager.i.map.Values)
        {
            Vector3 tileScreenPosition = Camera.main.WorldToScreenPoint(tile.transform.position);

            if (tileScreenPosition.x < min.x || tileScreenPosition.x > max.x)
            {
                continue;
            }

            if (tileScreenPosition.y < min.y || tileScreenPosition.y > max.y)
            {
                continue;
            }

            rectangleTiles.Add(tile);
        }

        return rectangleTiles; //returns the tiles that were highlighted that can be then filtered quickly using LINQ or something
    }
}