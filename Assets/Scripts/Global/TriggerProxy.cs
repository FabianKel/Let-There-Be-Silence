using UnityEngine;

public class TriggerProxy : MonoBehaviour
{
    public RoomConnector parentConnector;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (parentConnector != null)
        {
            parentConnector.OnChildTriggerEnter(collision);
        }
    }
}