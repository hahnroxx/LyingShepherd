using UnityEngine;
using UnityEngine.AI;

public class SheepSpawner : MonoBehaviour
{
    public GameObject sheepPrefab;
    public int sheepCount = 50; // 스테이지당 양의 수
    public float spawnRange = 100f; // 생성 반경

    void Start()
    {
        for (int i = 0; i < sheepCount; i++)
        {
            // 랜덤한 위치 계산
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0,
                Random.Range(-spawnRange, spawnRange)
            );

            // 터레인 높이에 맞춰 안착시키기
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                Instantiate(sheepPrefab, hit.position, Quaternion.identity);
            }
        }
    }
}