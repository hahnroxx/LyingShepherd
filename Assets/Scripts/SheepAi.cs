using UnityEngine;
using UnityEngine.AI;

public class SheepAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    public enum SheepState { Idle, Fleeing, Panic }
    [Header("Current Status")]
    public SheepState currentState = SheepState.Idle;

    [Header("Flee Settings (Normal)")]
    public float detectionRange = 10f;  // 플레이어 감지 거리
    public float fleeDistance = 7f;    // 도망 지점 거리
    public float normalFleeSpeed = 4f;  // 평소 도망 속도

    [Header("Panic Settings (Wolf Cry)")]
    public float panicSpeed = 12f;      // 거짓말 시 속도
    public float panicDuration = 3f;    // 패닉 유지 시간
    private float panicTimer;

    [Header("Flocking Settings")]
    public float flockRange = 8f;       // 주변 양들을 인식하는 범위

    [Header("Visual Effects")]
    public Transform bodyTransform; // 양의 외형(Sphere, Cylinder 등)이 들어있는 부모 오브젝트
    public float hopHeight = 0.5f;   // 점프 높이
    public float hopSpeed = 10f;    // 점프 속도

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 터레인 안착: 시작 시 공중에 떠 있지 않도록 강제 워프 시킵니다.
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 100.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        // 1. 패닉 상태 관리: 시간이 지나면 다시 평상시로 돌아옵니다.
        if (currentState == SheepState.Panic)
        {
            panicTimer -= Time.deltaTime;
            if (panicTimer <= 0) currentState = SheepState.Idle;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 2. 상태에 따른 이동 로직 실행
        if (currentState == SheepState.Panic)
        {
            // [패닉 모드] 뭉쳐서 빠르게 도망
            HandlePanicMovement();
        }
        else if (distanceToPlayer < detectionRange)
        {
            // [일반 도망 모드] 플레이어를 피해 천천히 이동
            currentState = SheepState.Fleeing;
            FleeFromPlayer(normalFleeSpeed);
        }
        else
        {
            currentState = SheepState.Idle;
            if (agent.remainingDistance < 0.1f) agent.speed = 2f;
        }

        HandleProceduralAnimation();
    }

    // 플레이어의 "늑대다!" 외침 신호를 받는 함수입니다.
    public void OnHeardWolfCry(Vector3 crierPosition)
    {
        currentState = SheepState.Panic;
        panicTimer = panicDuration;
    }

    void HandlePanicMovement()
    {
        Vector3 flockCenter = GetFlockCenter(); // 무리의 중심점을 찾습니다.

        // 도망 방향(개인)과 무리의 중심 방향(군집)을 섞어 뭉치게 만듭니다.
        Vector3 fleeDir = (transform.position - player.position).normalized;
        Vector3 groupDir = (flockCenter - transform.position).normalized;

        Vector3 combinedDir = (fleeDir + groupDir * 0.7f).normalized;
        MoveToTarget(transform.position + combinedDir * 12f, panicSpeed);
    }

    void FleeFromPlayer(float speed)
    {
        Vector3 fleeDir = (transform.position - player.position).normalized;
        fleeDir.y = 0; // 고저차에 의한 계산 꼬임 방지
        MoveToTarget(transform.position + fleeDir * fleeDistance, speed);
    }

    void MoveToTarget(Vector3 targetPos, float speed)
    {
        NavMeshHit hit;
        // 터레인 언덕 위아래를 잘 찾도록 범위를 15f로 넉넉히 설정합니다.
        if (NavMesh.SamplePosition(targetPos, out hit, 15f, NavMesh.AllAreas))
        {
            agent.speed = speed;
            agent.SetDestination(hit.position);
            Debug.DrawLine(transform.position, hit.position, currentState == SheepState.Panic ? Color.red : Color.yellow, 0.5f);
        }
    }

    Vector3 GetFlockCenter()
    {
        // 주변 8m 이내의 다른 양들을 찾습니다.
        Collider[] neighbors = Physics.OverlapSphere(transform.position, flockRange);
        Vector3 center = transform.position;
        int count = 0;

        foreach (var col in neighbors)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Sheep"))
            {
                center += col.transform.position;
                count++;
            }
        }
        return count > 0 ? center / (count + 1) : transform.position;
    }

    void HandleProceduralAnimation()
    {
        if (bodyTransform == null) return;

        float speed = agent.velocity.magnitude;

        // 상태가 'Panic'일 때만 통통 튀는 효과를 적용합니다.
        if (currentState == SheepState.Panic && speed > 0.1f)
        {
            // 늑대가 나타났을 때: 높고 빠르게 폴짝폴짝!
            float hop = Mathf.Abs(Mathf.Sin(Time.time * hopSpeed)) * hopHeight;
            bodyTransform.localPosition = new Vector3(0, hop, 0);

            
        }
        else if (currentState == SheepState.Fleeing && speed > 0.05f)
        {
            // 평소 도망칠 때: 튀지 않고 몸을 좌우로 살짝 흔들어 '걷는' 느낌만 줍니다.
            bodyTransform.localPosition = Vector3.zero; // 위아래로 튀지 않음
            float wobble = Mathf.Sin(Time.time * hopSpeed * 2f) * 5f;
            bodyTransform.localRotation = Quaternion.Euler(0, 0, wobble);
        }
        else
        {
            // 정지 상태: 모든 변형을 부드럽게 초기화합니다.
            bodyTransform.localPosition = Vector3.Lerp(bodyTransform.localPosition, Vector3.zero, Time.deltaTime * 5f);
            bodyTransform.localRotation = Quaternion.Lerp(bodyTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            // 성공 피드백 (소리나 파티클)
            Debug.Log("양 보호 성공!");

            // 오브젝트 풀링을 사용한다면 비활성화, 아니면 Destroy
            gameObject.SetActive(false);

            // 여기서 신뢰도를 조금 회복시키거나 점수를 올리는 처리를 합니다.
        }
    }
}