using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// The central handler for on-screen battle UI elements. Some are encapsulated into their own scripts (such as the GroupUI)
/// </summary>
public class BattleUI : MonoBehaviour
{
    Vector2 currentMousePos => Mouse.current.position.ReadValue();
    public static BattleUI i;

    private Vector2 selectorOrigin;
    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
    }

    private bool isSelecting;

    [SerializeField] Image selectorBox;
    [SerializeField] private GroupUI groupUI;
    public void setSelecting(bool selecting)
    {
        isSelecting = selecting;
        selectorBox.gameObject.SetActive(selecting);

        if(selecting)
        {
            //place the origin
            placeSelectorOrigin(currentMousePos);
        }
        else
        {
            //the selection box was released, handle selection of units here. Find all tiles bound between the origin and mouse position and select all units in that region.
            
        }
    }

    /// <summary>
    /// Places the anchored corner of the selector box (default is bottom left) in screenspace
    /// </summary>
    /// <param name="origin"></param>
    public void placeSelectorOrigin(Vector2 origin)
    {
        selectorOrigin = origin;
        selectorBox.rectTransform.position = origin;
    }

    /// <summary>
    /// Updates the opposite corner of the selector transform
    /// </summary>
    public void updateSelectorCorner()
    {
        Vector2 currentPosition = currentMousePos;
        Vector2 bottomLeft = new Vector2(Mathf.Min(selectorOrigin.x, currentPosition.x), Mathf.Min(selectorOrigin.y, currentPosition.y));
        Vector2 topRight = new Vector2(Mathf.Max(selectorOrigin.x, currentPosition.x), Mathf.Max(selectorOrigin.y, currentPosition.y));

        selectorBox.rectTransform.position = bottomLeft;
        selectorBox.rectTransform.sizeDelta = topRight - bottomLeft;
    }

    void Update()
    {
        if (isSelecting)
        {
            updateSelectorCorner();
        }
    }
}