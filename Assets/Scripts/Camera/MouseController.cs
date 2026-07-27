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
    public List<FieldCharacter> selectedCharacters; //the list of selected fieldCharacters. Right clicking a tile will move the formation.
    private OverlayTile characterToMoveSource; //the tile which the characterToMove resides.
    #endregion

    void Awake()
    {
        i = this;
        pathFinder = new PathFinder();

        selectedCharacters = new List<FieldCharacter>();
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
                    SetSelectedSingleUnit(fieldCharacter);
                }
                else
                {
                    //another tile was clicked on
                    selectedCharacters.Clear();
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
            if (selectedCharacters.Count == 1 && hoveredTile.RestingObject == null)
            {
                StartCoroutine(MoveCharacter(selectedCharacters[0], hoveredTile));
            }
            else if (selectedCharacters.Count > 1)
            {
                StartCoroutine(MoveFormation(selectedCharacters, hoveredTile));
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
    /// Moves a field character on the grid. Uses a coroutine to display the A* pathfinding arrows correctly. Adding parameters allows for multiple movements simultaneously.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveCharacter(FieldCharacter toMove, OverlayTile destination)
    {
        List<OverlayTile> path = pathFinder.FindPath(toMove.TilePosition, destination);

        yield return MoveCharacterAlongPath(toMove, destination, path, true, null);
    }

    private IEnumerator MoveCharacterAlongPath(FieldCharacter toMove, OverlayTile destination, List<OverlayTile> path, bool drawPathArrows, Action onComplete)
    {
        if (path == null)
        {
            yield break;
        }

        bool isStationaryMove = path.Count == 0 && toMove.TilePosition == destination;
        if (path.Count == 0 && !isStationaryMove)
        {
            yield break;
        }

        if (drawPathArrows && path.Count > 0)
        {
            MapManager.i.drawPathfindingArrows(path);
        }

        toMove.TilePosition.ClearRestingObject();

        if (path.Count > 0)
        {
            yield return toMove.setMoveOrders(path);
        }

        destination.SetRestingObject(toMove);

        if (drawPathArrows && path.Count > 0)
        {
            MapManager.i.destroyPathfindingArrows();
        }

        onComplete?.Invoke();
    }

    private IEnumerator MoveFormation(List<FieldCharacter> units, OverlayTile destination)
    {
        if (!TryBuildFormationMovePlan(units, destination, out List<FormationMovePlan> movePlan))
        {
            yield break;
        }

        foreach (var unit in units)
        {
            unit.TilePosition.ClearRestingObject();
        }

        int remainingMoves = movePlan.Count;
        foreach (var move in movePlan)
        {
            StartCoroutine(MoveCharacterAlongPath(move.Unit, move.TargetTile, move.Path, false, () => remainingMoves--));
        }

        while (remainingMoves > 0)
        {
            yield return null;
        }
    }

    private bool TryBuildFormationMovePlan(List<FieldCharacter> units, OverlayTile destination, out List<FormationMovePlan> movePlan)
    {
        movePlan = new List<FormationMovePlan>();

        if (units == null || units.Count == 0 || destination == null || MapManager.i == null)
        {
            return false;
        }

        HashSet<OverlayTile> selectedTiles = new HashSet<OverlayTile>(units.Where(unit => unit != null && unit.TilePosition != null).Select(unit => unit.TilePosition));

        FieldCharacter originUnit = units
            .Where(unit => unit != null && unit.TilePosition != null)
            .OrderBy(unit => GetTileDistance(unit.TilePosition, destination))
            .FirstOrDefault();

        if (originUnit == null)
        {
            return false;
        }

        Vector2Int originLocation = originUnit.TilePosition.gridLocation;

        foreach (FieldCharacter unit in units)
        {
            if (unit == null || unit.TilePosition == null)
            {
                return false;
            }

            Vector2Int offset = unit.TilePosition.gridLocation - originLocation;
            Vector2Int targetLocation = destination.gridLocation + offset;

            if (!MapManager.i.TryGetOverlayTile(targetLocation, out OverlayTile targetTile))
            {
                return false;
            }

            if (!IsFormationTargetAvailable(targetTile, selectedTiles))
            {
                return false;
            }

            List<OverlayTile> path = pathFinder.FindPath(unit.TilePosition, targetTile, selectedTiles);
            if (path.Count == 0 && unit.TilePosition != targetTile)
            {
                return false;
            }

            movePlan.Add(new FormationMovePlan
            {
                Unit = unit,
                TargetTile = targetTile,
                Path = path
            });
        }

        return true;
    }

    private bool IsFormationTargetAvailable(OverlayTile tile, HashSet<OverlayTile> selectedTiles)
    {
        if (tile == null || tile.isBlocked)
        {
            return false;
        }

        if (tile.RestingObject == null)
        {
            return true;
        }

        if (tile.RestingObject is FieldCharacter restingCharacter)
        {
            return selectedCharacters.Contains(restingCharacter) && selectedTiles.Contains(tile);
        }

        return false;
    }

    private int GetTileDistance(OverlayTile source, OverlayTile destination)
    {
        Vector2Int delta = source.gridLocation - destination.gridLocation;
        return Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
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
    
    public void SetSelectedSingleUnit(FieldCharacter unit)
    {
        selectedCharacters.Clear(); //if only a single unit is selected, clear the list and add the first one.

        selectedCharacters.Add(unit);
        characterToMoveSource = unit.TilePosition;
    }

    public void SetSelectedUnits(List<FieldCharacter> units)
    {
        selectedCharacters.Clear();
        selectedCharacters.AddRange(units.Where(unit => unit != null));
        characterToMoveSource = selectedCharacters.Count > 0 ? selectedCharacters[0].TilePosition : null;
    }

    public void ClearSelectedUnits()
    {
        selectedCharacters.Clear();
        characterToMoveSource = null;
    }

    private struct FormationMovePlan
    {
        public FieldCharacter Unit;
        public OverlayTile TargetTile;
        public List<OverlayTile> Path;
    }
}