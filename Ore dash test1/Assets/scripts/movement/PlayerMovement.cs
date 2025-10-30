using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData Data;

    #region Variables
    public Rigidbody2D RB { get; private set; }

    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsSliding { get; private set; }
    public bool IsDashing { get; private set; }

    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    private bool _isJumpCut;
    private bool _isJumpFalling;
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;
    private Vector2 _moveInput;
    public float LastPressedJumpTime { get; private set; }

    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;

    private bool _hasJumpedSinceGrounded;
    [SerializeField] private float jumpCooldown = 0.2f;
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
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
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
        #endregion

        #region COLLISION CHECKS
        if (!IsJumping)
        {
            bool grounded = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);
            if (grounded)
            {
                LastOnGroundTime = Data.coyoteTime;
                _hasJumpedSinceGrounded = false;
                _airDashesUsed = 0; // ✅ reset dashy po lądowaniu
            }

            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;

            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;

            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
        #endregion

        if (IsDashing) return; // ⛔ Zatrzymaj logikę, gdy dash trwa

        #region JUMP CHECKS
        if (IsJumping && RB.linearVelocity.y < 0)
        {
            IsJumping = false;
            if (!IsWallJumping)
                _isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
            IsWallJumping = false;

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;
            _isJumpFalling = false;
        }

        if (CanJump() && LastPressedJumpTime > 0)
            Jump();
        else if (CanWallJump() && LastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
            WallJump(_lastWallJumpDir);
        }
        #endregion

        #region SLIDE CHECKS
        IsSliding = CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0));
        #endregion

        #region GRAVITY
        if (IsSliding)
        {
            SetGravityScale(Data.gravityScale * 0.1f);
        }
        else if (RB.linearVelocity.y < 0 && _moveInput.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFastFallSpeed));
        }
        else if (_isJumpCut)
        {
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
        }
        else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
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
        #endregion
    }

    private void FixedUpdate()
    {
        if (IsDashing)
            return;

        if (IsWallJumping)
            Run(Data.wallJumpRunLerp);
        else
            Run(1);

        if (IsSliding)
            Slide();
    }

    #region INPUT CALLBACKS
    public void OnJumpInput() => LastPressedJumpTime = Data.jumpInputBufferTime;

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
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

        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult * 0.5f;
        }

        float movement = speedDif * accelRate;
        RB.AddForce(Vector2.right * movement, ForceMode2D.Force);
        RB.linearVelocity = new Vector2(Mathf.Clamp(RB.linearVelocity.x, -Data.runMaxSpeed, Data.runMaxSpeed), RB.linearVelocity.y);
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
        _lastJumpTime = Time.time;
        float force = Data.jumpForce;
        if (RB.linearVelocity.y < 0)
            force -= RB.linearVelocity.y;

        RB.linearVelocity = new Vector2(RB.linearVelocity.x, 0);
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        IsJumping = true;
        IsWallJumping = false;
        _isJumpCut = false;
        _isJumpFalling = false;
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        Vector2 force = new Vector2(Data.wallJumpForce.x * dir, Data.wallJumpForce.y);
        RB.linearVelocity = Vector2.zero;
        RB.AddForce(force, ForceMode2D.Impulse);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        float speedDif = Data.slideSpeed - RB.linearVelocity.y;
        float movement = speedDif * Data.slideAccel;
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        RB.AddForce(movement * Vector2.up);
    }
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

        Vector2 inputDir = new Vector2(_moveInput.x, _moveInput.y);
        if (inputDir == Vector2.zero)
            inputDir = IsFacingRight ? Vector2.right : Vector2.left;

        inputDir.Normalize();
        _dashDir = inputDir;

        float originalGravity = RB.gravityScale;
        SetGravityScale(0);
        RB.linearVelocity = _dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        SetGravityScale(originalGravity);
        IsDashing = false;
    }
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && Time.time - _lastJumpTime >= jumpCooldown && !IsWallJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 &&
            (!IsWallJumping || (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut() => IsJumping && RB.linearVelocity.y > 0;
    private bool CanWallJumpCut() => IsWallJumping && RB.linearVelocity.y > 0;
    public bool CanSlide() => LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0;
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion
}

