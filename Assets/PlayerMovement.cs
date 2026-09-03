using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float idleTimer = 0f;

    // ★追加：操作反転フラグ
    [HideInInspector] public bool isReversed = false;

    // ★追加：一時的な操作不能（硬直）タイマー
    private float stunTimer = 0f;

    // ★足音用のコンポーネントとタイマー
    private AudioSource audioSource;
    [Header("足音の設定")]
    public float footstepInterval = 0.4f;
    private float footstepTimer = 0f;

    [Header("ループ位置設定")]
    public Vector2 initialPosition = new Vector2(-13.31f, -0.75f);
    public Vector2 warpPosition = new Vector2(43.76991f, -0.75f);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer != null) spriteRenderer.enabled = true;

        transform.position = new Vector3(initialPosition.x, initialPosition.y, 0f);
        if (rb != null)
        {
            rb.position = initialPosition;
        }

        footstepTimer = footstepInterval;
    }

    // ★追加：指定した秒数だけプレイヤーを硬直させる関数（異変スクリプトから呼ばれます）
    public void Stun(float duration)
    {
        stunTimer = duration;
        movement = Vector2.zero; // 移動入力をリセット
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetFloat("Speed", 0f);
    }

    // 0番出口の開始位置へプレイヤーを戻す
    public void ResetToInitialPosition()
    {
        movement = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = initialPosition;
        }

        transform.position = new Vector3(
            initialPosition.x,
            initialPosition.y,
            0f
        );

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }

        // 操作反転も解除
        isReversed = false;

        Debug.Log(
            "Playerを0番出口の開始位置へ戻しました。"
        );
    }

    // ★操作反転をON/OFFする
    public void SetControlReversed(bool reversed)
    {
        isReversed = reversed;
    }

    private Coroutine reverseResetCoroutine;

    // ステージ切り替え後、数秒間停止してから操作反転を解除
    public void ResetReverseAfterDelay(float delay)
    {
        if (reverseResetCoroutine != null)
        {
            StopCoroutine(reverseResetCoroutine);
        }

        reverseResetCoroutine = StartCoroutine(
            ResetReverseRoutine(delay)
        );
    }

    private IEnumerator ResetReverseRoutine(float delay)
    {
        // 数秒間プレイヤーを停止
        Stun(delay);

        // 指定時間待つ
        yield return new WaitForSeconds(delay);

        // 操作反転解除
        isReversed = false;

        Debug.Log("操作反転を解除しました。");

        reverseResetCoroutine = null;
    }

    void Update()
    {
        // ★追加：スタン中（硬直中）はタイマーを減らして一切のキー入力を無視する
        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            movement = Vector2.zero;
            return; // これ以降のキー入力やアニメーション処理を行わずに抜ける
        }

        if (Keyboard.current != null)
        {
            float moveX = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;

            // 操作反転処理
            if (isReversed)
            {
                moveX *= -1f;
            }

            movement.x = moveX;

            // 足音の再生処理
            if (moveX != 0f)
            {
                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    if (audioSource != null && audioSource.clip != null)
                    {
                        audioSource.PlayOneShot(audioSource.clip);
                    }
                    footstepTimer = 0f;
                }
            }
            else
            {
                footstepTimer = footstepInterval;
            }

            // アニメーション制御
            if (animator != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(moveX));

                if (moveX == 0f)
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= 10f)
                    {
                        animator.SetTrigger("LookAround");
                        idleTimer = 0f;
                    }
                }
                else
                {
                    idleTimer = 0f;
                }
            }

            // キャラクターの向き（左右反転）
            if (spriteRenderer != null)
            {
                if (moveX < 0) spriteRenderer.flipX = true;
                if (moveX > 0) spriteRenderer.flipX = false;
            }
        }

        // --- 既存の移動処理（キー入力や transform.Translate など） ---
        // (例) transform.Translate(moveInput * moveSpeed * Time.deltaTime, 0, 0);


        // ★ここから追加：0番出口のときの左限度制限
        if (Exit8Manager.Instance != null && Exit8Manager.Instance.GetCurrentStage() == 0)
        {
            // プレイヤーの現在位置を取得
            Vector3 pos = transform.position;

            // X座標が -12.0 より左に行かないように固定する
            if (pos.x < -12.0f)
            {
                pos.x = -12.0f;
                transform.position = pos;
            }
        }
    }

    void FixedUpdate()
    {
        // スタン中は物理移動を行わない
        if (stunTimer > 0f) return;

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // 出口ステージ（9以上）のときのクリア座標チェック
        if (Exit8Manager.Instance != null && Exit8Manager.Instance.GetCurrentStage() >= 9)
        {
            if (rb.position.x >= 53.96975f)
            {
                rb.linearVelocity = Vector2.zero;
                movement = Vector2.zero;

                Debug.Log("クリア座標に到達！クリアシーンへ遷移します。");
                SceneManager.LoadScene("ClearScene");
                return;
            }
        }

        // ループ（右端に到達したとき）
        float currentWarpX = warpPosition.x;
        if (Exit8Manager.Instance != null && Exit8Manager.Instance.GetCurrentStage() >= 9)
        {
            currentWarpX = 9999f;
        }

        if (rb.position.x > currentWarpX)
        {
            if (Exit8Manager.Instance != null && !Exit8Manager.Instance.IsProcessingMove)
            {
                Exit8Manager.Instance.OnPlayerMove(true);
            }

            rb.position = initialPosition;
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 0f);
        }
        // ループ（左端に到達したとき）
        else if (rb.position.x < initialPosition.x)
        {
            if (Exit8Manager.Instance != null && !Exit8Manager.Instance.IsProcessingMove)
            {
                Exit8Manager.Instance.OnPlayerMove(false);
            }

            rb.position = warpPosition;
            transform.position = new Vector3(warpPosition.x, warpPosition.y, 0f);
        }
    }
}