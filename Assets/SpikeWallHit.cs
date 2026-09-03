using UnityEngine;

public class SpikeWallHit : MonoBehaviour
{
    private AnomalySpikeWall spikeWall;

    private void Awake()
    {
        // êeÇ…Ç†ÇÈ AnomalySpikeWall ÇéÊìæ
        spikeWall = GetComponentInParent<AnomalySpikeWall>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (spikeWall != null)
        {
            spikeWall.OnPlayerHit(other);
        }
    }
}