using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyNPC : MonoBehaviour, IAnomalyObject
{
    [Header("歩行設定")]
    public float moveSpeed = 2.0f;

    public Vector3 spawnPosition = new Vector3(30f, -0.88f, 0f);

    public Vector3 moveDirection = Vector3.left;

    [Header("歩行者オブジェクト")]
    public GameObject npcObject;

    [Header("暗転用 FadePanel")]
    [SerializeField] private Image fadeImage;

    [Header("暗転時間")]
    public float blackoutTime = 0.5f;

    [Header("接触後のプレイヤー位置")]
    public Vector3 respawnPosition = new Vector3(-13.31f, -0.75f, 0f);

    private bool isAnomaly = false;
    private bool isFading = false;

    private Animator npcAnimator;


    private void Awake()
    {
        if (npcObject != null)
        {
            npcAnimator = npcObject.GetComponent<Animator>();
        }

        // Inspectorで設定されていない場合の予備処理
        if (fadeImage == null)
        {
            GameObject fadeObj = GameObject.Find("FadePanel");

            if (fadeObj != null)
            {
                fadeImage = fadeObj.GetComponent<Image>();
            }
        }

        // 起動時は透明にする
        SetFadeAlpha(0f);
    }


    private void Update()
    {
        if (isAnomaly && npcObject != null)
        {
            npcObject.transform.Translate(
                moveDirection * moveSpeed * Time.deltaTime
            );
        }
    }


    public void SetupAnomaly(bool isAnomalyPresent)
    {
        isAnomaly = isAnomalyPresent;
        isFading = false;

        if (npcObject != null)
        {
            npcObject.SetActive(isAnomalyPresent);

            if (isAnomalyPresent)
            {
                npcObject.transform.position = spawnPosition;

                StartWalkAnimation();
            }
        }

        gameObject.SetActive(isAnomalyPresent);
    }


    private void StartWalkAnimation()
    {
        if (npcAnimator == null && npcObject != null)
        {
            npcAnimator = npcObject.GetComponent<Animator>();
        }

        if (npcAnimator != null)
        {
            npcAnimator.SetFloat("Speed", 1.0f);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAnomaly)
            return;

        if (isFading)
            return;

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("★★★ ドッペルゲンガー接触！ ★★★");

        StartCoroutine(
            BlackoutAndResetRoutine(other.transform)
        );
    }


    private IEnumerator BlackoutAndResetRoutine(
        Transform playerTransform)
    {
        isFading = true;

        // =====================================
        // ① 暗転
        // =====================================

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);

            SetFadeAlpha(1f);

            Debug.Log("ドッペルゲンガー：画面を暗転しました");
        }
        else
        {
            Debug.LogError(
                "FadePanelが設定されていません！"
            );
        }


        // =====================================
        // ② 暗転した状態で少し待つ
        // =====================================

        yield return new WaitForSeconds(blackoutTime);


        // =====================================
        // ③ プレイヤーを0番の位置へ
        // =====================================

        if (playerTransform != null)
        {
            playerTransform.position = respawnPosition;

            Rigidbody2D rb =
                playerTransform.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.position = new Vector2(
                    respawnPosition.x,
                    respawnPosition.y
                );

                rb.linearVelocity = Vector2.zero;
            }
        }


        // =====================================
        // ④ 0番出口へリセット
        // =====================================

        if (Exit8Manager.Instance != null)
        {
            Exit8Manager.Instance.ResetToStageZero();

            Debug.Log(
                "ドッペルゲンガー：0番出口へ戻しました"
            );
        }


        // =====================================
        // ⑤ 少し暗転を維持
        // =====================================

        yield return new WaitForSeconds(0.2f);


        // =====================================
        // ⑥ 暗転解除
        // =====================================

        SetFadeAlpha(0f);

        Debug.Log(
            "ドッペルゲンガー：暗転を解除しました"
        );

        isFading = false;
    }


    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage == null)
            return;

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }


    private void OnDisable()
    {
        // 異変が終了した場合も暗転を解除
        if (fadeImage != null)
        {
            SetFadeAlpha(0f);
        }
    }
}