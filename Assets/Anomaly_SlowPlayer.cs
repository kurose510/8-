using UnityEngine;

public class AnomalySlowPlayer : MonoBehaviour, IAnomalyObject
{
    [Header("ˆÙ•Ï‚ÌˆÚ“®‘¬“x")]
    public float slowSpeed = 3.8f;

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
                isAnomalyPresent ? slowSpeed : normalSpeed;
        }

        gameObject.SetActive(isAnomalyPresent);
    }
}