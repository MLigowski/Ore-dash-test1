using UnityEngine;

public class AreaTeleport : MonoBehaviour
{
    [Header("Teleport settings")]
    public Transform teleportPoint; 
    public Camera mainCamera;       
    public Vector3 cameraOffset = new Vector3(0, 0, -10); 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Teleport gracza
        other.transform.position = teleportPoint.position;

        
        if (mainCamera != null)
            mainCamera.transform.position = teleportPoint.position + cameraOffset;
    }
}
