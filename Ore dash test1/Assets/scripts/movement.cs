using UnityEngine;

public class movement : MonoBehaviour
{
    public float speed = 5f;
    private float direction = 0f;
    private Rigidbody2D player;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        direction = Input.GetAxis("Horizontal");

        if (direction > 0f)
        {
            player.linearVelocity = new Vector2(direction * speed, player.velocity.y);
        }
        else if (direction < 0f)
        {
            player.linearVelocity = new Vector2(direction * speed, player.linearVelocity.y);
        }
        else
        {
            player.linearVelocity = new Vector2(0, player.linearVelocity.y);
        }
    }
}
