using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main controller for the flow of combat states. Make sure to outsource as much functionality as possible to prevent a script 2000+ lines long.
/// </summary>
public class BattleManager : MonoBehaviour
{
    public static BattleManager i;
    public BattleState battleState { get; private set; }
    private Stack<BattleState> currentStates;
    void Awake()
    {
        if (i == null) i = this;
        MouseController.i.updateBattleState += UpdateBattleState;
        currentStates = new Stack<BattleState>();
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
        currentStates.Push(state);
        battleState = currentStates.Peek();
    }

    private void PerformAttack(FieldCharacter attacker, ObjectHP receiver, AttackType attackType)
    {
        if(attackType == AttackType.Melee)
        {
            
        }
        else if(attackType == AttackType.Ranged)
        {
            
        }
    }

    /// <summary>
    /// Pops the current BattleState and restores the one before, allowing and undoing of the last action in battle.
    /// </summary>
    public void PopBattleState()
    {
        currentStates.Pop();
        battleState = currentStates.Peek(); //peek the top value of the stack.
    }

    /// <summary>
    /// Clears the stack of BattleStates, due to a new turn or irreversible action.
    /// </summary>
    public void ClearBattleStates()
    {
        currentStates.Clear();
    }
}

public enum BattleState
{
    SelectUnit,
    UnitSelected,
    CheckingLOS //checking ranged line of sight
}
/// <summary>
/// Whether or not an attack from a Unit is melee or ranged.
/// </summary>
public enum AttackType
{
    Melee,
    Ranged
}