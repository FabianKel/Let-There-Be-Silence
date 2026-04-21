using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public Camera mainCamera;
    public float cameraLerpSpeed = 5f;
    private Vector3 targetCameraPosition;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        targetCameraPosition = mainCamera.transform.position;
    }

    void Update()
    {
        Vector3 smoothPos = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraLerpSpeed);
        mainCamera.transform.position = smoothPos;
    }

    public void ChangeRoom(Vector3 newRoomCenter)
    {
        targetCameraPosition = new Vector3(newRoomCenter.x, newRoomCenter.y, mainCamera.transform.position.z);
    }
}