using UnityEngine;
using UnityEngine.AI;

public class WereWolfVisual : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Wolf Motion Settings")]
    public float strideSpeed = 20f;    // 양보다 훨씬 빠른 발걸음
    public float strideHeight = 0.4f;  // 박차고 오르는 높이
    public float tiltAngle = 20f;      // 좌우로 크게 기우뚱
    public float leanForward = 25f;    // 포식자처럼 앞을 낮게 숙임

    void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent == null || !agent.enabled) return;

        float speed = agent.velocity.magnitude;

        if (speed > 0.1f)
        {
            // 늑대 특유의 거친 상하 운동 (박차고 나가는 느낌)
            float hop = Mathf.Abs(Mathf.Sin(Time.time * strideSpeed)) * strideHeight;

            // 좌우로 미친 듯이 흔들리는 각도
            float tilt = Mathf.Sin(Time.time * strideSpeed * 0.8f) * tiltAngle;

            // 정면 방향 유지하면서 앞뒤/좌우 흔들림 적용
            // Y축(currentY)은 건드리지 않아야 클릭한 곳을 제대로 바라봅니다.
            float currentY = transform.localRotation.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(leanForward, currentY, tilt);
            transform.localPosition = new Vector3(0, hop, 0);
        }
        else
        {
            // 멈췄을 때 씩씩거리는 거친 숨소리 연출
            float breath = Mathf.Sin(Time.time * 4f) * 0.05f;
            transform.localPosition = new Vector3(0, breath, 0);
        }
    }
}