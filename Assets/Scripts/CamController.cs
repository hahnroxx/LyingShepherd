using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // 따라갈 플레이어
    public Vector3 offset = new Vector3(0, 55, -35); // 유지할 거리

    [Header("Free Move Settings")]
    public float freeMoveSpeed = 30f;
    private Vector3 currentManualOffset;
    private bool isFreeMoving = false;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 5f;  // 가장 가까운 거리
    public float maxZoom = 80f; // 가장 먼 거리
    private float currentZoom;

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public float cameraTiltX = 60f; // 위아래 기울기 (Pitch)
    public float cameraTiltZ = 0f;  // 좌우 기울기 (Roll)
    private float currentRotationY = 0f; // 현재 Y축 회전값 (Yaw)

    void Start()
    {
        // 게임 시작 시 인스펙터에 설정된 초기 회전값들을 가져옵니다.
        cameraTiltX = transform.eulerAngles.x;
        currentRotationY = transform.eulerAngles.y;
        cameraTiltZ = transform.eulerAngles.z;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();     // 줌 입력 처리
        HandleRotation(); // 회전 입력 처리

        if (Input.GetMouseButton(1)) // 우클릭 시 자유 이동
        {
            isFreeMoving = true;
            MoveCameraManually();
        }
        else
        {
            isFreeMoving = false;
            FollowPlayer();
        }
    }

    // 스크롤로 줌인/줌아웃
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // offset의 길이를 조절하여 줌 구현
            float newDistance = offset.magnitude - (scroll * zoomSpeed);
            newDistance = Mathf.Clamp(newDistance, minZoom, maxZoom);
            offset = offset.normalized * newDistance;
        }
    }

    // Q/E 키로 카메라 회전
    void HandleRotation()
    {
        // Q/E 키로는 Y축(좌우 방향)만 회전시킵니다.
        if (Input.GetKey(KeyCode.Q)) currentRotationY -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) currentRotationY += rotationSpeed * Time.deltaTime;

        // X, Y, Z 모든 값을 변수로 처리하여 인스펙터 설정이 유지되게 합니다.
        transform.rotation = Quaternion.Euler(cameraTiltX, currentRotationY, cameraTiltZ);
    }

    void FollowPlayer()
    {
        currentManualOffset = Vector3.zero;

        // 회전된 방향에 맞춰 offset 계산
        Quaternion camRotation = Quaternion.Euler(0, currentRotationY, 0);
        Vector3 rotatedOffset = camRotation * offset;

        transform.position = target.position + rotatedOffset;
    }

    void MoveCameraManually()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 현재 카메라가 바라보는 방향을 기준으로 WASD 이동
        Vector3 forward = transform.forward;
        forward.y = 0; // 바닥 평면 이동을 위해 y축 무시
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDir = (right * h) + (forward * v);
        currentManualOffset += moveDir * freeMoveSpeed * Time.deltaTime;

        Quaternion camRotation = Quaternion.Euler(0, currentRotationY, 0);
        transform.position = target.position + (camRotation * offset) + currentManualOffset;
    }
}