using UnityEngine;

public class AnomalyStrawDoll : MonoBehaviour, IAnomalyObject
{
    public void SetupAnomaly(bool isAnomalyPresent)
    {
        gameObject.SetActive(isAnomalyPresent);
    }
}