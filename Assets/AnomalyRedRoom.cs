using UnityEngine;

public class AnomalyRedRoom : MonoBehaviour, IAnomalyObject
{
    [Header("Main Camera の下にある RedFilter オブジェクト")]
    public GameObject redFilterObject;

    public void SetupAnomaly(bool isAnomalyPresent)
    {
        if (redFilterObject != null)
        {
            redFilterObject.SetActive(isAnomalyPresent);
        }

        gameObject.SetActive(isAnomalyPresent);
    }

    private void OnDisable()
    {
        if (redFilterObject != null)
        {
            redFilterObject.SetActive(false);
        }
    }
}