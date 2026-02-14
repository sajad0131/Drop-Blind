using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private System.Action<Obstacle> returnToPoolAction;
    private float destructionY; // Y position where the obstacle is "safe" to remove (above player)

    public void Initialize(System.Action<Obstacle> returnAction, float topYThreshold)
    {
        returnToPoolAction = returnAction;
        destructionY = topYThreshold;
    }

    private void Update()
    {
        // Check if we have moved past the player and gone off-screen (Top)
        // Since the World moves UP, the obstacle's World Position Y increases.
        if (transform.position.y > destructionY)
        {
            returnToPoolAction?.Invoke(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("GAME OVER! Hit Obstacle.");
            // TODO: Trigger Game Over Event
        }
    }
}