using UnityEngine;

public class ExitGoal : MonoBehaviour
{
    // オブジェクト（Trigger）に何かが触れたときに実行されるUnityの基本機能
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 触れてきたオブジェクトのタグが「Player」だったら
        if (collision.CompareTag("Player"))
        {
            // Exit8Managerに「クリアしたよ！」と伝える
            if (Exit8Manager.Instance != null)
            {
                Exit8Manager.Instance.ConvertToClear();
            }
        }
    }
}