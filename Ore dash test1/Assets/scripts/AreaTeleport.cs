using UnityEngine;

public class AreaTeleport : MonoBehaviour
{
    [Header("Teleport settings")]
    public Transform teleportPoint;
    public Camera mainCamera;
    public Vector3 cameraOffset = new Vector3(0, 0, -10);

    [Header("Unlocking")]
    public bool isUnlocked = false; // domyœlnie zablokowany

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !isUnlocked)
            return; // jeœli teleport zablokowany, nic siê nie dzieje

        // Teleport gracza
        other.transform.position = teleportPoint.position;

        if (mainCamera != null)
            mainCamera.transform.position = teleportPoint.position + cameraOffset;
    }

    public void UnlockTeleport()
    {
        isUnlocked = true;
        // Tutaj mo¿esz te¿ dodaæ wizualn¹ informacjê, np. zmieniæ kolor
        GetComponent<SpriteRenderer>().color = Color.green;
    }
}