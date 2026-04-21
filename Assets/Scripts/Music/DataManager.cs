using UnityEngine;

public class DataManager : MonoBehaviour
{

    [System.Serializable]
    public class RhythmData
    {
        public float[] bassTimestamps;
        public float[] kitTimestamps;
        public float[] pianoTimestamps;
    }
}
