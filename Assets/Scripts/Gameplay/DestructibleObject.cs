using System.Collections;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A prefab monobehavior for a destructible object. May inhibit movement. Has a late script execution order to prevent race condition with start on mapmanager.
/// </summary>
public class DestructibleObject : MonoBehaviour, ObjectHP
{
    private SpriteRenderer spriteRenderer => gameObject.GetComponent<SpriteRenderer>();
    [SerializeField] string objectName;
    [SerializeField] string objectDescription;
    [SerializeField] int objectHealth;
    [SerializeField] SpriteEffect destructionEffect;
    [SerializeField] Vector2 startingPosition; //TODO: remove this field when proc gen is added
    [SerializeField] Vector2 positionalOffset;
    [SerializeField] bool canWalkThrough; //if an object can be walked through in pathfinding
    private OverlayTile tilePosition;

    public string exposeObjectInfo(out Sprite windowSprite, out string description)
    {
        description = objectDescription;
        windowSprite = spriteRenderer.sprite;
        return objectName;
    }

    public void onLeftClick()
    {
        StartCoroutine(TakeDamage(20));
    }

    public IEnumerator TakeDamage(int taken)
    {
        objectHealth = math.max(0, objectHealth - taken);

        if (objectHealth == 0)
        {
            Debug.Log("Destructible object destroyed");
            //cause the destruction effect to take place on this sprite only
            yield return destructionEffect.runSpriteEffect(spriteRenderer);

            //destroy the object
            Destroy(this.gameObject);
        }
    }

    private void PlaceObjectOnGrid(Vector2 tilePosition)
    {
        if (!MapManager.i.TryGetOverlayTile(tilePosition, out OverlayTile currentTile))
        {
            Debug.LogError($"No overlay tile found at tile position {tilePosition}.");
            return;
        }

        this.tilePosition = currentTile;

        if (currentTile != null)
            currentTile.SetRestingObject(this); //set the current tile as its resting object

        transform.position = (Vector2)currentTile.transform.position + positionalOffset;
    }

    void Start()
    {
        PlaceObjectOnGrid(startingPosition);
    }

    public bool allowPassthrough(FieldCharacter passing)
    {
        return canWalkThrough;
    }
}