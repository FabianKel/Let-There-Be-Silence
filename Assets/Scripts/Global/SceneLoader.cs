using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("UI de Carga")]
    [SerializeField] private GameObject loadingOverlay;
    [SerializeField] private Slider progressBar;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            loadingOverlay.SetActive(false);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        SceneLoader.Instance.LoadLevel("MainMenu");
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    private IEnumerator LoadAsynchronously(string sceneName)
    {
        loadingOverlay.SetActive(true);

        // Efecto de Fade In
        yield return StartCoroutine(Fade(1));

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
        yield return SceneManager.LoadSceneAsync("GlobalUI_Additive", LoadSceneMode.Additive);

        if (LevelManager.Instance != null) LevelManager.Instance.PrepararNivel();

        yield return StartCoroutine(Fade(0));
        loadingOverlay.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float speed = 2f;
        while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }
    }
}