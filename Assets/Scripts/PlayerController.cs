using UnityEngine;
using UnityEngine.AI;

public class PlayerClickMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    public LayerMask groundLayer;
    public GameObject markerPrefab;
    private GameObject spawnedMarker;
    public float markerDuration = 1.0f;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    // snappy한 회전을 위해 값을 720 정도로 높게 잡는 걸 추천합니다!
    public float rotationSpeed = 720f;

    // 미끄러짐을 방지하는 핵심 변수입니다.
    public float acceleration = 50f;

    private Animator anim;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // 시작할 때 에이전트의 속도를 설정한 변수값으로 초기화합니다.
        UpdateAgentSettings();

        // 시작할 때 마커를 미리 하나 만들어두고 꺼둡니다.
        if (markerPrefab != null)
        {
            spawnedMarker = Instantiate(markerPrefab);
            spawnedMarker.SetActive(false);
        }
    }

    void Update()
    {
        // 현재 속도를 애니메이터의 Speed 파라미터에 전달합니다.
        // magnitude는 속도의 크기를 의미합니다.
        float velocity = agent.velocity.magnitude;
        anim.SetFloat("Speed", velocity);

        // 실시간으로 인스펙터 값을 반영하고 싶다면 매 프레임 업데이트합니다.
        // 성능을 위해 필요할 때만 호출할 수도 있지만, 1인 개발 단계에서는 편리함이 우선입니다.
        UpdateAgentSettings();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                agent.SetDestination(hit.point);
                // 마커를 클릭 지점으로 옮기고 켭니다.
                ShowMarker(hit.point);
            }
        }
    }

    // 에이전트의 속도 설정을 업데이트하는 함수입니다.
    void UpdateAgentSettings()
    {
        if (agent != null)
        {
            agent.speed = moveSpeed;
            // 아래 두 줄이 추가되어야 미끄러짐과 선회가 해결됩니다!
            agent.angularSpeed = rotationSpeed; // 회전 속도 적용
            agent.acceleration = acceleration; // 가속도 적용
        }
    }

    //void SpawnMarker(Vector3 position)
    //{
    //    if (markerPrefab != null)
    //    {
    //        Vector3 spawnPos = new Vector3(position.x, position.y + 0.1f, position.z);

    //        // 1. Instantiate의 결과를 GameObject 변수에 담습니다.
    //        GameObject marker = Instantiate(markerPrefab, spawnPos, Quaternion.Euler(-90, 0, 0));

    //        // 2. 생성된 그 'marker'를 0.5초 뒤에 파괴하라고 예약합니다.
    //        // 0.5f는 이펙트가 보여지는 시간입니다. 원하는 만큼 조절하세요!
    //        Destroy(marker, 0.5f);
    //    }
    //}

    void ShowMarker(Vector3 position)
    {
        if (spawnedMarker != null)
        {
            // [핵심] 이전의 사라짐 예약을 취소합니다. 
            // 이걸 안 하면 광클할 때 마커가 켜지자마자 사라질 수 있습니다.
            CancelInvoke("HideMarker");

            spawnedMarker.transform.position = position;
            spawnedMarker.SetActive(true);

            // 지정된 시간(markerDuration) 후에 HideMarker 함수를 실행합니다.
            Invoke("HideMarker", markerDuration);
        }
    }

    void HideMarker()
    {
        if (spawnedMarker != null)
        {
            spawnedMarker.SetActive(false);
        }
    }
}