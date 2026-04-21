using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public CanvasGroup mainMenuCanvasGroup;
    public GameObject pausePanel;
    public GameObject confirmationPanel;

    [Header("Confirmation Text")]
    public TMPro.TextMeshProUGUI confirmationText;

    private string confirmationTarget;

    [Header("Transitions")]
    public Animator imageAnimator;
    public string animationTriggerName = "phasing";
    public string startFrameTriggerName = "startFrame";
    public float fadeDuration = 1.0f;

    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !mainMenuPanel.activeSelf)
        {
            TogglePause(!pausePanel.activeSelf);
        }
    }

    public void ShowMainMenu()
    {
        if (RhythmManager.Instance) RhythmManager.Instance.StopRhythm();
        if (AudioMixer.Instance) AudioMixer.Instance.StopAllAudio();

        ResetAllPanels();
        mainMenuPanel.SetActive(true);
        imageAnimator.gameObject.SetActive(true);

        if (imageAnimator != null)
            imageAnimator.SetTrigger(startFrameTriggerName);
    }

    public void PlayGame()
    {
        var raycaster = mainMenuPanel.GetComponent<GraphicRaycaster>();
        if (raycaster != null) raycaster.enabled = false;

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (imageAnimator != null) imageAnimator.SetTrigger(animationTriggerName);

        LevelManager.Instance.PrepararNivel();

        float counter = 0;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            if (mainMenuCanvasGroup != null)
                mainMenuCanvasGroup.alpha = Mathf.Lerp(1, 0, counter / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        RhythmManager.Instance.StartRhythm();
        AudioMixer.Instance.StartAudio();

        mainMenuPanel.SetActive(false);

        var raycaster = mainMenuPanel.GetComponent<GraphicRaycaster>();
        if (raycaster != null) raycaster.enabled = true;
    }


    public void ShowSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    public void TogglePause(bool isPaused)
    {
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame() => TogglePause(false);

    public void RequestQuit()
    {
        confirmationTarget = "Quit";
        confirmationText.text = "¿Estás seguro de que quieres salir?";
        confirmationPanel.SetActive(true);
    }

    public void RequestMainMenu()
    {
        confirmationTarget = "MainMenu";
        confirmationText.text = "¿Quieres volver al menú principal?";
        confirmationPanel.SetActive(true);
    }

    public void ConfirmAction()
    {
        if (confirmationTarget == "Quit") Application.Quit();
        else if (confirmationTarget == "MainMenu") ShowMainMenu();
    }

    public void CancelAction() => confirmationPanel.SetActive(false);

    private void ResetAllPanels()
    {
        mainMenuCanvasGroup.alpha = 1f;
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);
        confirmationPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}