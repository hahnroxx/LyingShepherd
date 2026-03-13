using UnityEngine;

public class MarkerVisual : MonoBehaviour
{
    public float rotateSpeed = 100f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    void Update()
    {
        // 1. 회전 효과
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        // 2. 둥실거리는 효과 (Sin 함수 활용)
        float newY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(0, 0.5f + newY, 0); // 0.5f는 기본 높이
    }
}
