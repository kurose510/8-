using UnityEngine;

public class AnomalyReverseControl : MonoBehaviour, IAnomalyObject
{
    [Header("操作反転が発生するX座標")]
    public float reverseX = 17f;

    [Header("1フレームでこれ以上移動した場合はワープとみなす")]
    public float maxNormalMoveDistance = 2f;

    private bool isAnomaly = false;
    private bool hasTriggered = false;

    private float previousPlayerX;

    public void SetupAnomaly(bool isAnomalyPresent)
    {
        isAnomaly = isAnomalyPresent;
        hasTriggered = false;

        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();

        if (player != null)
        {
            previousPlayerX = player.transform.position.x;
        }

        Debug.Log(
            "AnomalyReverseControl SetupAnomaly : "
            + isAnomalyPresent
        );

        gameObject.SetActive(isAnomalyPresent);
    }

    public bool IsCurrentlyAnomaly()
    {
        return isAnomaly;
    }

    private void Update()
    {
        if (!isAnomaly)
            return;

        if (hasTriggered)
            return;

        PlayerMovement player =
            FindAnyObjectByType<PlayerMovement>();

        if (player == null)
            return;

        float currentX = player.transform.position.x;

        // 前回位置から今回位置までの移動量
        float deltaX = currentX - previousPlayerX;

        // ステージ切り替えなどによる大きなワープは無視
        if (Mathf.Abs(deltaX) > maxNormalMoveDistance)
        {
            previousPlayerX = currentX;
            return;
        }

        // 左 → 右へX=17を通過
        bool crossedFromLeft =
            previousPlayerX < reverseX &&
            currentX >= reverseX;

        // 右 → 左へX=17を通過
        bool crossedFromRight =
            previousPlayerX > reverseX &&
            currentX <= reverseX;

        if (crossedFromLeft || crossedFromRight)
        {
            Debug.Log("操作反転発生！");
            Debug.Log(
                "Player X座標：" + currentX
            );

            player.SetControlReversed(true);

            hasTriggered = true;
        }

        // 今回の位置を保存
        previousPlayerX = currentX;
    }
}