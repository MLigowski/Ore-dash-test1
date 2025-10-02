using UnityEngine;

public class FlipCharacter : MonoBehaviour
{
    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (input > 0)
        {
            // Patrz w prawo
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (input < 0)
        {
            // Patrz w lewo
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
