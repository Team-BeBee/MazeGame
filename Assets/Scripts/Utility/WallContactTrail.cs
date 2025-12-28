using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterController 기반 플레이어가 Wall 레이어(벽/문벽 포함)에 닿을 때
/// - 점(Quad 등) 흔적 생성
/// - 패널티 1 증가 (PenaltyManager.AddLocalPenalty)
/// - 전역 쿨타임(기본 1초)
/// - 흔적 최대 개수(기본 1000개) 초과 시 오래된 것부터 삭제
///
/// A방식:
/// - 흔적의 높이(Y)는 "플레이어 기준 고정 높이"로 잡고,
/// - 수평 퍼짐은 "벽을 따라" (월드 Up 기준 tangent),
/// - 수직 퍼짐은 "월드 Up"으로만 랜덤.
/// => 벽 방향(앞/뒤)이나 접촉 상황에 따른 높이 경향(머리/다리 띠) 제거.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class WallContactTrail : MonoBehaviour
{
    [Header("Wall Detection")]
    [Tooltip("Wall 레이어만 선택하세요. (문벽 포함)")]
    public LayerMask wallLayerMask;

    [Header("Mark Prefab")]
    [Tooltip("벽에 찍힐 점(흔적) 프리팹 (Quad 권장). Collider는 제거하세요.")]
    public GameObject markPrefab;

    [Tooltip("벽 표면 z-fighting 방지를 위해 법선 방향으로 살짝 띄우는 값")]
    [Range(0f, 0.1f)]
    public float surfaceOffset = 0.01f;

    [Header("Cooldown")]
    [Tooltip("벽 접촉 패널티/흔적 생성 전역 쿨타임(초)")]
    public float cooldownSeconds = 1.0f;

    [Header("Marks Limit")]
    [Tooltip("동시에 유지할 흔적 최대 개수")]
    public int maxMarks = 1000;

    [Header("Spread Control")]
    [Tooltip("벽을 따라 좌우(수평)로 퍼지는 강도")]
    [Range(0f, 0.2f)]
    public float horizontalSpread = 0.03f;

    [Tooltip("위/아래(Y)로 퍼지는 강도")]
    [Range(0f, 0.2f)]
    public float verticalSpread = 0.03f;

    [Header("Height Basis (A)")]
    [Tooltip("흔적의 높이를 플레이어 기준으로 고정합니다.")]
    public bool useFixedHeight = true;

    [Tooltip("플레이어 기준 고정 높이(미터). 예: 1.2 = 가슴/목 근처")]
    public float fixedHeightFromPlayer = 1.2f;

    [Header("Hierarchy")]
    [Tooltip("Hierarchy 정리를 위해 흔적을 담을 부모 오브젝트를 자동 생성합니다.")]
    public bool createMarksRoot = true;

    [Tooltip("자동 생성되는 부모 오브젝트 이름")]
    public string marksRootName = "TrailMarksRoot";

    private float nextAllowedTime = 0f;
    private readonly Queue<GameObject> marksQueue = new Queue<GameObject>(1024);
    private Transform marksRoot;

    private void Awake()
    {
        if (createMarksRoot)
        {
            GameObject existing = GameObject.Find(marksRootName);
            if (existing != null)
            {
                marksRoot = existing.transform;
            }
            else
            {
                var rootObj = new GameObject(marksRootName);
                marksRoot = rootObj.transform;
            }
        }
    }

    /// <summary>
    /// CharacterController가 충돌한 콜라이더 정보를 받습니다.
    /// (메시가 아니라 CharacterController 캡슐 기준 접촉)
    /// </summary>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 1) 쿨타임 체크(전역)
        if (Time.time < nextAllowedTime)
            return;

        // 2) Wall 레이어만 필터링
        int hitLayer = hit.collider.gameObject.layer;
        if ((wallLayerMask.value & (1 << hitLayer)) == 0)
            return;

        // 3) 트리거는 무시(안전장치)
        if (hit.collider.isTrigger)
            return;

        // 4) 패널티 증가
        if (PenaltyManager.Instance != null)
        {
            PenaltyManager.Instance.AddLocalPenalty();
        }
        else
        {
            Debug.LogWarning("[WallContactTrail] PenaltyManager.Instance is null.");
        }

        // 5) 흔적 생성
        SpawnMark(hit);

        // 6) 쿨타임 갱신
        nextAllowedTime = Time.time + Mathf.Max(0f, cooldownSeconds);
    }

    private void SpawnMark(ControllerColliderHit hit)
    {
        if (markPrefab == null)
            return;

        Vector3 n = hit.normal.normalized;

        // 기본 위치: 접촉점 + 법선 방향으로 살짝 띄움
        Vector3 pos = hit.point + n * surfaceOffset;

        // (A) 높이를 플레이어 기준으로 고정
        if (useFixedHeight)
        {
            pos.y = transform.position.y + fixedHeightFromPlayer;
        }

        // 벽을 따라가는 수평 방향(tangent): 월드 Up 기준으로 만들어 벽 앞/뒤 영향 최소화
        Vector3 tangent = Vector3.Cross(Vector3.up, n);
        if (tangent.sqrMagnitude < 1e-6f)
        {
            // 벽이 거의 up/down을 향하는 특수 케이스 대비
            tangent = Vector3.Cross(Vector3.forward, n);
        }
        tangent.Normalize();

        // 수평(벽을 따라) 퍼짐
        if (horizontalSpread > 0f)
        {
            float side = Random.Range(-horizontalSpread, horizontalSpread);
            pos += tangent * side;
        }

        // 수직(Y) 퍼짐은 월드 Up으로만
        if (verticalSpread > 0f)
        {
            float upDown = Random.Range(-verticalSpread, verticalSpread);
            pos += Vector3.up * upDown;
        }

        // 회전: 벽을 바라보게 해서 Quad가 벽에 붙는 느낌
        Quaternion rot = Quaternion.LookRotation(-n);

        Transform parent = (marksRoot != null) ? marksRoot : null;
        GameObject mark = Instantiate(markPrefab, pos, rot, parent);

        marksQueue.Enqueue(mark);

        // 최대 개수 초과 시 오래된 것부터 제거
        int limit = Mathf.Max(0, maxMarks);
        while (marksQueue.Count > limit)
        {
            GameObject old = marksQueue.Dequeue();
            if (old != null)
                Destroy(old);
        }
    }

    /// <summary>
    /// 외부에서 흔적을 전부 삭제하고 싶을 때 사용(레벨 시작 시 초기화 등)
    /// </summary>
    public void ClearAllMarks()
    {
        while (marksQueue.Count > 0)
        {
            GameObject obj = marksQueue.Dequeue();
            if (obj != null) Destroy(obj);
        }
    }
}
