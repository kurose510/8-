using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    // インスペクターで、通路内にある異変オブジェクト（ポスター等）をすべてドラッグ＆ドロップ
    [SerializeField] private List<GameObject> anomalyGameObjects;

    private List<IAnomalyObject> anomalyObjects = new List<IAnomalyObject>();

    void Awake()
    {
        // 開始時にインターフェースを取得しておく
        foreach (var obj in anomalyGameObjects)
        {
            var anomaly = obj.GetComponent<IAnomalyObject>();
            if (anomaly != null) anomalyObjects.Add(anomaly);
        }
    }

    // 新しい部屋（ループ後）を生成・リセットする関数
    public bool InitializeRoom(int currentStage)
    {
        // 最初の部屋(0番)は絶対に異変なし（確率0%）
        // それ以外は50%の確率で異変が発生
        bool hasAnomaly = (currentStage != 0) && (Random.value > 0.5f);

        if (hasAnomaly)
        {
            // どのオブジェクトに異変を起こすかをランダムに決める（今回は1つだけ異変を起こす例）
            int randomIndex = Random.Range(0, anomalyObjects.Count);

            for (int i = 0; i < anomalyObjects.Count; i++)
            {
                // 選ばれたオブジェクトだけ true を渡し、他は false (正常) にする
                anomalyObjects[i].SetupAnomaly(i == randomIndex);
            }
        }
        else
        {
            // 全て正常に戻す
            foreach (var anomaly in anomalyObjects)
            {
                anomaly.SetupAnomaly(false);
            }
        }

        // 今回の部屋に異変があったかどうかを GameManager に返す
        return hasAnomaly;
    }
}