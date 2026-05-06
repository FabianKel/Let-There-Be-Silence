using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static DataManager;


public enum TrackType { Bass, Kit, Piano }

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;
    public RhythmData rhythmData;

    public event Action OnBassBeat;
    public event Action OnKitBeat;
    public event Action OnPianoBeat;

    private float songTimer = 0f;
    private int bassIdx, kitIdx, pianoIdx;
    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;
    }

    public void StartRhythm()
    {
        if (rhythmData == null)
        {
            Debug.LogError("No se puede iniciar el ritmo: rhythmData es nulo");
            return;
        }
        songTimer = 0f;
        bassIdx = 0;
        kitIdx = 0;
        pianoIdx = 0;

        isPlaying = true;
        Debug.Log("Ritmo iniciado");
    }

    public void StopRhythm()
    {
        isPlaying = false;
    }

    // Aumentar el pitch al golpear un enemigo
    public IEnumerator HitEffect()
    {
        print(("Hit effect triggered"));
        gameObject.GetComponent<AudioSource>().pitch = 1.5f;
        //yield return new WaitForSeconds(0.2f);
        //gameObject.GetComponent<AudioSource>().pitch -= 0.05f;
        yield break;
    }

    public void LoadData(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            rhythmData = JsonUtility.FromJson<RhythmData>(json);
            Debug.Log("Nuevo ritmo cargado: " + fileName);
        }
    }


    void Update()
    {
        if (!isPlaying || rhythmData == null) return;

        songTimer += Time.deltaTime;

        CheckBeat(rhythmData.bassTimestamps, ref bassIdx, OnBassBeat);
        CheckBeat(rhythmData.kitTimestamps, ref kitIdx, OnKitBeat);
        CheckBeat(rhythmData.pianoTimestamps, ref pianoIdx, OnPianoBeat);
    }

    void CheckBeat(float[] timestamps, ref int index, Action beatEvent)
    {
        if (timestamps == null) return;

        if (index < timestamps.Length && songTimer >= timestamps[index])
        {
            beatEvent?.Invoke();
            index++;
        }
    }
}