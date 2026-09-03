using UnityEngine;

public class AnomalyFastPlayer : MonoBehaviour, IAnomalyObject
{
    [Header("ˆÙ•Ï‚ÌˆÚ“®‘¬“x")]
    public float fastSpeed = 6.5f;

    [Header("’Êí‚ÌˆÚ“®‘¬“x")]
    public float normalSpeed = 5.0f;

    private PlayerMovement player;

    public void SetupAnomaly(bool isAnomalyPresent)
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMovement>();
        }

        if (player != null)
        {
            player.moveSpeed =
                isAnomalyPresent ? fastSpeed : normalSpeed;
        }

        gameObject.SetActive(isAnomalyPresent);
    }
}