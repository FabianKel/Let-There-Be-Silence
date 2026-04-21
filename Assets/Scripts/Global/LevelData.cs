using UnityEngine;

[CreateAssetMenu(fileName = "NuevoNivel", menuName = "Juego/Nivel")]
public class LevelData : ScriptableObject
{
    public string nombreNivel;
    public AudioClip bajo, bateria, piano;
    public string RythmFile; // Mi Level 1 es: Level1RhythmData

    [Header("Spawn Settings")]
    public Vector2 initialPlayerPosition;
}