using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The graphical frontend of a combat unit.
/// </summary>
public class FieldCharacter : MonoBehaviour, ObjectHP
{
    #region Fields
    private OverlayTile tilePosition;
    private OverlayTile targetTile;
    [SerializeField] List<OverlayTile> movementOrders;
    public string internalCharacterName { get; private set; }
    public string unitDisplayName {get; private set;} //determined from province of recruitment probably? like 2nd Dolebin Archers
    [SerializeField] CharacterSpriteHandler spriteHandler;
    public CharacterSpriteHandler SpriteHandler => spriteHandler;
    [SerializeField] UnitBase defaultBase;
    private Unit unit;
    public int MaxUnitHP => unit.vitality;
    public int UnitHP {get; private set;}
    public bool PlayerControlled; //make private set later
    private static Vector2 positionalOffset = new Vector2(0, 0.25f); //change to make it so character positions reflect properly on the isometric grid
    #endregion
    #region Setup
    //TODO: change in place of selecting deployment area later in development
    void Start()
    {
        SetupUnit(defaultBase, 4 * Vector2.one);
    }
    public void SetupUnit(UnitBase uBase, Vector2 tilePosition)
    {
        unit = new Unit(uBase);
        internalCharacterName = uBase.UnitName; //bubbled up 
        spriteHandler.Setup(uBase.CombatSprites);

        UnitHP = MaxUnitHP; //TODO: change to be carryover from campaign map

        SetTilePosition(tilePosition);
    }
    #endregion
    #region Tile Movement
    public void SetTilePosition(Vector2 tilePosition)
    {
        // null check for map manager
        if (MapManager.i == null)
        {
            Debug.LogError("MapManager instance not found when setting up field character.");
            return;
        }

        //check to see if a tile exists at this position
        if (!MapManager.i.TryGetOverlayTile(tilePosition, out OverlayTile currentTile))
        {
            Debug.LogError($"No overlay tile found at tile position {tilePosition}.");
            return;
        }

        this.tilePosition = currentTile;
        targetTile = currentTile;
        
        if(currentTile != null)
            currentTile.SetRestingObject(this); //set the current tile as its resting object
        
        transform.position = (Vector2)currentTile.transform.position + positionalOffset;
    }
    /// <summary>
    /// Receives a list of overlay tiles for movement, and then calls FollowPath to lerp to each of those tiles.
    /// </summary>
    /// <param name="tiles"></param>
    public IEnumerator setMoveOrders(List<OverlayTile> tiles)
    {
        movementOrders = tiles;
        yield return FollowPath();
    }
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private AnimationCurve movementEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private IEnumerator FollowPath(){ //coroutine to follow a path
        if (movementOrders == null || movementOrders.Count == 0)
        {
            yield break; //early termination
        }

        foreach (var nextTile in movementOrders)
        {
            targetTile = nextTile;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = nextTile.transform.position + (Vector3)positionalOffset;
            float moveDuration = Vector3.Distance(startPosition, targetPosition) / moveSpeed;
            if (moveDuration <= 0f)
            {
                transform.position = targetPosition;
                tilePosition = nextTile;
                continue;
            }

            float elapsedTime = 0f;

            while (elapsedTime < moveDuration) //the actual lerp
            {
                float normalizedTime = Mathf.Clamp01(elapsedTime / moveDuration);
                transform.position = Vector3.Slerp(startPosition, targetPosition, movementEase.Evaluate(normalizedTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            tilePosition = nextTile;
        }

        movementOrders.Clear();
    }
    #endregion
    #region MapObject and HP implementation
    public string exposeObjectInfo(out Sprite windowSprite, out string description, out float healthPercentage, out int maxHealth)
    {
        windowSprite = unit._base.PortraitSprite;
        description = unit._base.UnitDescription;
        healthPercentage = (float)UnitHP / (float) MaxUnitHP;
        maxHealth = (int)MaxUnitHP;
        return unit._base.UnitName;
    }
    public IEnumerator TakeDamage(int taken)
    {
        throw new NotImplementedException();
    }

    public bool AllowPassthrough(FieldCharacter passing)
    {
        return passing.PlayerControlled == this.PlayerControlled; //if both are playercontrolled of both are not.
    }

    private void FieldUnitDeath(){
        
    }

    #endregion
}