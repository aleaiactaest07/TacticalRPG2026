using UnityEngine;
using UnityEngine.InputSystem;

public class BattleKeyboardManager : MonoBehaviour
{
    InputSystem_Actions inputActions;
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable(); //enable keybinds

        inputActions.BattleBinds.ToggleGroup.performed += ctx => ToggleGroup(ctx);

        inputActions.BattleBinds.Select.performed += ctx => BattleUI.i.setSelecting(true);
        inputActions.BattleBinds.Select.canceled += ctx => BattleUI.i.setSelecting(false);
    }

    private void ToggleGroup(InputAction.CallbackContext ctx)
    {
        
    }
}