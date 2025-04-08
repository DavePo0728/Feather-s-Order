using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Diagnostics.Contracts;

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
    public void GetSkipInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StopAllCoroutines();
            StartGame();
        }
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
    public void BackTotitle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(0);
        }
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(1);
    }
}
