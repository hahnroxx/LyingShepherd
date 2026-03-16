using UnityEngine;

public class SheepGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 닿은 물체가 'Sheep' 태그를 가지고 있는지 확인합니다.
        if (other.CompareTag("Sheep"))
        {
            // 양을 안전하게 처리합니다.
            SaveSheep(other.gameObject);
        }
    }

    void SaveSheep(GameObject sheep)
    {
        // 1. GameManager에게 알림
        GameManager.Instance.OnSheepSaved();

        // 2. 연출: 양이 사라지기 전에 '메~' 소리를 내거나 파티클을 터뜨리면 좋습니다.
        Debug.Log("양 한 마리가 무사히 우리에 들어왔습니다!");

        // 3. 오브젝트 제거 (또는 비활성화)
        Destroy(sheep);
    }
}