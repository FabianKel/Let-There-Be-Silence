using System;
using UnityEngine;

public class AudioMixer : MonoBehaviour
{
    public static AudioMixer Instance;

    public event Action OnAllTracksSilenced;

    [Header("Audio Sources")]
    public AudioSource pistaBateria;
    public AudioSource pistaBajo;
    public AudioSource pistaPiano;

    [Header("Configuración Inicial")]
    public int bassEnemiesPerRun = 1;
    public int kitEnemiesPerRun = 1;
    public int pianoEnemiesPerRun = 1;

    private int currentBass, currentKit, currentPiano;
    private int totalBass, totalKit, totalPiano;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void StartAudio()
    {
        totalBass = currentBass = bassEnemiesPerRun;
        totalKit = currentKit = kitEnemiesPerRun;
        totalPiano = currentPiano = pianoEnemiesPerRun;

        if (pistaBateria) pistaBateria.volume = 1f;
        if (pistaBajo) pistaBajo.volume = 1f;
        if (pistaPiano) pistaPiano.volume = 1f;

        double startTime = AudioSettings.dspTime + 0.5;

        if (pistaBateria) pistaBateria.PlayScheduled(startTime);
        if (pistaBajo) pistaBajo.PlayScheduled(startTime);
        if (pistaPiano) pistaPiano.PlayScheduled(startTime);

        Debug.Log("Audio sincronizado e iniciado");
    }

    public void EnemigoDerrotado(TrackType tipo)
    {
        switch (tipo)
        {
            case TrackType.Bass:
                currentBass--;
                ActualizarVolumen(pistaBajo, currentBass, totalBass);
                break;
            case TrackType.Kit:
                currentKit--;
                ActualizarVolumen(pistaBateria, currentKit, totalKit);
                break;
            case TrackType.Piano:
                currentPiano--;
                ActualizarVolumen(pistaPiano, currentPiano, totalPiano);
                break;
        }
        CheckWinCondition();
    }

    void ActualizarVolumen(AudioSource source, int actuales, int iniciales)
    {
        if (source == null || iniciales <= 0) return;

        float nuevoVolumen = (float)actuales / iniciales;
        source.volume = Mathf.Clamp01(nuevoVolumen);

        print($"Actualizando {source.name}: {actuales}/{iniciales} enemigos. Vol: {source.volume}");
    }

    private void CheckWinCondition()
    {
        if (currentBass <= 0 && currentKit <= 0 && currentPiano <= 0)
        {
            Debug.Log("Todas las pistas musicales han sido silenciadas.");
            StopAllAudio();

            OnAllTracksSilenced?.Invoke();
        }
    }

    public void StopAllAudio()
    {
        pistaBateria?.Stop();
        pistaBajo?.Stop();
        pistaPiano?.Stop();
    }
}