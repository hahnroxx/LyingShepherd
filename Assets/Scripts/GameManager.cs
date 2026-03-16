using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("--- UI References ---")]
    public RectTransform trustMaskRect;
    public RectTransform beastMaskRect;
    public Image redOverlay;
    public TMP_Text sheepText;

    [Header("--- Game Stats ---")]
    public float villageTrust = 100f;       // 현재 신뢰도
    public float maxTrust = 100f;           // 최대 신뢰도 (추가됨)
    public float trustLossPerLie = 20f;     // 거짓말 패널티
    public float trustGainPerSheep = 10f;   // 구출 보너스

    [Header("--- Sheep Progress ---")]
    public int savedSheepCount = 0;         // 구출한 양
    public int totalSheepToSave = 5;        // 목표 수치
    public int currentSheepCount;           // 현재 필드의 양 (생존 수)

    [Header("--- Beast System ---")]
    public float beastGauge = 0f;
    public float maxBeast = 100f;
    public float beastDecayRate = 5f;
    public bool isWerewolf = false;

    [Header("--- Transformation ---")]
    public GameObject shepherdModel;
    public GameObject wolfModel;
    public GameObject transformEffectPrefab;
    public AudioSource audioSource;
    public AudioClip wolfHowlClip;

    [Header("--- Spawn Settings ---")]
    public Transform[] spawnPoints;
    public GameObject wolfPrefab;

    // 내부 계산용 변수
    private float maxTrustMaskWidth;
    private float maxBeastMaskWidth;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        InitializeStats();
        UpdateUI();
        StartCoroutine(SpawnWolfRoutine());
    }

    void InitializeStats()
    {
        // 필드의 양 개수 파악
        currentSheepCount = GameObject.FindGameObjectsWithTag("Sheep").Length;

        // UI 마스크 초기 너비 저장
        if (trustMaskRect != null) maxTrustMaskWidth = trustMaskRect.rect.width;
        if (beastMaskRect != null) maxBeastMaskWidth = beastMaskRect.rect.width;
    }

    void Update()
    {
        HandleBeastGaugeDecay();
    }

    // [기능] 타락 게이지 자동 감소
    void HandleBeastGaugeDecay()
    {
        if (!isWerewolf && beastGauge > 0)
        {
            beastGauge -= beastDecayRate * Time.deltaTime;
            beastGauge = Mathf.Max(beastGauge, 0f);
            UpdateUI();
        }
    }

    // [이벤트] 양 구출 성공 (우리에 도착)
    public void OnSheepSaved()
    {
        savedSheepCount++;
        villageTrust = Mathf.Min(villageTrust + trustGainPerSheep, maxTrust);

        Debug.Log($"양 구출! ({savedSheepCount}/{totalSheepToSave})");

        UpdateUI();
        if (savedSheepCount >= totalSheepToSave) GameClear();
    }

    // [이벤트] 양 잡아먹힘 (늑대인간 상태)
    public void OnSheepEaten()
    {
        currentSheepCount--;
        UpdateUI();

        if (currentSheepCount <= 0) WerewolfVictory();
    }

    // [액션] 거짓말 외침 버튼
    public bool UseWolfCry()
    {
        if (isWerewolf || villageTrust <= 0) return false;

        villageTrust -= trustLossPerLie;
        beastGauge += 15f; // 상승 수치 조정

        // 외침 애니메이션 실행 (컴포넌트가 있다면)
        // shepherdModel.GetComponent<Animator>()?.SetTrigger("Cry");

        if (beastGauge >= maxBeast) StartTransforming();

        UpdateUI();
        return true;
    }

    // [시스템] 변신 로직
    void StartTransforming()
    {
        if (isWerewolf) return;
        isWerewolf = true;

        // 1. 시각/청각 효과
        if (transformEffectPrefab != null)
        {
            Vector3 spawnPos = shepherdModel.transform.position + Vector3.up * 1f;
            Destroy(Instantiate(transformEffectPrefab, spawnPos, Quaternion.identity), 2f);
        }
        audioSource?.PlayOneShot(wolfHowlClip);

        // 2. 모델 스왑
        shepherdModel.SetActive(false);
        wolfModel.SetActive(true);

        // 3. 조종 권한 교체 (부모 Player에서 컴포넌트 제어)
        GetComponent<PlayerClickMovement>().enabled = false;
        GetComponent<WerewolfControl>().enabled = true;

        Debug.Log("<color=red>늑대인간 변신 완료!</color>");
    }

    // [UI] 모든 정보 갱신
    void UpdateUI()
    {
        // 1. 신뢰도 바 (현재값/최대값 비율로 계산하도록 수정)
        if (trustMaskRect != null)
        {
            float ratio = villageTrust / maxTrust;
            trustMaskRect.sizeDelta = new Vector2(maxTrustMaskWidth * ratio, trustMaskRect.sizeDelta.y);
        }

        // 2. 타락 게이지 바
        if (beastMaskRect != null)
        {
            float ratio = beastGauge / maxBeast;
            beastMaskRect.sizeDelta = new Vector2(maxBeastMaskWidth * ratio, beastMaskRect.sizeDelta.y);
        }

        // 3. 화면 붉기 조절
        if (redOverlay != null)
        {
            float alpha = (beastGauge / maxBeast) * 0.6f;
            if (isWerewolf) alpha = 0.6f;
            redOverlay.color = new Color(redOverlay.color.r, redOverlay.color.g, redOverlay.color.b, alpha);
        }

        // 4. 텍스트 정보
        if (sheepText != null)
        {
            sheepText.text = $"Sheep: {savedSheepCount} / {totalSheepToSave}";
        }
    }

    public IEnumerator SpawnWolfRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(15f, 30f));
            if (spawnPoints.Length > 0)
            {
                int idx = Random.Range(0, spawnPoints.Length);
                Instantiate(wolfPrefab, spawnPoints[idx].position, Quaternion.identity);
            }
        }
    }

    void GameClear() => Debug.Log("<color=green>Mission Complete!</color>");
    void WerewolfVictory() => Debug.Log("<color=red>Werewolf Victory!</color>");
}