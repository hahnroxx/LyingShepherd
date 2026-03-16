using UnityEngine;

public class WerewolfAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 늑대인간 상태일 때만 양을 잡아먹습니다.
        if (GameManager.Instance.isWerewolf && other.CompareTag("Sheep"))
        {
            EatSheep(other.gameObject);
        }
    }

    void EatSheep(GameObject sheep)
    {
        // 1. 양 제거 (펑! 하는 복셀 파편 이펙트를 넣으면 더 좋습니다)
        Destroy(sheep);

        // 2. 게임 매니저에게 양이 죽었음을 알림
        GameManager.Instance.OnSheepEaten();

        // 3. 연출: 화면을 아주 잠깐 더 붉게 만들거나 늑대 크기를 키울 수도 있습니다.
        Debug.Log("양을 잡아먹었습니다! 남은 양이 줄어듭니다.");
    }
}