using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager i;

    public Dictionary<Vector2Int, OverlayTile> map;
    public OverlayTile overlayTilePrefab;
    public GameObject overlayParent;

    [SerializeField] Tilemap walkableMap;

    [SerializeField] GameObject pathArrowHead;
    [SerializeField] Transform arrowParent;
    [SerializeField] Color pathLineColor = new Color(0.2f, 0.85f, 1f, 1f);
    [SerializeField] float pathLineWidth = 0.2f;
    [SerializeField] string pathLineSortingLayer = "Default";

    void Awake()
    {
        if (i == null) i = this;
    }

    void Start()
    {
        BoundsInt bounds = walkableMap.cellBounds;
        Debug.Log("Looping through all tiles");

        map = new Dictionary<Vector2Int, OverlayTile>();

        //loop through all tiles
        for (int z = bounds.min.z; z < bounds.max.z; z++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z); //store the location of that tile
                    var tileKey = new Vector2Int(x,y);
                    if (walkableMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayParent.transform);
                        var cellWorldPosition = walkableMap.layoutGrid.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y - 0.125f, cellWorldPosition.z);
                        overlayTile.name = $"{x},{y}"; //for ease of viewing in the inspector
                        overlayTile.GetComponent<SpriteRenderer>().sortingLayerName = "Floor Highlight";
                        overlayTile.gridLocation = new Vector2Int(x,y); //insert the X and Y location

                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Given a list of overlay tiles, computes corner points and draws a dynamic path line.
    /// </summary>
    /// <param name="tiles"></param>
    public void drawPathfindingArrows(List<OverlayTile> tiles)
    {
        DestroyAllChildrenOfGameobject(arrowParent.gameObject);

        if (tiles == null || tiles.Count == 0)
        {
            return;
        }

        List<Vector2> cornerPoints = BuildPathCorners(tiles);
        DrawPathLine(cornerPoints);

        //temporarily disabling arrow head since it looks like ass
        //GameObject arrowHead = Instantiate(pathArrowHead, arrowParent);
        //arrowHead.transform.position = tiles[tiles.Count - 1].transform.position;
    }

    List<Vector2> BuildPathCorners(List<OverlayTile> tiles)
    {
        List<Vector2> corners = new List<Vector2>();
        corners.Add(tiles[0].transform.position);

        if (tiles.Count == 1)
        {
            return corners;
        }

        Vector2Int previousDirection = tiles[1].gridLocation - tiles[0].gridLocation;

        for (int i = 1; i < tiles.Count - 1; i++)
        {
            Vector2Int nextDirection = tiles[i + 1].gridLocation - tiles[i].gridLocation;

            if (nextDirection != previousDirection)
            {
                corners.Add(tiles[i].transform.position);
            }

            previousDirection = nextDirection;
        }

        corners.Add(tiles[tiles.Count - 1].transform.position);
        return corners;
    }
    void DrawPathLine(List<Vector2> cornerPoints)
    {
        if (cornerPoints.Count < 2)
        {
            return;
        }

        GameObject lineObject = new GameObject("PathLine");
        lineObject.transform.SetParent(arrowParent, false);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = pathLineColor;
        lineRenderer.endColor = pathLineColor;
        lineRenderer.startWidth = pathLineWidth;
        lineRenderer.endWidth = pathLineWidth;
        lineRenderer.sortingLayerName = pathLineSortingLayer;
        lineRenderer.useWorldSpace = true;
        lineRenderer.numCapVertices = 0;
        lineRenderer.numCornerVertices = 0;
        lineRenderer.positionCount = cornerPoints.Count;

        Vector3[] linePoints = new Vector3[cornerPoints.Count];
        for (int i = 0; i < cornerPoints.Count; i++)
        {
            linePoints[i] = new Vector3(cornerPoints[i].x, cornerPoints[i].y, 0f);
        }

        lineRenderer.SetPositions(linePoints);
    }
    static void DestroyAllChildrenOfGameobject(GameObject go)
    {
        for(int i = go.transform.childCount - 1; i >=0; i--)
        {
            Destroy(go.transform.GetChild(i).gameObject);
        }
    }
}