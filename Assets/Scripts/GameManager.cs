using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public RectTransform trustMaskRect; // Slider 대신 마스크가 붙은 RectTransform을 가져옵니다.
    public TMP_Text sheepText;

    [Header("Game Stats")]
    public int savedSheep = 0;
    public float villageTrust = 1000f;
    public float trustLossPerLie = 20f;

    // 마스크의 최대 가로 길이를 저장합니다.
    private float maxMaskWidth;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 시작할 때 마스크의 전체 가로 길이를 기억해 둡니다.
        if (trustMaskRect != null) maxMaskWidth = trustMaskRect.rect.width;

        UpdateUI();
    }

    public void OnSheepSaved()
    {
        savedSheep++;
        villageTrust = Mathf.Min(100f, villageTrust + 5f);
        UpdateUI();
    }

    public bool UseWolfCry()
    {
        if (villageTrust >= trustLossPerLie)
        {
            villageTrust -= trustLossPerLie;
            UpdateUI();
            return true;
        }
        return false;
    }

    // UI를 최신 정보로 갱신하는 함수
    void UpdateUI()
    {
        // 신뢰도 비율(0~1)에 따라 마스크의 가로 길이를 조절합니다.
        if (trustMaskRect != null)
        {
            float targetWidth = maxMaskWidth * (villageTrust / 1000f);

            // RectTransform의 sizeDelta를 사용해 가로 길이만 변경합니다.
            Vector2 newSize = trustMaskRect.sizeDelta;
            newSize.x = targetWidth;
            trustMaskRect.sizeDelta = newSize;
        }

        if (sheepText != null) sheepText.text = $"Saved: {savedSheep}";
    }
}