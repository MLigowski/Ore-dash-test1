using UnityEngine;
using Unity.Cinemachine;

public class CameraTeleportWithBounds : MonoBehaviour
{
#pragma warning disable CS0618 // Type or member is obsolete
    public CinemachineVirtualCamera vCam;
#pragma warning restore CS0618 // Type or member is obsolete
    public Transform player;

    /// <summary>
    /// Teleportuje gracza na nową pozycję i ustawia nowe granice kamery
    /// </summary>
    /// <param name="newPosition">Nowa pozycja gracza</param>
    /// <param name="newBounds">Nowy BoxCollider2D jako granice kamery</param>
    [System.Obsolete]
    public void TeleportPlayer(Vector3 newPosition, PolygonCollider2D newBounds)
    {
        
        player.position = newPosition;

        // 2️⃣ Wymuś natychmiastową pozycję kamery
        vCam.ForceCameraPosition(player.position, vCam.transform.rotation);

        // 3️⃣ Zmień granice kamery
        var confiner = vCam.GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = newBounds;
        confiner.InvalidatePathCache(); // aktualizuje granice

        // 4️⃣ (Opcjonalnie) Reset Damping na chwilę, jeśli używasz smooth follow
        var transposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        float originalXDamping = transposer.m_XDamping;
        float originalYDamping = transposer.m_YDamping;

        transposer.m_XDamping = 0f;
        transposer.m_YDamping = 0f;

        // Kamera od razu podąża za graczem
        vCam.ForceCameraPosition(player.position, vCam.transform.rotation);

        // Przywróć damping
        transposer.m_XDamping = originalXDamping;
        transposer.m_YDamping = originalYDamping;
    }
}
