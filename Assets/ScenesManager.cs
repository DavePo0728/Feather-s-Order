using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ScenesManager : MonoBehaviour
{
    private void Awake()
    {

    }
    private void Update()
    {
        
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void GetStartInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartGame();
        }
    }
    public void GetReloadInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ReloadScene();
        }
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(1);
    }
}
