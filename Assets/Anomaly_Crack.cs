using UnityEngine;

public class AnomalyCrack : MonoBehaviour, IAnomalyObject
{
    // Exit8Manager ‚©‚ç‚Ì bool ˆø”‚É‘Î‰‚³‚¹‚é
    public void SetupAnomaly(bool isAnomalyPresent)
    {
        gameObject.SetActive(isAnomalyPresent);
    }
}