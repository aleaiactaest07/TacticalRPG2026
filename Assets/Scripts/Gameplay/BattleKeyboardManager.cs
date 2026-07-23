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
    }

    private void ToggleGroup(InputAction.CallbackContext ctx)
    {
        
    }
}