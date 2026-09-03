using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnomalyDarkDepth : MonoBehaviour, IAnomalyObject
{
    [Header("暗くする対象の背景スプライト群")]
    public SpriteRenderer[] bgRenderers;

    [Header("色の設定")]
    public Color normalColor = Color.white;
    public Color darkColor = new Color(0.05f, 0.05f, 0.05f, 1.0f);

    [Header("ライト制御（Global Light 2D）")]
    public Light2D globalLight;

    public float normalLightIntensity = 1.0f;
    public float darkLightIntensity = 0.1f;

    public void SetupAnomaly(bool isAnomalyPresent)
    {
        Color targetColor =
            isAnomalyPresent ? darkColor : normalColor;

        if (bgRenderers != null)
        {
            foreach (var bg in bgRenderers)
            {
                if (bg != null)
                {
                    bg.color = targetColor;
                }
            }
        }

        if (globalLight != null)
        {
            globalLight.intensity =
                isAnomalyPresent
                    ? darkLightIntensity
                    : normalLightIntensity;
        }

        gameObject.SetActive(isAnomalyPresent);
    }
}