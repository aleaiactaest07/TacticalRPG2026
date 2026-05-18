using System.Collections.Generic;
using UnityEngine;

public class FieldCharacter : MonoBehaviour
{
    [SerializeField] OverlayTile tilePosition;
    [SerializeField] OverlayTile targetTile;
    [SerializeField] List<Vector2> movementOrders;
    public string internalCharacterName {get; private set;}

    [SerializeField] CharacterSpriteHandler spriteHandler;

    private static Vector2 offset = Vector2.zero; //change to make it so positions reflect properly
}