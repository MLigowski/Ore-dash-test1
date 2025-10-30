using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPosX;
    public GameObject cam;
    [Range(0f, 1f)] public float parallaxEffectX;  // Efekt parallaxu w poziomie

    private void Start()
    {
        startPosX = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Pobiera szerokoœæ sprite'a
    }

    private void LateUpdate() // zamiast FixedUpdate – p³ynniejszy ruch
    {
        // Ruch w poziomie
        float tempX = cam.transform.position.x * (1 - parallaxEffectX);
        float distX = cam.transform.position.x * parallaxEffectX;
        transform.position = new Vector3(startPosX + distX, transform.position.y, transform.position.z);

        // Przewijanie t³a w poziomie (jeœli kamera przekroczy³a d³ugoœæ t³a)
        if (tempX > startPosX + length)
            startPosX += length;
        else if (tempX < startPosX - length)
            startPosX -= length;
    }
}




