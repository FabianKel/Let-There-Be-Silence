using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    [Header("Target References")]
    public Transform exitPoint;
    public Transform roomCameraNode;

    public void OnChildTriggerEnter(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit the Entry child of: " + gameObject.name);
            Transition(collision.transform);
        }
    }

    private void Transition(Transform player)
    {
        player.position = exitPoint.position;
        RoomManager.Instance.ChangeRoom(roomCameraNode.position);
    }
}