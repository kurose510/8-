using UnityEngine;

public class AnomalySpikeWall : MonoBehaviour, IAnomalyObject
{
    [Header("トゲ壁オブジェクト（子の 壁トラップpsd_0）")]
    public GameObject spikeWallObject;

    [Header("移動・出現設定")]
    public float moveSpeed = 3.5f;

    [Header("出現位置")]
    public float spawnX = 50f;
    public float spawnY = 0.5f;

    [Header("接触時のワープ指定座標")]
    public Vector3 respawnPosition =
        new Vector3(-13.31f, -0.75f, 0f);

    private bool isAnomaly = false;
    private bool isFading = false;

    private FadeInManager fadeManager;

    private void Awake()
    {
        // FadeInManagerを取得
        fadeManager = FindAnyObjectByType<FadeInManager>();
    }

    void Update()
    {
        if (isAnomaly)
        {
            transform.Translate(
                Vector3.left * moveSpeed * Time.deltaTime
            );
        }
    }

    public void SetupAnomaly(bool isActive)
    {
        isAnomaly = isActive;
        isFading = false;

        if (isActive)
        {
            // トゲ壁を出現位置へ
            transform.position =
                new Vector3(spawnX, spawnY, 0f);

            if (spikeWallObject != null)
            {
                spikeWallObject.SetActive(true);
                spikeWallObject.transform.localPosition =
                    Vector3.zero;
            }
        }
        else
        {
            if (spikeWallObject != null)
            {
                spikeWallObject.SetActive(false);
            }
        }

        gameObject.SetActive(isActive);
    }

    // SpikeWallHit.cs から呼ばれる
    public void OnPlayerHit(Collider2D other)
    {
        if (!isAnomaly)
            return;

        if (isFading)
            return;

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("棘壁にプレイヤーが接触しました！");

        isFading = true;

        // FadeInManagerに暗転を依頼
        if (fadeManager != null)
        {
            Debug.Log("暗転開始");

            fadeManager.StartBlackout(() =>
            {
                ResetToStageZero(other.transform);
            });
        }
        else
        {
            Debug.LogWarning(
                "FadeInManagerが見つかりませんでした。"
            );

            // FadeInManagerが無い場合の保険
            ResetToStageZero(other.transform);

            isFading = false;
        }
    }

    // 0番出口へ戻す
    private void ResetToStageZero(Transform playerTransform)
    {
        Debug.Log("棘壁：0番出口へ戻します。");

        // プレイヤーを0番の開始位置へ
        if (playerTransform != null)
        {
            playerTransform.position =
                respawnPosition;
        }

        // ステージを0番へ戻す
        if (Exit8Manager.Instance != null)
        {
            Exit8Manager.Instance.ResetToStageZero();
        }
    }
}