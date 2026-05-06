using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Configuración del Nivel Actual")]
    public LevelData nivelActual;

    [Header("Flujo de Niveles")]
    public string[] nivelesDisponibles = { "Nivel1", "Nivel2", "Nivel3" };
    private int indiceNivelActual = 0;

    public void CargarSiguienteNivel()
    {
        indiceNivelActual++;
        if (indiceNivelActual < nivelesDisponibles.Length)
        {
            SceneLoader.Instance.LoadLevel(nivelesDisponibles[indiceNivelActual]);
        }
        else
        {
            SceneLoader.Instance.LoadLevel("MainMenu");
        }
    }

    public void SeleccionarNivel(int index)
    {
        indiceNivelActual = index;
        SceneLoader.Instance.LoadLevel(nivelesDisponibles[index]);
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PrepararNivel()
    {
        if (nivelActual == null) return;

        AudioMixer.Instance.pistaBajo.clip = nivelActual.bajo;
        AudioMixer.Instance.pistaBateria.clip = nivelActual.bateria;
        AudioMixer.Instance.pistaPiano.clip = nivelActual.piano;

        RhythmManager.Instance.LoadData(nivelActual.RythmFile);

        RhythmicEnemy[] todosLosEnemigos = Object.FindObjectsByType<RhythmicEnemy>(FindObjectsSortMode.None);
        AudioMixer.Instance.bassEnemiesPerRun = todosLosEnemigos.Count(e => e.myTrack == TrackType.Bass);
        AudioMixer.Instance.kitEnemiesPerRun = todosLosEnemigos.Count(e => e.myTrack == TrackType.Kit);
        AudioMixer.Instance.pianoEnemiesPerRun = todosLosEnemigos.Count(e => e.myTrack == TrackType.Piano);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = nivelActual.initialPlayerPosition;
            RoomManager.Instance.ChangeRoom(player.transform.position);
        }
    }


    public void PausarJuego()
    {
        Time.timeScale = 0f;
        AudioMixer.Instance.pistaBajo.Pause();
        AudioMixer.Instance.pistaBateria.Pause();
        AudioMixer.Instance.pistaPiano.Pause();
    }

    public void ReanudarJuego()
    {
        Time.timeScale = 1f;
        AudioMixer.Instance.pistaBajo.UnPause();
        AudioMixer.Instance.pistaBateria.UnPause();
        AudioMixer.Instance.pistaPiano.UnPause();
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Ganar()
    {
        Debug.Log("¡Nivel completado!");
    }
}