using UnityEngine;

/// <summary>
/// 테스트용 랜덤 배회 AI.
/// 사용 전 확인:
/// - 크리쳐 루트에 Rigidbody/Collider/Animator가 있어야 합니다.
/// - Animator의 Apply Root Motion은 테스트 목적이라면 OFF를 권장합니다.
/// - Rigidbody의 Interpolate를 켜면 이동이 더 부드럽게 보일 수 있습니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SimpleWanderAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float changeDirectionTime = 3f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";

    private Rigidbody rb;
    private Vector3 currentDirection;
    private float directionTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        PickRandomDirection();
    }

    private void FixedUpdate()
    {
        directionTimer += Time.fixedDeltaTime;
        if (directionTimer >= changeDirectionTime)
        {
            directionTimer = 0f;
            PickRandomDirection();
        }

        Vector3 moveDelta = currentDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDelta);

        if (currentDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection, Vector3.up);
            Quaternion smoothedRotation = Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(smoothedRotation);
        }

        UpdateAnimation();
    }

    private void PickRandomDirection()
    {
        Vector2 random2D = Random.insideUnitCircle.normalized;

        if (random2D.sqrMagnitude < 0.0001f)
        {
            random2D = Vector2.up;
        }

        currentDirection = new Vector3(random2D.x, 0f, random2D.y).normalized;
    }

    private void UpdateAnimation()
    {
        if (animator == null || string.IsNullOrEmpty(speedParameter))
        {
            return;
        }

        // 테스트 용도에서는 속도가 0이 되지 않게 moveSpeed 기반으로 전달해 걷기 애니메이션 유지.
        float animationSpeed = Mathf.Max(moveSpeed, rb.linearVelocity.magnitude);
        animator.SetFloat(speedParameter, animationSpeed);
    }
}
