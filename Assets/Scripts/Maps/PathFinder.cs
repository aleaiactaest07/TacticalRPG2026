using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    //return a list of tiles a path must go through to reach a set target.
    public List<OverlayTile> FindPath(OverlayTile source, OverlayTile target)
    {
        List<OverlayTile> openList = new List<OverlayTile>(); //tiles we want to check in the next loop iteration
        List<OverlayTile> closedList = new List<OverlayTile>(); //tiles we do not need to recheck
        
        openList.Add(source);

        while(openList.Count > 0)
        {
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if(currentOverlayTile == target)
            {
                //found target tile, finalize path
                return GetFinishedList(source, target);
            }

            var neighborTiles = GetNeighborTiles(currentOverlayTile);

            foreach(var neighbor in neighborTiles)
            {
                if (neighbor.isBlocked || neighbor.RestingObject != null || closedList.Contains(neighbor)) //TODO: put elevation differences, as well as passthrough implementation for friendly units
                {
                    continue;   
                }

                neighbor.G = GetManhattanDistance(source, neighbor);
                neighbor.H = GetManhattanDistance(target, neighbor);

                neighbor.previous = currentOverlayTile;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
            }
        }

        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile source, OverlayTile target)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();

        OverlayTile current = target;

        while(current != source)
        {
            finishedList.Add(current);
            current = current.previous; //traverse the nodes
        }

        finishedList.Reverse();
        return finishedList;
    }

    private int GetManhattanDistance(OverlayTile source, OverlayTile neighbor)
    {
        return Mathf.Abs(source.gridLocation.x - neighbor.gridLocation.x) + Mathf.Abs(source.gridLocation.y - neighbor.gridLocation.y);
    }

    private List<OverlayTile> GetNeighborTiles(OverlayTile sourceTile)
    {
        var map = MapManager.i.map;
         
        List<OverlayTile> neighbors = new List<OverlayTile>();

        //Top
        Vector2Int locationToCheck = new Vector2Int(sourceTile.gridLocation.x, sourceTile.gridLocation.y + 1);

        if (map.ContainsKey(locationToCheck))
        {
            neighbors.Add(map[locationToCheck]);
        }

        //Bottom
        locationToCheck = new Vector2Int(sourceTile.gridLocation.x, sourceTile.gridLocation.y - 1);

        if (map.ContainsKey(locationToCheck))
        {
            neighbors.Add(map[locationToCheck]);
        }

        //Left
        locationToCheck = new Vector2Int(sourceTile.gridLocation.x - 1, sourceTile.gridLocation.y);

        if (map.ContainsKey(locationToCheck))
        {
            neighbors.Add(map[locationToCheck]);
        }

        //Right
        locationToCheck = new Vector2Int(sourceTile.gridLocation.x + 1, sourceTile.gridLocation.y);

        if (map.ContainsKey(locationToCheck))
        {
            neighbors.Add(map[locationToCheck]);
        }

        return neighbors;
    }
}
