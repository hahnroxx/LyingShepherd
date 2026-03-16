using UnityEngine;
using UnityEngine.AI;

public class WolfVisual : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Run Settings")]
    public float strideSpeed = 15f;    // 발걸음 속도 (양보다 훨씬 빠르게!)
    public float strideHeight = 0.3f;  // 뛰는 높이
    public float tiltAngle = 15f;      // 좌우로 기우뚱하는 각도
    public float leanForward = 10f;    // 달릴 때 앞쪽으로 숙이는 정도

    void Start()
    {
        // 부모 오브젝트의 NavMeshAgent를 가져옵니다.
        agent = GetComponentInParent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent == null) return;
        float speed = agent.velocity.magnitude;

        if (speed > 0.1f)
        {
            float hop = Mathf.Abs(Mathf.Sin(Time.time * strideSpeed)) * strideHeight;
            float tilt = Mathf.Sin(Time.time * strideSpeed * 0.5f) * tiltAngle;

            // 현재 모델이 가진 로컬 Y 회전값을 그대로 가져와서 사용합니다.
            float currentYRotation = transform.localRotation.eulerAngles.y;

            // Y값에 0 대신 currentYRotation을 넣습니다.
            Quaternion targetRotation = Quaternion.Euler(leanForward, currentYRotation, tilt);

            transform.localPosition = new Vector3(0, hop, 0);
            transform.localRotation = targetRotation;
        }
        else
        {
            // 멈췄을 때 (가쁜 숨을 몰아쉬는 연출)
            float breath = Mathf.Sin(Time.time * 2f) * 0.05f;
            transform.localPosition = new Vector3(0, breath, 0);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }
}