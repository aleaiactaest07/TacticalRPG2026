using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    private PathFinder pathFinder;

    private OverlayTile cachedTileTest;

    private OverlayTile rightClickedTile;
    private OverlayTile movementSourceTile;

    void Awake()
    {
        pathFinder = new PathFinder();
    }
    void Update()
    {

    }

    void LateUpdate()
    {
        if (Mouse.current == null || Camera.main == null)
        {
            return;
        }

        var focusedTileHit = GetFocusedOnTile();

        if (focusedTileHit.HasValue)
        {
            GameObject overlayTile = focusedTileHit.Value.collider.gameObject;
            transform.position = overlayTile.transform.position;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                cachedTileTest = overlayTile.GetComponent<OverlayTile>(); //assign the cached tile test
                cachedTileTest?.ShowTile();

                movementSourceTile = overlayTile.GetComponent<OverlayTile>();
            }
            if (Mouse.current.rightButton.wasReleasedThisFrame && cachedTileTest != null)
            {
                //generate a path from the cursorHighlightSprite.position to the clicked position
                rightClickedTile = overlayTile.GetComponent<OverlayTile>();
                var path = pathFinder.FindPath(cachedTileTest, rightClickedTile);

                // foreach(var tile in path)
                // {
                //     tile.ShowTile();
                // }

                MapManager.i.drawPathfindingArrows(path); //draw the pathfinding arrows
            }
        }
    }

    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    private void PlaceCharacterOnTile(OverlayTile tile, FieldCharacter character)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.25f, tile.transform.position.z);
    }
}