using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("Wolf Cry Settings")]
    public float cryRadius = 15f;    // 거짓말 소리가 들리는 넓은 범위
    public LayerMask sheepLayer;     // 양들이 설정된 레이어

    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        // 스페이스바를 누르면 거짓말 외침 발동
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShoutWolfCry();
        }
    }

    void ShoutWolfCry()
    {
        if (GameManager.Instance.UseWolfCry())
        {
            // 애니메이터의 Shout 트리거를 작동시킵니다.
            anim.SetTrigger("Shout");
            Debug.Log("<color=red>늑대가 나타났다!!!</color>");

            // 1. 일정 반경 내의 모든 'Sheep' 레이어 오브젝트를 찾습니다.
            Collider[] sheepColliders = Physics.OverlapSphere(transform.position, cryRadius, sheepLayer);

            foreach (var col in sheepColliders)
            {
                // GetComponent 대신 GetComponentInParent를 사용하세요!
                SheepAI sheep = col.GetComponentInParent<SheepAI>();
                if (sheep != null)
                {
                    sheep.OnHeardWolfCry(transform.position);
                }
            }
        }
    }

    // 범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cryRadius);
    }
}