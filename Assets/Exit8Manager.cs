using System.Linq;
using UnityEngine;

public class Exit8Manager : MonoBehaviour
{
    public static Exit8Manager Instance { get; private set; }

    [Header("現在のステージ数（0番出口〜）")]
    [SerializeField] private int currentStage = 0;

    [Header("看板のSpriteRenderer")]
    public SpriteRenderer signboardRenderer;

    [Header("看板の画像リスト(0〜9)")]
    public Sprite[] signboardSprites;

    [Header("異変リスト")]
    public GameObject[] anomalyObjects;

    [Header("異変の発生確率 (0.0 ～ 1.0)")]
    [Range(0f, 1f)]
    public float anomalyProbability = 0.75f;

    [Header("ReverseControl終了時の停止時間")]
    public float stageTransitionPause = 2.0f;

    public bool IsProcessingMove { get; private set; } = false;

    private int lastAnomalyIndex = -1;
    private bool isAnomalyActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateSignboard();
        SetupNextStage();
    }

    public int GetCurrentStage()
    {
        return currentStage;
    }

    private void UpdateSignboard()
    {
        if (signboardRenderer != null && signboardSprites != null)
        {
            if (currentStage >= 0 && currentStage < signboardSprites.Length)
            {
                signboardRenderer.sprite = signboardSprites[currentStage];
            }
        }
    }

    public void OnPlayerMove(bool isRightMove)
    {
        if (IsProcessingMove) return;

        IsProcessingMove = true;

        // ★ステージ移動前に、現在のステージで
        // ReverseControlが発生していたかを記録する
        bool wasReverseControl = false;

        foreach (GameObject anomalyObject in anomalyObjects)
        {
            if (anomalyObject == null) continue;

            AnomalyReverseControl reverseControl =
                anomalyObject.GetComponent<AnomalyReverseControl>();

            if (reverseControl != null)
            {
                wasReverseControl = reverseControl.IsCurrentlyAnomaly();
                break;
            }
        }

        bool isCorrectChoice = false;

        // 【8番出口の判定ロジック】
        if (isAnomalyActive && !isRightMove)
        {
            isCorrectChoice = true;
            Debug.Log("異変を見破って引き返した！正解！");
        }
        else if (!isAnomalyActive && isRightMove)
        {
            isCorrectChoice = true;
            Debug.Log("異変なしで進んだ！正解！");
        }
        else
        {
            isCorrectChoice = false;
            Debug.Log("選択を間違えたため 0番出口 に戻りました。");
        }

        if (isCorrectChoice)
        {
            currentStage++;
            Debug.Log($"進行！ 現在のステージ: {currentStage}");
        }
        else
        {
            currentStage = 0;
            Debug.Log("0番出口に戻ります。");
        }

        UpdateSignboard();

        // 次のステージをセットアップ
        SetupNextStage();

        // ReverseControlの異変だった場合だけ、
        // ステージ切り替え後に停止する
        if (wasReverseControl)
        {
            PlayerMovement player =
                FindAnyObjectByType<PlayerMovement>();

            if (player != null)
            {
                player.ResetReverseAfterDelay(stageTransitionPause);

                Debug.Log(
                    "Anomaly_ReverseControl終了：" +
                    stageTransitionPause +
                    "秒間停止します。"
                );
            }
        }

        IsProcessingMove = false;
    }

    public void SetupNextStage()
    {
        Debug.Log("=== SetupNextStage が呼ばれました ===");
        Debug.Log("現在のステージ：" + currentStage);
        Debug.Log("登録されている異変数：" + anomalyObjects.Length);

        // ★修正点1: .Count を .Length に変更（配列のため）
        for (int i = 0; i < anomalyObjects.Length; i++)
        {
            if (anomalyObjects[i] != null)
            {
                var anomaly = anomalyObjects[i].GetComponent<IAnomalyObject>();
                if (anomaly != null)
                {
                    anomaly.SetupAnomaly(false);
                }
                else
                {
                    anomalyObjects[i].SendMessage("SetupAnomaly", false, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        // 0番ステージは絶対に異変が発生しない
        if (currentStage == 0)
        {
            isAnomalyActive = false; // ★異変フラグをオフにする
            return;
        }

        // 確率判定（設定した確率に応じて異変を発生させるか決定）
        // ※「100%必ず異変を出す」場合はこの条件分岐を消して isAnomalyActive = true にしてください
        isAnomalyActive = Random.value < anomalyProbability;

        // 異変が発生しないターンの場合
        if (!isAnomalyActive)
        {
            Debug.Log("今回のステージ: 異変なし");
            return;
        }

        // 直前と同じ異変にならないようにランダム選出
        int newIndex;
        do
        {
            // ★修正点2: .Count を .Length に変更
            newIndex = Random.Range(0, anomalyObjects.Length);
        }
        while (newIndex == lastAnomalyIndex && anomalyObjects.Length > 1);

        lastAnomalyIndex = newIndex;

        // ★選ばれた異変だけを ON にする
        var selectedAnomaly = anomalyObjects[newIndex].GetComponent<IAnomalyObject>();
        if (selectedAnomaly != null)
        {
            selectedAnomaly.SetupAnomaly(true);
        }
        else
        {
            anomalyObjects[newIndex].SendMessage("SetupAnomaly", true, SendMessageOptions.DontRequireReceiver);
        }

        Debug.Log($"異変発生！ インデックス: {newIndex} ({anomalyObjects[newIndex].name})");
    }

    public void ResetToStageZero()
    {
        currentStage = 0;
        lastAnomalyIndex = -1;
        isAnomalyActive = false;

        UpdateSignboard();
        SetupNextStage();
    }

    public void ConvertToClear()
    {
        currentStage = 9;
        UpdateSignboard();
        Debug.Log("クリア状態に切り替わりました！");
    }
}