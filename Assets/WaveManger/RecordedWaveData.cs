using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecordedWaveData", menuName = "Wave/RecordedWaveData")]
public class RecordedWaveData : ScriptableObject
{
    [System.Serializable]
    public class SpawnRecord
    {
        [Tooltip("從錄製開始後的經過時間（秒）")]
        public float timestamp;

        [Tooltip("EnemyDataList 中的索引")]
        public int enemyIndex;

        [Tooltip("SpawnDataList 中的索引")]
        public int spawnDataIndex;

        [Tooltip("GunDataList 中的索引")]
        public int gunDataIndex;

        [Tooltip("敵人進場的行為腳本類型名稱")]
        public string entryTypeName;

        [Tooltip("敵人移動的行為腳本類型名稱")]
        public string moveTypeName;

        [Tooltip("敵人離場的行為腳本類型名稱")]
        public string leaveTypeName;
    }

    [Header("錄製時產生的敵人生成紀錄")]
    public List<SpawnRecord> spawnRecords = new List<SpawnRecord>();

    // 防止空物件未初始化時報錯（選擇性）
    private void OnEnable()
    {
        if (spawnRecords == null)
            spawnRecords = new List<SpawnRecord>();
    }
}
