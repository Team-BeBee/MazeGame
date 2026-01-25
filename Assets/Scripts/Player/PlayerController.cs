using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;

    [Header("Acceleration")]
    public float walkAcceleration = 14f;
    public float sprintAcceleration = 20f;

    [Tooltip("0~1 : 가속 초반 저항 / 후반 가속 느낌")]
    public AnimationCurve accelerationCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Resistance")]
    public float stopResistance = 24f;

    [Range(0f, 1f)] public float turnResistance = 0.35f;
    [Range(0f, 180f)] public float turnResistanceAngle = 90f;
    public float resistanceRecovery = 6f;

    [Header("Jump")]
    public float jumpHeight = 1.4f;
    public float jumpBufferTime = 0.12f;
    public float coyoteTime = 0.10f;

    [Header("Gravity")]
    public float gravity = -9.81f;

    private CharacterController cc;
    private PlayerInput input;

    private Vector3 velocity;
    private float currentSpeed;
    private Vector3 lastMoveDir;
    private float resistanceMultiplier = 1f;

    private float jumpBufferTimer = 0f;
    private float coyoteTimer = 0f;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        lastMoveDir = transform.forward;
    }

    private void Update()
    {
        bool jumpPressedThisFrame = Input.GetKeyDown(KeyCode.Space);

        // ---- Jump buffer ----
        if (jumpPressedThisFrame)
            jumpBufferTimer = jumpBufferTime;
        jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - Time.deltaTime);

        // ---- Read grounded EARLY ----
        bool grounded = cc.isGrounded;

        // ---- Coyote update ----
        if (grounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.deltaTime);

        // ---- Stick to ground ----
        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        // ---- Jump execute ----
        bool canJump = jumpBufferTimer > 0f && coyoteTimer > 0f;
        if (canJump)
        {
            float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
            velocity.y = jumpVelocity;

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            grounded = false;
        }

        // ---- Horizontal movement ----
        Vector2 raw = input.MoveInput;
        Vector3 moveDir = Vector3.ClampMagnitude(
            transform.right * raw.x + transform.forward * raw.y, 1f);

        bool hasInput = moveDir.sqrMagnitude > 0.0001f;
        bool sprint = hasInput &&
                      (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        float targetSpeed = hasInput ? (sprint ? sprintSpeed : walkSpeed) : 0f;
        float accelBase = sprint ? sprintAcceleration : walkAcceleration;

        if (hasInput)
        {
            float angle = Vector3.Angle(lastMoveDir, moveDir);
            if (angle >= turnResistanceAngle)
            {
                float t = Mathf.InverseLerp(turnResistanceAngle, 180f, angle);
                resistanceMultiplier = Mathf.Min(
                    resistanceMultiplier,
                    1f - Mathf.Lerp(0f, turnResistance, t)
                );
            }
            lastMoveDir = moveDir;
        }

        resistanceMultiplier = Mathf.MoveTowards(
            resistanceMultiplier, 1f, resistanceRecovery * Time.deltaTime);

        float effectiveTarget = targetSpeed * resistanceMultiplier;

        float ratio = effectiveTarget > 0.01f
            ? Mathf.Clamp01(currentSpeed / effectiveTarget)
            : 0f;

        float curve = Mathf.Max(0.05f, accelerationCurve.Evaluate(ratio));
        float accel = hasInput ? accelBase * curve : stopResistance;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed, effectiveTarget, accel * Time.deltaTime);

        cc.Move(moveDir * currentSpeed * Time.deltaTime);

        // ---- Gravity & vertical move ----
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
