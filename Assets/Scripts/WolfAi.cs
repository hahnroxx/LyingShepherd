using UnityEngine;
using UnityEngine.AI;

public class WolfAI : MonoBehaviour
{
    public float detectRange = 20f;
    private NavMeshAgent agent;
    private Transform targetSheep;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // 3초마다 타겟을 갱신합니다. (성능 최적화)
        InvokeRepeating("FindClosestSheep", 0f, 3f);
    }

    void Update()
    {
        if (targetSheep != null)
        {
            agent.SetDestination(targetSheep.position);
        }
    }

    void FindClosestSheep()
    {
        // 맵에 있는 모든 'Sheep' 태그를 가진 오브젝트를 찾습니다.
        GameObject[] sheeps = GameObject.FindGameObjectsWithTag("Sheep");
        float closestDistance = Mathf.Infinity;
        GameObject closestSheep = null;

        foreach (GameObject sheep in sheeps)
        {
            float distance = Vector3.Distance(transform.position, sheep.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSheep = sheep;
            }
        }

        if (closestSheep != null) targetSheep = closestSheep.transform;
    }

    // 양과 부딪혔을 때 처리
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            Destroy(other.gameObject); // 양을 잡아먹음
            GameManager.Instance.villageTrust -= 10f; // 신뢰도 하락
            Debug.Log("늑대가 양을 잡아먹었습니다! 신뢰도 하락!");

            // 양을 먹고 나면 일단 퇴장하거나 다음 타겟으로!
        }
    }
}