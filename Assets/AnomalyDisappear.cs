using UnityEngine;

public class AnomalyDisappear : MonoBehaviour, IAnomalyObject
{
    [Header("異変発生時に非表示にする通常オブジェクト")]
    public GameObject targetNormalObject;

    public void SetupAnomaly(bool isAnomalyPresent)
    {
        // 異変オブジェクト自体のON/OFF
        gameObject.SetActive(isAnomalyPresent);

        // 通常オブジェクトのON/OFF
        if (targetNormalObject != null)
        {
            targetNormalObject.SetActive(!isAnomalyPresent);
        }
    }
}