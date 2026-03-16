using UnityEngine;
using UnityEngine.AI;

public class WerewolfControl : MonoBehaviour
{
    private NavMeshAgent agent;
    public LayerMask groundLayer;

    [Header("Wolf Movement Settings")]
    [Range(5f, 30f)] // 슬라이더로 조절할 수 있게 만듭니다.
    public float wolfSpeed = 15f;

    [Range(50f, 200f)]
    public float wolfAcceleration = 120f;

    [Range(500f, 2000f)]
    public float wolfRotationSpeed = 1000f;

    private Animator anim;

    void OnEnable()
    {
        // 부모(Player)의 NavMeshAgent를 가져옵니다.
        agent = GetComponentInParent<NavMeshAgent>();

        if (agent != null)
        {
            agent.ResetPath();
            ApplyStats(); // 켜질 때 스펙 적용
        }
    }

    // 인스펙터의 값을 에이전트에 주입하는 함수
    void ApplyStats()
    {
        agent.speed = wolfSpeed;
        agent.acceleration = wolfAcceleration;
        agent.angularSpeed = wolfRotationSpeed;
    }
    void Update()
    {
        if (agent == null) return;

        // [핵심] 실시간으로 인스펙터 값을 반영합니다.
        // 테스트할 때는 이 코드를 켜두면 아주 편합니다!
        ApplyStats();

        // 마우스 클릭 이동 로직
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}