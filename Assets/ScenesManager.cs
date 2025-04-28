using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScenesManager : MonoBehaviour
{
    [SerializeField]
    public GameObject LoadingPanel;
    public float fadeDuration;
    Image LoadImage;
    [SerializeField]
    GameObject gameoverPanel;
    [SerializeField]
    GameObject gameClearImageObject, backImageObject;
    Image gameClearImage, backImage;
    public bool isGameClear = false;
    public bool isGameOver = false;
    bool isPause = false;
    [SerializeField]
    private GameObject pauseImageObject;
    public void GetPauseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isPause == false)
            {
                pauseImageObject.SetActive(true);
                Time.timeScale = 0;
                isPause = true;
            }
            else
            {
                pauseImageObject.SetActive(false);
                Time.timeScale = 1;
                isPause = false;
            }
        }
    }
    private void Awake()
    {
        LoadImage = LoadingPanel.GetComponent<Image>();
        if (gameClearImageObject != null)
            gameClearImage = gameClearImageObject.GetComponent<Image>();
        if (gameClearImageObject != null)
            backImage = backImageObject.GetComponent<Image>();
        pauseImageObject = GameObject.Find("PauseImage");
        if (pauseImageObject != null)
            pauseImageObject.SetActive(false);
    }
    private void Start()
    {
        FadeOut();
        Invoke("DisablePanel", fadeDuration);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F9))
        {
            FadeIn();
            Invoke("LoadTeaching", fadeDuration);
        }
        if(Input.GetKeyDown(KeyCode.F10))
        {
            FadeIn();
            Invoke("LoadGame1", fadeDuration);
        }
        if(Input.GetKeyDown(KeyCode.F11))
        {
            FadeIn();
            Invoke("LoadGame2", fadeDuration);
        }
        if(Input.GetKeyDown(KeyCode.F12))
        {
            FadeIn();
            Invoke("LoadGame3", fadeDuration);
        }
    }
    void DisablePanel()
    {
        LoadingPanel.SetActive(false);
    }
    public void LoadNextScene()
    {
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (currentSceneIndex)
        {
            case 1:
                FadeIn();
                Invoke("LoadGame1", fadeDuration);
                break;
            case 2:
                FadeIn();
                Invoke("LoadGame2", fadeDuration);
                break;
            case 3:
                FadeIn();
                Invoke("LoadGame3", fadeDuration);
                break;
            case 4:
                GameClear();
                break;

        }
    }
    public void GameClear()
    {
        gameoverPanel.SetActive(false);
        gameClearImageObject.SetActive(true);
        backImageObject.SetActive(true);
        StartCoroutine(Fade(gameClearImage, 0f, 1f,fadeDuration));
        StartCoroutine(Fade(backImage, 0f, 1f, fadeDuration));
        isGameClear = true;
    }
    public void StartTeaching()
    {
        FadeIn();
        Invoke("LoadTeaching", fadeDuration);
    }
    public void LoadTeaching()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadGame1()
    {
        SceneManager.LoadScene(2);
    }
    public void LoadGame2()
    {
        SceneManager.LoadScene(3);
    }
    public void LoadGame3()
    {
        SceneManager.LoadScene(4);
    }
    public void GetGameClearInput(InputAction.CallbackContext context)
    {
        if (context.performed&&isGameClear ==true)
        {
            FadeIn();
            SceneManager.LoadScene(0);
        }
    }
    public void GetSkipInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StopAllCoroutines();
            FadeIn();
            Invoke("LoadGame1", fadeDuration);
        }
    }
    public void GetStartInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartTeaching();
        }
    }
    public void GetReloadInput(InputAction.CallbackContext context)
    {
        if (context.performed&&isGameOver ==true)
        {
            ReloadScene();
        }
    }
    public void BackTotitle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StopAllCoroutines();
            FadeIn();
            SceneManager.LoadScene(0);
        }
    }
    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void FadeIn()
    {
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        LoadingPanel.SetActive(true);
        Color color = LoadImage.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
            LoadImage.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = toAlpha;
        LoadImage.color = color;
    }
    public IEnumerator Fade(Image image,float fromAlpha, float toAlpha,float fadeDuration)
    {
        image.gameObject.SetActive(true);
        Color color = image.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
            image.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = toAlpha;
        image.color = color;
    }
}
