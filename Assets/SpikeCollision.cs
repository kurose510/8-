using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpikeCollision : MonoBehaviour
{
    [Header("暗転フェード設定")]
    public Image fadeImage; // FadePanelをアタッチ

    private bool isFading = false;

    private void Awake()
    {
        // もしInspectorでFadePanelが未設定の場合、自動でCanvasから探す処理
        if (fadeImage == null)
        {
            GameObject fadeObj = GameObject.Find("FadePanel");
            if (fadeObj != null) fadeImage = fadeObj.GetComponent<Image>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 接触したのがPlayerかつ、まだフェード中でない場合
        if (!isFading && other.CompareTag("Player"))
        {
            StartCoroutine(InstantBlackoutAndResetRoutine());
        }
    }

    private IEnumerator InstantBlackoutAndResetRoutine()
    {
        isFading = true;

        // 1. 触れた瞬間に画面を真っ黒にする
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 1.0f;
            fadeImage.color = color;
        }

        // 2. 0.3秒待機
        yield return new WaitForSeconds(0.3f);

        // 3. 0番出口へリセット
        if (Exit8Manager.Instance != null)
        {
            Exit8Manager.Instance.ResetToStageZero();
        }

        // 4. 暗転パネルを透明に戻す
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }

        isFading = false;
    }
}