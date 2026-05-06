using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUIManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "SampleScene";

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject LevelMenu;
    public GameObject confirmationPanel;
    public CanvasGroup mainMenuCanvasGroup;

    [Header("Confirmation")]
    public TextMeshProUGUI confirmationText;
    private string confirmationTarget;

    [Header("Transitions")]
    public Animator imageAnimator;
    public string animationTriggerName = "phasing";
    public string startFrameTriggerName = "startFrame";
    public float fadeDuration = 1.0f;

    [Header("Loading UI")]
    public GameObject loadingPanel;
    public Slider loadingBar;
    public TMPro.TextMeshProUGUI progressText;
    void Start()
    {
        Time.timeScale = 1f;
        if (AudioMixer.Instance) AudioMixer.Instance.StopAllAudio();

        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        ResetAllPanels();
        mainMenuPanel.SetActive(true);
        if (imageAnimator != null)
        {
            imageAnimator.gameObject.SetActive(true);
            imageAnimator.SetTrigger(startFrameTriggerName);
        }
    }

    public void ShowLevelMenu() => LevelMenu.SetActive(true);
    public void CloseLevelMenu() => LevelMenu.SetActive(false);

    public void PlayGame()
    {
        Debug.Log("Iniciando juego...");
        var raycaster = mainMenuPanel.GetComponent<GraphicRaycaster>();
        if (raycaster != null) raycaster.enabled = false;

        StartCoroutine(PlaySequence());
    }

    public void PlayLevel(string levelName)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        StartCoroutine(LoadLevelAsync(levelName));
    }

    private IEnumerator LoadLevelAsync(string levelName)
    {
        if (loadingPanel != null) loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingBar != null) loadingBar.value = progress;
            Debug.Log($"Cargando nivel... {Mathf.RoundToInt(progress * 100)}%");
            if (progressText != null) progressText.text = $"Cargando nivel... {Mathf.RoundToInt(progress * 100)}%";

            if (operation.progress >= 0.9f)
            {
                if (progressText != null) progressText.text = "¡Listo! Entrando...";
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator PlaySequence()
    {
        if (imageAnimator != null) imageAnimator.SetTrigger(animationTriggerName);

        float counter = 0;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            if (mainMenuCanvasGroup != null)
                mainMenuCanvasGroup.alpha = Mathf.Lerp(1, 0, counter / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    public void RequestQuit()
    {
        confirmationTarget = "Quit";
        confirmationText.text = "¿Estás seguro de que quieres salir?";
        confirmationPanel.SetActive(true);
    }

    public void ConfirmAction()
    {
        if (confirmationTarget == "Quit") Application.Quit();
    }

    public void CancelAction() => confirmationPanel.SetActive(false);

    private void ResetAllPanels()
    {
        if (mainMenuCanvasGroup != null) mainMenuCanvasGroup.alpha = 1f;
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmationPanel.SetActive(false);
    }
}