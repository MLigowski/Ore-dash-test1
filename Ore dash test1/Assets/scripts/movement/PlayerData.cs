using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength;
    [HideInInspector] public float gravityScale;
    [Space(5)]
    public float fallGravityMult;
    public float maxFallSpeed;
    [Space(5)]
    public float fastFallGravityMult;
    public float maxFastFallSpeed;

    [Space(20)]

    [Header("Run")]
    public float runMaxSpeed;
    public float runAcceleration;
    [HideInInspector] public float runAccelAmount;
    public float runDecceleration;
    [HideInInspector] public float runDeccelAmount;
    [Space(5)]
    [Range(0f, 1)] public float accelInAir;
    [Range(0f, 1)] public float deccelInAir;
    [Space(5)]
    public bool doConserveMomentum = true;

    [Space(20)]

    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex;
    [HideInInspector] public float jumpForce;

    [Header("Both Jumps")]
    public float jumpCutGravityMult;
    [Range(0f, 1)] public float jumpHangGravityMult;
    public float jumpHangTimeThreshold;
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp;
    [Range(0f, 1.5f)] public float wallJumpTime;
    public bool doTurnOnWallJump;

    [Space(20)]

    [Header("Slide")]
    public float slideSpeed;
    public float slideAccel;

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime;
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;


    private void OnValidate()
    {
        // Bezpieczne obliczenia (unikamy dzielenia przez 0)
        if (jumpTimeToApex <= 0.01f) jumpTimeToApex = 0.01f;
        if (runMaxSpeed <= 0.01f) runMaxSpeed = 0.01f;

        //  Poprawione obliczenia grawitacji
        gravityStrength = 2 * jumpHeight / Mathf.Pow(jumpTimeToApex, 2);
        gravityScale = gravityStrength / Mathf.Abs(Physics2D.gravity.y);
        gravityStrength *= -1f;

        // U¿ycie Time.fixedDeltaTime zamiast magicznej liczby 50
        float fixedStep = (1f / Time.fixedDeltaTime);
        runAccelAmount = (fixedStep * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (fixedStep * runDecceleration) / runMaxSpeed;

        // Calculate jump force
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }


}
