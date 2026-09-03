using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInManager : MonoBehaviour
{
    [Header("フェード用パネル（黒いImage）")]
    public Image fadePanel;

    [Header("明けるのにかける時間（秒）")]
    public float fadeDuration = 1.0f;

    private Coroutine fadeCoroutine;

    void Start()
    {
        if (fadePanel != null)
        {
            // 最初は真っ黒
            Color color = fadePanel.color;
            color.a = 1f;
            fadePanel.color = color;

            fadePanel.gameObject.SetActive(true);

            // ゲーム開始時のフェードイン
            fadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            if (fadePanel != null)
            {
                Color color = fadePanel.color;
                color.a = Mathf.Clamp01(1f - (timer / fadeDuration));
                fadePanel.color = color;
            }

            yield return null;
        }

        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;

            // 明るくなったら非表示
            fadePanel.gameObject.SetActive(false);
        }

        fadeCoroutine = null;
    }

    // 異変に触れたときの「暗転 → 処理 → 明転」

    public void StartBlackout(System.Action onBlackout)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(BlackoutRoutine(onBlackout));
    }

    private IEnumerator BlackoutRoutine(System.Action onBlackout)
    {
        if (fadePanel == null)
        {
            yield break;
        }

        // FadePanelを必ず表示
        fadePanel.gameObject.SetActive(true);

        // 完全な黒
        Color color = fadePanel.color;
        color.a = 1f;
        fadePanel.color = color;

        Debug.Log("暗転しました");

        // 黒画面を少し維持
        yield return new WaitForSeconds(0.3f);

        // 黒画面のまま0番への処理
        if (onBlackout != null)
        {
            onBlackout.Invoke();
        }

        // 黒画面から明るくする
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            if (fadePanel != null)
            {
                color = fadePanel.color;
                color.a = Mathf.Clamp01(1f - (timer / fadeDuration));
                fadePanel.color = color;
            }

            yield return null;
        }

        if (fadePanel != null)
        {
            color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;

            fadePanel.gameObject.SetActive(false);
        }

        Debug.Log("暗転解除");

        fadeCoroutine = null;
    }
}