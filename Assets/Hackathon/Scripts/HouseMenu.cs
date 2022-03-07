using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HouseMenu : MonoBehaviour
{
    public InputActionProperty showMenu;

    private void Start()
    {
        showMenu.action.performed += OpenMenu;
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        Debug.Log("Open");
    }
}
