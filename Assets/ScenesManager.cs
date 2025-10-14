using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public enum ScenesNum
{
	StartScene,
	BattleScene,
}

public class ScenesManager : MonoBehaviour
{
    public ScenesNum scenesNum = ScenesNum.BattleScene;
	PlayerHP playerHP;
    PlayerSlashAttack playerSlashAttack;
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
    public bool isSecondLife = true;
    bool isPause = false;
    [SerializeField]
    private GameObject pauseImageObject;
    [SerializeField] GainWAnimator GainV;

	public Image loadingBar;
	private AsyncOperationHandle<SceneInstance> sceneHandle;
	private bool sceneLoaded = false;
    private bool readyToActivate = false;
    bool AniTrue = false;
    public LoadindAnim loadindAnim;
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
        Time.timeScale = 1;
        
        if (gameClearImageObject != null)
            gameClearImage = gameClearImageObject.GetComponent<Image>();
        if (gameClearImageObject != null)
            backImage = backImageObject.GetComponent<Image>();
        pauseImageObject = GameObject.Find("PauseImage");

		LoadingPanel = GameObject.Find("LoadingPanel");
		if (scenesNum == ScenesNum.StartScene)
        {
			LoadImage = LoadingPanel.GetComponent<Image>();
		}
		if (scenesNum == ScenesNum.BattleScene)
		{
			
			pauseImageObject = GameObject.Find("PauseImage");
			playerHP = GameObject.Find("Player").transform.Find("HPCollider").GetComponent<PlayerHP>();
            playerSlashAttack = GameObject.Find("Player").GetComponent<PlayerSlashAttack>();
        }
        if (pauseImageObject != null)
            pauseImageObject.SetActive(false);
        loadingBar = GameObject.Find("LoadingBar2").GetComponent<Image>();
		loadindAnim = GameObject.Find("LoadC").GetComponent<LoadindAnim>();



    }
    private void Start()
    {
        if (scenesNum == ScenesNum.StartScene)
        {
            FadeOut();
        }

        //Invoke("DisablePanel", fadeDuration);
    }
	IEnumerator ActivateScene()
	{
		if (!sceneHandle.IsDone || sceneHandle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogWarning("場景尚未完成加載，無法啟用！");
			yield break;
		}

		yield return sceneHandle.Result.ActivateAsync();
		Debug.Log("場景已啟用！");
	}
	private void Update()
    {




		if (Input.GetKeyDown(KeyCode.F9))
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
		GameObject.Find("BlowingLeaves_1").GetComponent<ParticleSystem>().Stop();
		loadindAnim.StartFadeOutMusic();
		GameObject.Find("BlowingLeaves").GetComponent<ParticleSystem>().Stop();
        loadindAnim.floatIn();
		if (GainV != null&& AniTrue == false)
        {
            AniTrue = true;
			FadeIn();
            GainV.StartAnimation();
        }
    }
	public void InvokeLoadTeaching()
	{
		StartCoroutine(LoadSceneAsync("Assets/Scenes/Scene1.unity")); // 建議使用 Address 名稱，不要用 Assets/Scenes/Scene1.unity
	}
	IEnumerator LoadSceneAsync(string sceneAddress)
	{
		sceneLoaded = false;
		readyToActivate = false;
        
		// 開始加載（不自動啟用）
		sceneHandle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Single, false);
		yield return sceneHandle;

		if (sceneHandle.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError("場景加載失敗！");
			yield break;
		}

		// 第一段動畫：根據進度填充到 0.9
		yield return AnimateLoadingBarTo90();
        readyToActivate = true;


		StartCoroutine(ActivateScene());
		// 第二段動畫：純動畫從 0.9 填滿到 1
		//yield return AnimateLoadingBarTo100();

		//// 準備好接受按鍵轉場
		//Debug.Log("載入完成，請按任意鍵繼續...");
		//sceneLoaded = true;
	}

	IEnumerator AnimateLoadingBarTo90()
	{
		float displayProgress = 0f;

		while (sceneHandle.PercentComplete < 0.9f)
		{
			float target = Mathf.Clamp01(sceneHandle.PercentComplete / 0.9f); // 轉為 0 ~ 1
			displayProgress = Mathf.MoveTowards(displayProgress, target, Time.deltaTime * 0.5f);
			loadingBar.fillAmount = displayProgress;
			yield return null;
		}

		// 確保最後停在 0.9
		displayProgress = 0.9f;
		loadingBar.fillAmount = displayProgress;
	}

	public void EndLoading()
	{
		StartCoroutine(AnimateLoadingBarTo100());
	}
	IEnumerator AnimateLoadingBarTo100()
	{
		float displayProgress = 0.9f;

        while (displayProgress < 1f)
        {
            displayProgress = Mathf.MoveTowards(displayProgress, 1f, Time.deltaTime * 0.5f);
            loadingBar.fillAmount = displayProgress;
            yield return null;
        }
        sceneLoaded = true;
		loadindAnim.LoadOver = true;
		loadindAnim.Gamestarted = true;
		loadindAnim.StartGame();
		Debug.Log("載入完成，請按任意鍵繼續...");
	}

    public void EndGame()
    {
        Application.Quit();
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
        if (context.performed && isGameOver && isSecondLife)
        {
            isSecondLife = false;
            isGameOver = false;
            playerHP.SecondLife();
            playerSlashAttack.NormalizePlayer();
        }
        else if(context.performed && isGameOver && isSecondLife==false)
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
