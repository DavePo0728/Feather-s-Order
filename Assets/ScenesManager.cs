using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ScenesManager : MonoBehaviour
{
    public void GetReloadInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ReloadScene();
        }
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
