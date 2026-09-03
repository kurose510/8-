using UnityEngine;

public class AnomalyPosterScale : MonoBehaviour, IAnomalyObject
{
    [Header("異変時の設定")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.3f;

    [Header("連動して非表示にする通常オブジェクト")]
    public GameObject targetNormalObject;

    private Vector3 initialScale;

    void Awake()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = initialScale * scale;
    }

    public void SetupAnomaly(bool isAnomalyPresent)
    {
        if (targetNormalObject != null)
        {
            targetNormalObject.SetActive(!isAnomalyPresent);
        }

        gameObject.SetActive(isAnomalyPresent);
    }
}