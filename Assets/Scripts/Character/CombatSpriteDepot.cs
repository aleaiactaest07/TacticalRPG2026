using System.Collections.Generic;
using UnityEngine;

public class CombatSpriteDepot : ScriptableObject
{
    [SerializeField] List<Sprite> idle;
    [SerializeField] List<Sprite> meleeBase;
}