using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData Data;
    public Health Health; // Health component

    #region Variables
    public Rigidbody2D RB { get; private set; }

    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsSliding { get; private set; }
    public bool IsDashing { get; private set; }

    public float LastOnGroundTime { get; private set; }

    private bool _isJumpCut;
    private bool _isJumpFalling;
    private Vector2 _moveInput;
    public float LastPressedJumpTime { get; private set; }

    private bool _hasJumpedSinceGrounded;

    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;

    [Header("Jump")]
    [SerializeField] private float jumpCooldown = 0f;
    private float _lastJumpTime;
    #endregion

    #region Dash Variables
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private int maxAirDashes = 1;

    private float _lastDashTime;
    private int _airDashesUsed;
    private Vector2 _dashDir;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        LastOnGroundTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;

        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
            OnJumpInput();

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J))
            OnJumpUpInput();

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K))
            TryDash();

        bool grounded = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);
        if (grounded)
        {
            LastOnGroundTime = Data.coyoteTime;
            _hasJumpedSinceGrounded = false;
            _airDashesUsed = 0;
        }

        if (IsDashing) return;

        if (IsJumping && RB.linearVelocity.y < 0)
        {
            IsJumping = false;
            _isJumpFalling = true;
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;
            _isJumpFalling = false;
        }

        if (CanJump() && LastPressedJumpTime > 0)
            Jump();

        IsSliding = false;

        if (_isJumpCut)
        {
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
        }
        else if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        else if (RB.linearVelocity.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
        }
        else
        {
            SetGravityScale(Data.gravityScale);
        }
    }

    private void FixedUpdate()
    {
        if (IsDashing) return;

        Run(1);
    }

    #region INPUT CALLBACKS
    public void OnJumpInput() => LastPressedJumpTime = Data.jumpInputBufferTime;

    public void OnJumpUpInput()
    {
        if (CanJumpCut())
            _isJumpCut = true;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale) => RB.gravityScale = scale;
    #endregion

    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        float currentXSpeed = RB.linearVelocity.x;
        float speedDif = targetSpeed - currentXSpeed;

        float accelRate;
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;

        float movement = speedDif * accelRate;
        RB.AddForce(Vector2.right * movement, ForceMode2D.Force);
        RB.linearVelocity = new Vector2(Mathf.Clamp(RB.linearVelocity.x, -Data.runMaxSpeed, Data.runMaxSpeed), RB.linearVelocity.y);
    }

    private void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        if (!Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            return;

        _lastJumpTime = Time.time;
        LastPressedJumpTime = 0;
        _hasJumpedSinceGrounded = true;

        RB.linearVelocity = new Vector2(RB.linearVelocity.x, 0);
        RB.AddForce(Vector2.up * Data.jumpForce, ForceMode2D.Impulse);

        IsJumping = true;
        _isJumpCut = false;
        _isJumpFalling = false;
    }

    private bool CanJump()
    {
        return Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);
    }

    private bool CanJumpCut() => IsJumping && RB.linearVelocity.y > 0;
    #endregion

    #region DASH METHODS
    private void TryDash()
    {
        if (Time.time - _lastDashTime < dashCooldown) return;
        if (IsDashing) return;
        if (LastOnGroundTime <= 0 && _airDashesUsed >= maxAirDashes) return;

        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        IsDashing = true;
        _lastDashTime = Time.time;

        if (LastOnGroundTime <= 0)
            _airDashesUsed++;

        
        Vector2 inputDir = new Vector2(_moveInput.x, 0f);
        if (inputDir == Vector2.zero)
            inputDir = IsFacingRight ? Vector2.right : Vector2.left;

        inputDir.Normalize();
        _dashDir = inputDir;


        float originalGravity = RB.gravityScale;
        SetGravityScale(0);

        Health.IsInvincible = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Coroutine blinkRoutine = StartCoroutine(BlinkEffect(sr));

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        RB.linearVelocity = _dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        SetGravityScale(originalGravity);
        IsDashing = false;
        Health.IsInvincible = false;

        StopCoroutine(blinkRoutine);
        sr.color = Color.white;

        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        RB.linearVelocity = new Vector2(RB.linearVelocity.x * 0.5f, RB.linearVelocity.y);
    }

    private IEnumerator BlinkEffect(SpriteRenderer sr)
    {
        while (true)
        {
            sr.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
    #endregion
}


