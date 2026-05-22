using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BiomePalette : ScriptableObject
{
    [SerializeField] tileComposition groundComposition;
    
    [Header("Ground props")]
    [SerializeField] List<GameObject> rocks;
    [SerializeField] List<GameObject> tress;
    
}

[System.Serializable]
public class tileComposition
{
    public List<Tile> tiles;

    [Header("Composition percentages. Must add up to 100% and have same n as tiles")]
    public List<int> percentages;
}