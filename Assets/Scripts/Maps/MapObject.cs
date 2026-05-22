using UnityEngine;

/// <summary>
/// A MapObject is any object that sits on top of a tile. When a tile is clicked, it highlights the current mapobject, shows its object info, etc.
/// </summary>
public interface MapObject
{    
    /// <summary>
    /// Exposes data for the relevant popup menu formatted depending on the type of object (destructible wall, character, etc.). Also outputs a sprite for a windowed view UI
    /// </summary>
    /// <returns></returns>
    public string exposeObjectInfo(out Sprite windowSprite, out string description);
    /// <summary>
    /// Whether or not a passerby is allowed to walk through this object in pathfinding.
    /// </summary>
    /// <param name="passing"></param>
    /// <returns></returns>
    public bool allowPassthrough(FieldCharacter passing);
}