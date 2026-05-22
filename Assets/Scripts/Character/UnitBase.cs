using UnityEngine;

/// <summary>
/// UnitBase is the data container for combat units. The Unit class itself allows for functions to be called (stat calculations) that use this data.
/// </summary>
[CreateAssetMenu(menuName = "Unit/Create New Unit")]
public class UnitBase : ScriptableObject
{
    [SerializeField] string unitName;
    [SerializeField] UnitType unitType;
    [SerializeField] CombatSpriteDepot combatSprites;

    [Header("Panel View Data")]
    [TextArea(3,5)] [SerializeField] string unitDescription;
    [SerializeField] Sprite portraitSprite;

    [SerializeField] int vitality;
    [SerializeField] int agility;
    [SerializeField] int strength;
    [SerializeField] int dexterity;
    [SerializeField] int tactics;

    //getters
    public string UnitName {get {return unitName;}}
    public UnitType UnitType => unitType;
    public CombatSpriteDepot CombatSprites {get {return combatSprites;}}
    public string UnitDescription {get {return unitDescription;}}
    public Sprite PortraitSprite {get {return portraitSprite;}}

    public int Vitality {get {return vitality;}}
    public int Agility {get {return agility;}}
    public int Strength {get {return strength;}}
    public int Dexterity {get {return dexterity;}}
    public int Tactics {get {return tactics;}}
}

public enum UnitType
{
    Melee,
    Ranged
}