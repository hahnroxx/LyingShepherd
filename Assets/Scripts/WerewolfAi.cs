using UnityEngine;
using UnityEngine.AI;

public class WerewolfAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public float detectRange = 50f; // 양을 감지하는 범위
    private Transform targetSheep;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 늑대는 무식하게 돌진하니까, 속도를 팍! 높여보자. 
        if (agent != null)
        {
            agent.speed = 10f; // 기존 목동 속도(8f)보다 빠르게!
            agent.acceleration = 100f; // 가속도도 팍!
            agent.angularSpeed = 720f; // 회전도 팍!
        }
    }

    void Update()
    {
        // 타겟 양이 없거나 비활성화되었다면, 다시 찾습니다.
        if (targetSheep == null || !targetSheep.gameObject.activeInHierarchy)
        {
            FindClosestSheep();
        }

        // 타겟 양이 있다면, 그 양을 향해 이동합니다.
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

            // 감지 범위 내에 있는 가장 가까운 양을 찾습니다.
            if (distance < closestDistance && distance <= detectRange)
            {
                closestDistance = distance;
                closestSheep = sheep;
            }
        }

        if (closestSheep != null)
        {
            targetSheep = closestSheep.transform;
            Debug.Log($"<color=red>늑대가 새로운 타겟을 찾았습니다: {targetSheep.name}</color>");
        }
        else
        {
            targetSheep = null;
            Debug.Log("<color=yellow>범위 내에 양이 없습니다.</color>");
        }
    }
}