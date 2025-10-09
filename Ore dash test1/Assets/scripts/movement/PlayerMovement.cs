using UnityEngine;

public class PlayerMovement : MonoBehaviour

{

    [Header("Movement Settings")]

    public float moveSpeed = 5f;

    public float jumpForce = 10f;

    [Header("Jump Cooldown")]

    public float jumpCooldown = 3f;

    private float lastJumpTime = -Mathf.Infinity;

    [Header("Dash Settings")]

    public float dashSpeed = 15f;

    public float dashDuration = 0.2f;

    private bool isDashing = false;

    private bool canDash = true;

    private float horizontalInput;

    private Rigidbody2D rb;

    void Start()

    {

        rb = GetComponent<Rigidbody2D>();

    }

    void Update()

    {

        if (isDashing) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Skok z cooldownem

        if (Input.GetButtonDown("Jump") && Time.time >= lastJumpTime + jumpCooldown)

        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            lastJumpTime = Time.time;

            Debug.Log("Skok! (Cooldown)");

        }

        // Dash

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)

        {

            StartCoroutine(Dash());

        }

    }

    void FixedUpdate()

    {

        if (isDashing) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

    }

    System.Collections.IEnumerator Dash()

    {

        isDashing = true;

        canDash = false;

        float originalGravity = rb.gravityScale;

        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;

        isDashing = false;

        yield return new WaitForSeconds(0.5f);

        canDash = true;

    }

}

