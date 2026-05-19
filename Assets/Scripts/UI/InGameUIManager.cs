using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;

    [Header("Scene Names")]
    public string menuSceneName = "Menu";

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject confirmationPanel;
    public GameObject settingsPanel;
    public GameObject victoryPanel;

    [Header("Confirmation")]
    public TextMeshProUGUI confirmationText;
    private string confirmationTarget;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Time.timeScale = 1f;

        if (LevelManager.Instance) LevelManager.Instance.PrepararNivel();
        if (RhythmManager.Instance) RhythmManager.Instance.StartRhythm();
        if (AudioMixer.Instance) AudioMixer.Instance.StartAudio();

        pausePanel.SetActive(false);
        confirmationPanel.SetActive(false);
        settingsPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        if (victoryPanel != null && victoryPanel.activeSelf) return;

    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (settingsPanel.activeSelf) CloseSettings();
        else if (confirmationPanel.activeSelf) CancelAction();
        else TogglePause(!pausePanel.activeSelf);
    }

    public void TogglePause(bool isPaused)
    {
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        // Pausar audio y ritmo en el futuro
        // if (isPaused) RhythmManager.Instance.Pause();
    }

    public void ShowVictoryScreen()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            Time.timeScale = 0f;
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void ResumeGame() => TogglePause(false);

    public void ShowSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    public void RequestMainMenu()
    {
        confirmationTarget = "MainMenu";
        confirmationText.text = "¿Quieres volver al menú principal? Se perderá el progreso.";
        confirmationPanel.SetActive(true);
    }

    public void ConfirmAction()
    {
        if (confirmationTarget == "MainMenu")
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }
    }

    public void CancelAction() => confirmationPanel.SetActive(false);
}