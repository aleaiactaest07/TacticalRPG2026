using UnityEngine;

/// <summary>
/// Main controller for the flow of combat states. Make sure to outsource as much functionality as possible to prevent a script 2000+ lines long.
/// </summary>
public class BattleManager : MonoBehaviour
{
    public static BattleManager i;
    public BattleState battleState { get; private set; }
    void Awake()
    {
        if (i == null) i = this;
        MouseController.i.updateBattleState += UpdateBattleState;
    }

    void Update()
    {
        MouseController.i.HandleUpdate(battleState);
    }

    /// <summary>
    /// Setter for battle state. Not to be called directly (to avoid a circular dependency). Invoke an event instead.
    /// </summary>
    /// <param name="state"></param>
    private void UpdateBattleState(BattleState state)
    {
        battleState = state;
    }
}

public enum BattleState
{
    SelectUnit,
    UnitSelected
}