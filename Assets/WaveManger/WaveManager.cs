using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.InputSystem;
using System.Net;
#if UNITY_EDITOR
using UnityEditor;

#endif
public class WaveManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemyList;
    [Space(10)]
    [SerializeField]
    List<PathCreator> pathList;
    [SerializeField]
    List<CustomPathData> customPathDataList;

    [Header("UI")]
    [SerializeField]
    GameObject gameoverPanel;
    [SerializeField]
    GameObject gameOverText, GameClearText;
    [SerializeField]
    bool debug = false;
    [SerializeField]
    EnemyDataList enemyDataList;
    EnemyData[] enemyDatas => enemyDataList.enemyDatas;
    [SerializeField]
    SpawnDataList spawnDataList;
    SpawnData[] spawnDatas => spawnDataList.spawnDatas;
    [SerializeField]
    GunDataList gunDataList;
    GunData[] gunDatas =>gunDataList.gunDatas;

    [Header("Recording")]
    [SerializeField] private bool isRecording = false;
    [SerializeField] private RecordedWaveData currentRecording;
    [SerializeField] private float recordStartTime;
    [SerializeField] private string savePath = "Assets/WaveRecordings/";
    [SerializeField] private List<RecordedWaveData> recordedWaveDataList;
    [Header("Debug UI")]
    [SerializeField] private bool showDebugUI = true;
    [SerializeField] private Vector2 debugUIPosition = new Vector2(10, 10);
    [SerializeField] private Vector2 debugUISize = new Vector2(300, 200);
    [SerializeField] private GUIStyle debugTextStyle;
    [SerializeField] private bool autoStartRecording = false;
    [SerializeField] List<RecordedWaveData> recordedWaves;
    private int currentRecordingIndex = 0;

    IEnumerator PlayRecordedWave(RecordedWaveData data, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        float lastTime = 0f;

        foreach (var record in data.spawnRecords)
        {
            float wait = record.timestamp - lastTime;
            if (wait > 0) yield return new WaitForSeconds(wait);
            lastTime = record.timestamp;

            // 建立行為類別
            IEntryBehaviour entry = System.Activator.CreateInstance(System.Type.GetType(record.entryTypeName)) as IEntryBehaviour;
            IMoveBehaviour move = System.Activator.CreateInstance(System.Type.GetType(record.moveTypeName)) as IMoveBehaviour;
            ILeaveBehaviour leave = System.Activator.CreateInstance(System.Type.GetType(record.leaveTypeName)) as ILeaveBehaviour;

            if (entry == null || move == null || leave == null)
            {
                Debug.LogWarning("Replay failed: missing behaviour");
                continue;
            }

            NewSpawn(enemyDatas[record.enemyIndex], spawnDatas[record.spawnDataIndex], gunDatas[record.gunDataIndex], entry, move, leave);
        }
    }
    private void OnGUI()
    {
        if (!showDebugUI) return;

        debugTextStyle.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(debugUIPosition.x, debugUIPosition.y, debugUISize.x, debugUISize.y), GUI.skin.box);

        GUILayout.Label("<b><size=16>🎛️ Wave Debug UI</size></b>", debugTextStyle);

        if (recordedWaveDataList.Count > 0)
        {
            GUILayout.Label($"當前錄製資料：<b>{currentRecording.name}</b>", debugTextStyle);

            if (GUILayout.Button("🔁 切換錄製資料 (F4)"))
            {
                currentRecordingIndex = (currentRecordingIndex + 1) % recordedWaveDataList.Count;
                currentRecording = recordedWaveDataList[currentRecordingIndex];
                Debug.Log($"切換到錄製資料：{currentRecording.name}");
            }

            if (GUILayout.Button("▶️ 播放當前錄製 (F5)"))
            {
                ReplayRecording(currentRecording);
                Debug.Log($"播放錄製資料：{currentRecording.name}");
            }
        }
        else
        {
            GUILayout.Label("⚠️ 無錄製資料");
        }

        GUILayout.Space(10);
        GUILayout.Label($"錄製狀態：<color={(isRecording ? "green" : "red")}><b>{(isRecording ? "錄製中" : "未錄製")}</b></color>", debugTextStyle);

        GUILayout.EndArea();
    }

    public static class RecordingHelper
    {
        public static bool isRecording;
        public static float recordStartTime;
        public static void Start(RecordedWaveData data)
        {
            recordStartTime = Time.time;
            isRecording = true;
            data.spawnRecords.Clear();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(data);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
            Debug.Log("Recording started.");
        }

        public static void Stop(RecordedWaveData data)
        {
            isRecording = false;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(data);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
            Debug.Log("Recording saved.");
        }

        public static void Record(RecordedWaveData data, int enemyIndex, int spawnIndex, int gunIndex, IEntryBehaviour entry, IMoveBehaviour move, ILeaveBehaviour leave)
        {
            if (!isRecording || data == null) return;

            var record = new RecordedWaveData.SpawnRecord
            {
                timestamp = Time.time - recordStartTime,
                enemyIndex = enemyIndex,
                spawnDataIndex = spawnIndex,
                gunDataIndex = gunIndex,
                entryTypeName = entry.GetType().Name,
                moveTypeName = move.GetType().Name,
                leaveTypeName = leave.GetType().Name
            };
            data.spawnRecords.Add(record);
        }

        public static IEnumerator Replay(RecordedWaveData data, System.Action<int, int, int, IEntryBehaviour, IMoveBehaviour, ILeaveBehaviour> spawnAction)
        {
            if (data == null || data.spawnRecords == null || data.spawnRecords.Count == 0)
            {
                Debug.LogWarning("No records to replay");
                yield break;
            }

            float lastTime = 0f;
            foreach (var record in data.spawnRecords)
            {
                float waitTime = record.timestamp - lastTime;
                if (waitTime > 0f)
                    yield return new WaitForSeconds(waitTime);
                lastTime = record.timestamp;

                IEntryBehaviour entry = System.Activator.CreateInstance(System.Type.GetType(record.entryTypeName)) as IEntryBehaviour;
                IMoveBehaviour move = System.Activator.CreateInstance(System.Type.GetType(record.moveTypeName)) as IMoveBehaviour;
                ILeaveBehaviour leave = System.Activator.CreateInstance(System.Type.GetType(record.leaveTypeName)) as ILeaveBehaviour;

                if (entry == null || move == null || leave == null)
                {
                    Debug.LogWarning("Replay failed: could not instantiate behaviours.");
                    continue;
                }

                spawnAction(record.enemyIndex, record.spawnDataIndex, record.gunDataIndex, entry, move, leave);
            }
        }
    }
    public void StartRecording(RecordedWaveData recording)
    {
        // 自動建立資料夾（如果不存在）
        if (!System.IO.Directory.Exists(savePath))
        {
            System.IO.Directory.CreateDirectory(savePath);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh(); // 確保 Unity 看得到新資料夾
#endif
        }

        currentRecording = recording;
        currentRecording.spawnRecords.Clear();
        recordStartTime = Time.time;
        isRecording = true;
        Debug.Log("Recording started.");
    }

    public void StopRecording()
    {
        isRecording = false;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(currentRecording);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        Debug.Log("Recording stopped and saved.");
    }

    public void NewSpawn_WithRecord(int enemyIndex, int spawnIndex, int gunIndex,
    IEntryBehaviour entry, IMoveBehaviour move, ILeaveBehaviour leave)
    {
        NewSpawn(enemyDatas[enemyIndex], spawnDatas[spawnIndex], gunDatas[gunIndex], entry, move, leave);

        if (!isRecording || currentRecording == null) return;

        var record = new RecordedWaveData.SpawnRecord
        {
            timestamp = Time.time - recordStartTime,
            enemyIndex = enemyIndex,
            spawnDataIndex = spawnIndex,
            gunDataIndex = gunIndex,
            entryTypeName = entry.GetType().Name,
            moveTypeName = move.GetType().Name,
            leaveTypeName = leave.GetType().Name
        };

        currentRecording.spawnRecords.Add(record);
    }

    public void ReplayRecording(RecordedWaveData data)
    {
        StartCoroutine(ReplayCoroutine(data));
    }

    private IEnumerator ReplayCoroutine(RecordedWaveData data)
    {
        float lastTimestamp = 0f;

        foreach (var record in data.spawnRecords)
        {
            float waitTime = record.timestamp - lastTimestamp;
            yield return new WaitForSeconds(waitTime);
            lastTimestamp = record.timestamp;

            IEntryBehaviour entry = System.Activator.CreateInstance(System.Type.GetType(record.entryTypeName)) as IEntryBehaviour;
            IMoveBehaviour move = System.Activator.CreateInstance(System.Type.GetType(record.moveTypeName)) as IMoveBehaviour;
            ILeaveBehaviour leave = System.Activator.CreateInstance(System.Type.GetType(record.leaveTypeName)) as ILeaveBehaviour;

            if (entry == null || move == null || leave == null)
            {
                Debug.LogWarning($"[Replay] Failed to spawn. entry: {entry}, move: {move}, leave: {leave}");
                continue;
            }

            Debug.Log($"[Replay] Spawn enemy at {record.timestamp:F2}s (waited {waitTime:F2}s)");

            NewSpawn(
                enemyDatas[record.enemyIndex],
                spawnDatas[record.spawnDataIndex],
                gunDatas[record.gunDataIndex],
                entry, move, leave
            );
        }
    }
    public enum BulletType
    {
        Black,
        Red,
        Purple,
        BlackRed,
    }
    private void Awake()
    {
        //customPathDataList = new List<CustomPathData>();
        //customPathDataList.Add(Resources.Load<CustomPathData>("PathData/PathData1"));
        if (debugTextStyle == null)
        {
            debugTextStyle = new GUIStyle(GUI.skin.label);
            debugTextStyle.richText = true;
            debugTextStyle.fontSize = 14;
        }
    }

    //public BulletType bulletType;
    // Start is called before the first frame update
    void Start()
    {
        isRecording = false; // 強制關閉錄製，避免 Inspector 影響
        if (!debug)
            StartCoroutine(WaveSpawn());

        if (autoStartRecording && currentRecording != null)
        {
            StartRecording(currentRecording);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            StartCoroutine(WaveSpawn());
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(SpawnGroup1_1());
            //StartCoroutine(TestSpawn());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(SpawnGroup1_2());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(SpawnGroup1_3());
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(SpawnGroup1_4());
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(SpawnGroup1_5());
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(SpawnGroup1_6());
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartCoroutine(SpawnGroup1_7());
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(SpawnGroup1_8());
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(SpawnGroup1_9());
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(SpawnGroup1_10());
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            StartCoroutine(SpawnGroup1_11());
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            StartCoroutine(SpawnGroup1_12());
        }
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            StartCoroutine(SpawnGroup1_13());
        }
        // === 錄製控制 ===
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartRecording(currentRecording);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            StopRecording();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ReplayRecording(currentRecording);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            CreateNewRecordingAsset("NewWaveRecording"); // 可以自訂名稱
        }
        // === 切換錄製資料 ===
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (recordedWaveDataList.Count == 0) return;
            currentRecordingIndex = (currentRecordingIndex + 1) % recordedWaveDataList.Count;
            currentRecording = recordedWaveDataList[currentRecordingIndex];
            Debug.Log($"切換到錄製資料：{currentRecording.name}");
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (currentRecording != null)
            {
                ReplayRecording(currentRecording);
                Debug.Log($"播放錄製資料：{currentRecording.name}");
            }
        }
    }
    //spawn A
    void SpawnEnemy(GameObject enemy, float hp,float lifeTime,bool corrupted, float corruptionStack,float paralyzeTime, bool haveShield,float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour,IMoveBehaviour moveABehaviour , ILeaveBehaviour leaveBehaviour,float curveHeight,
        int gunIndex,float rpm,float shootingCoolDown, float bulletAmount, float spinSpeed,BulletType bulletType,float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        EnemyHp enemyHp = temp.GetComponent<EnemyHp>();
        enemyHp.maxHp = hp;
        enemyHp.corrupted = corrupted;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyHp.MaxcorruptionStack = corruptionStack;
        enemyMove.lifeTime = lifeTime;
        enemyMove.paralyzeTime = paralyzeTime;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        //Debug.Log(gunIndex);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry,move,leave);
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed, shootingCoolDown, (EnemyMove.BulletType)bulletType,MaxShootWave);
    }
    //spawn B
    void SpawnEnemy(GameObject enemy, float hp, float lifeTime, bool corrupted, float corruptionStack, float paralyzeTime, bool haveShield, float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour, IMoveBehaviour moveBBehaviour, ILeaveBehaviour leaveBehaviour, float curveHeight, PathCreator CurvePath,
        int gunIndex, float rpm, float shootingCoolDown, float bulletAmount, float spinSpeed, BulletType bulletType, float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        EnemyHp enemyHp = temp.GetComponent<EnemyHp>();
        enemyHp.maxHp = hp;
        enemyHp.corrupted = corrupted;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyHp.MaxcorruptionStack = corruptionStack;
        enemyMove.lifeTime = lifeTime;
        enemyMove.paralyzeTime = paralyzeTime;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        enemyMove.moveB_PathList = CurvePathGenerator.pathInstance.GetCurvePath(CurvePath);
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed,shootingCoolDown,(EnemyMove.BulletType)bulletType, MaxShootWave);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveBBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }
    //spawn C
    void SpawnEnemy(GameObject enemy,float hp,float lifeTime, bool corrupted,float corruptionStack,float paralyzeTime, bool haveShield, float shieldHp,
        Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint, 
        IEntryBehaviour entryBehaviour, IMoveBehaviour moveCBehaviour, ILeaveBehaviour leaveBehaviour, 
        float curveHeight,int pathListLength,float pointWaitTime,int pathListIndex,
        int gunIndex, float rpm, float shootingCoolDown, float bulletAmount,float spinSpeed, BulletType bulletType, float MaxShootWave)
    {
        GameObject temp = Instantiate(enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        EnemyHp enemyHp = temp.GetComponent<EnemyHp>();
        enemyHp.maxHp = hp;
        enemyHp.corrupted = corrupted;
        enemyHp.haveshield = haveShield;
        enemyHp.maxShieldHp = shieldHp;
        enemyHp.MaxcorruptionStack = corruptionStack;
        enemyMove.lifeTime = lifeTime;
        enemyMove.paralyzeTime = paralyzeTime;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.curveHeight = curveHeight;
        enemyMove.moveC_PathList = Vector3PointGenerator.instance.GetMoveCPathList(4, customPathDataList[pathListIndex]);
        //enemyMove.stayTime = (float)pathListLength;
        enemyMove.pointWaitTime = pointWaitTime;
        enemyMove.ActiveGun(gunIndex, rpm, bulletAmount, spinSpeed,shootingCoolDown, (EnemyMove.BulletType)bulletType, MaxShootWave);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveCBehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }
    void NewSpawn(EnemyData enemyData,SpawnData spawnData,GunData gunData,IEntryBehaviour entryBehaviour, IMoveBehaviour moveABehaviour, ILeaveBehaviour leaveBehaviour)
    {
        Vector3 spawnPoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.spawnPosition.x, (int)spawnData.data.spawnPosition.y, (int)spawnData.data.spawnPosition.z);
        Vector3 endPoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.endPosition.x, (int)spawnData.data.endPosition.y, (int)spawnData.data.endPosition.z);
        Vector3 leavePoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.LeavePositon.x, (int)spawnData.data.LeavePositon.y, (int)spawnData.data.LeavePositon.z);
        GameObject temp = Instantiate(enemyData.data.enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        EnemyHp enemyHp = temp.GetComponent<EnemyHp>();
        enemyHp.maxHp = enemyData.data.hp;
        enemyMove.lifeTime = enemyData.data.lifeTime;
        enemyMove.entryTime = enemyData.data.entryTime;
        enemyMove.moveTime = enemyData.data.moveTime;
        enemyMove.leaveTime = enemyData.data.leaveTime;
        enemyMove.pointWaitTime = spawnData.data.pointWaitTime;
        enemyMove.paralyzeTime = enemyData.data.paralyzeTime;
        enemyHp.corrupted = enemyData.data.corrupted;
        enemyHp.haveshield = enemyData.data.haveShield;
        enemyHp.maxShieldHp = enemyData.data.shieldHp;
        enemyHp.MaxcorruptionStack = enemyData.data.corruptionStack;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        if(spawnData.data.curveHeight == 0)
        {
            spawnData.data.curveHeight = Random.Range(-100f, 100f);
        }
        else
        {
            enemyMove.curveHeight = spawnData.data.curveHeight;
        }
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
        enemyMove.ActiveGun(gunData.data.gunIndex, gunData.data.rpm, gunData.data.bulletAmount, gunData.data.spinSpeed, gunData.data.shootingCoolDown, (EnemyMove.BulletType)gunData.data.bulletType, gunData.data.MaxShootWave);
    }
    void NewSpawnB(EnemyData enemyData, SpawnData spawnData, GunData gunData, IEntryBehaviour entryBehaviour, IMoveBehaviour moveABehaviour, ILeaveBehaviour leaveBehaviour)
    {
        Vector3 spawnPoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.spawnPosition.x, (int)spawnData.data.spawnPosition.y, (int)spawnData.data.spawnPosition.z);
        Vector3 endPoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.endPosition.x, (int)spawnData.data.endPosition.y, (int)spawnData.data.endPosition.z);
        Vector3 leavePoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.LeavePositon.x, (int)spawnData.data.LeavePositon.y, (int)spawnData.data.LeavePositon.z);
        GameObject temp = Instantiate(enemyData.data.enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        EnemyHp enemyHp = temp.GetComponent<EnemyHp>();
        enemyHp.maxHp = enemyData.data.hp;
        enemyMove.lifeTime = enemyData.data.lifeTime;
        enemyMove.entryTime = enemyData.data.entryTime;
        enemyMove.moveTime = enemyData.data.moveTime;
        enemyMove.leaveTime = enemyData.data.leaveTime;
        enemyMove.paralyzeTime = enemyData.data.paralyzeTime;
        enemyHp.corrupted = enemyData.data.corrupted;
        enemyHp.haveshield = enemyData.data.haveShield;
        enemyHp.maxShieldHp = enemyData.data.shieldHp;
        enemyHp.MaxcorruptionStack = enemyData.data.corruptionStack;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        if (spawnData.data.curveHeight == 0)
        {
            spawnData.data.curveHeight = Random.Range(-100f, 100f);
        }
        else
        {
            enemyMove.curveHeight = spawnData.data.curveHeight;
        }
        enemyMove.moveB_PathList = CurvePathGenerator.pathInstance.GetCurvePath(pathList[spawnData.data.pathNum]);
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
        enemyMove.ActiveGun(gunData.data.gunIndex, gunData.data.rpm, gunData.data.bulletAmount, gunData.data.spinSpeed, gunData.data.shootingCoolDown, (EnemyMove.BulletType)gunData.data.bulletType, gunData.data.MaxShootWave);
    }
    void NewSpawnC(EnemyData enemyData, SpawnData spawnData, GunData gunData, IEntryBehaviour entryBehaviour, IMoveBehaviour moveABehaviour, ILeaveBehaviour leaveBehaviour)
    {
        Vector3 spawnPoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.spawnPosition.x, (int)spawnData.data.spawnPosition.y, (int)spawnData.data.spawnPosition.z);
        Vector3 endPoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.endPosition.x, (int)spawnData.data.endPosition.y, (int)spawnData.data.endPosition.z);
        Vector3 leavePoint = Vector3PointGenerator.instance.GetPoint((int)spawnData.data.LeavePositon.x, (int)spawnData.data.LeavePositon.y, (int)spawnData.data.LeavePositon.z);
        GameObject temp = Instantiate(enemyData.data.enemy, spawnPoint, Quaternion.identity);
        EnemyMove enemyMove = temp.GetComponent<EnemyMove>();
        EnemyHp enemyHp = temp.GetComponent<EnemyHp>();
        enemyHp.maxHp = enemyData.data.hp;
        enemyMove.lifeTime = enemyData.data.lifeTime;
        enemyMove.entryTime = enemyData.data.entryTime;
        enemyMove.moveTime = enemyData.data.moveTime;
        enemyMove.leaveTime = enemyData.data.leaveTime;
        enemyMove.paralyzeTime = enemyData.data.paralyzeTime;
        enemyHp.corrupted = enemyData.data.corrupted;
        enemyHp.haveshield = enemyData.data.haveShield;
        enemyHp.maxShieldHp = enemyData.data.shieldHp;
        enemyHp.MaxcorruptionStack = enemyData.data.corruptionStack;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        if (spawnData.data.curveHeight == 0)
        {
            spawnData.data.curveHeight = Random.Range(-100f, 100f);
        }
        else
        {
            enemyMove.curveHeight = spawnData.data.curveHeight;
        }
        CustomPathData path = customPathDataList[spawnData.data.customPathNum];
        enemyMove.moveC_PathList = Vector3PointGenerator.instance.GetMoveCPathList(path.pathX.Count, path);
        enemyMove.pointWaitTime = spawnData.data.pointWaitTime;
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
        enemyMove.ActiveGun(gunData.data.gunIndex, gunData.data.rpm, gunData.data.bulletAmount, gunData.data.spinSpeed, gunData.data.shootingCoolDown, (EnemyMove.BulletType)gunData.data.bulletType, gunData.data.MaxShootWave);
    }
    /*A_01: done
    SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
    new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
    */
    /*A_01S: done
     SpawnEnemy(enemyList[0],5,true,5, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f),3,300,1f,6,0,BulletType.Black);
     */
    /*A_02: done
     SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeB(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],4,300,2.5f,4,0,BulletType.purple);
     */
    /*A_02S: done
     SpawnEnemy(enemyList[0],5,true,5, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],4,300,2.5f,4,0,BulletType.purple);
     */
    /*A_03: done
     SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
     */
    /*A_04: done
      SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
     */
    /*A_05:
     SpawnEnemy(enemyList[0],5,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
     new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), Random.Range(-100f, 100f),1,300,1.5f,3,0,BulletType.Red);
    */
    /*B_01:
    SpawnEnemy(enemyList[1],20,false,0, Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0), Vector3PointGenerator.instance.GetPoint(17, 3, 0),
    new EntryTypeA(), new MoveTypeC(), new LeaveTypeA(), Random.Range(-100f, 100f),pathList[0],5,300,2.5f,4,2,BulletType.red);
    */

    /*SpawnEnemy(GameObject enemy, float hp, bool haveShield,float shieldHp,
    Vector3 spawnPoint, Vector3 endPoint, Vector3 leavePoint,
    IEntryBehaviour entryBehaviour,IMoveBehaviour moveBehaviour, ILeaveBehaviour leaveBehaviour,float curveHeight,
    int gunIndex,float rpm,float shootingCoolDown, float bulletAmount, float spinSpeed, BulletType bulletType)
        */
    // MoveTypeA :小範圍隨機移動
    // MoveTypeB :固定軌道移動
    // MoveTypeC :大範圍節點移動​
    // MoveTypeD :直進直出
    // MoveTypeE :自訂進停退時間
    IEnumerator TestSpawn()
    {
        NewSpawn(enemyDatas[0], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());   ///MoveTypeA  Or   MovetypeD

        NewSpawn(enemyDatas[3], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());   ///MoveTypeA  Or   MovetypeD
        NewSpawn(enemyDatas[4], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeE(), new LeaveTypeA());   ///MoveTypeA  Or   MovetypeD

        yield return new WaitForSeconds(0.1f);
        NewSpawnB(enemyDatas[1], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeB(), new LeaveTypeA());   //MoveTypeB
        yield return new WaitForSeconds(0.1f);
        NewSpawnC(enemyDatas[2], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeC(), new LeaveTypeA());   //MoveTypeC
    }
    IEnumerator SpawnGroup1_1() // R
    {
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup1_2() // L
    {
        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup1_3() // blackbullet
    {
        NewSpawn_WithRecord(0, 2, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 2, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 2, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup1_4() // M_to_LB red
    {
        NewSpawn_WithRecord(0, 3, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 4, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 5, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup1_5() // CT_to_RB red
    {
        NewSpawn_WithRecord(0, 6, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 7, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 8, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

    }
    IEnumerator SpawnGroup1_6() // LB_to_RT red
    {
        NewSpawn_WithRecord(0, 9, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 9, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 9, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
    }
    IEnumerator SpawnGroup1_7() // LC_to_R
    {
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());

    }
    IEnumerator SpawnGroup1_8()
    {
        NewSpawn(enemyDatas[0], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);


    }
    IEnumerator SpawnGroup1_9()
    {
        NewSpawn(enemyDatas[0], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);


    }
    IEnumerator SpawnGroup1_10()
    {
        NewSpawn(enemyDatas[0], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);

    }
    IEnumerator SpawnGroup1_11()
    {
        NewSpawn(enemyDatas[0], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);

    }
    IEnumerator SpawnGroup1_12()
    {
        NewSpawn(enemyDatas[0], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);

    }
    IEnumerator SpawnGroup1_13()
    {
        NewSpawn(enemyDatas[0], spawnDatas[0], gunDatas[0], new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.1f);

    }
    IEnumerator WaveSpawn()
    {
        //wave1
        
        StartCoroutine(PlayRecordedWave(recordedWaves[0]));

        yield return new WaitForSeconds(2f);
        StartCoroutine(PlayRecordedWave(recordedWaves[1]));

        yield return new WaitForSeconds(3f);
        StartCoroutine(PlayRecordedWave(recordedWaves[2]));

        yield return new WaitForSeconds(1f);
        StartCoroutine(SpawnGroup1_1());

        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnGroup1_2());

        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_1());

        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnGroup1_2());
        //wave2
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnGroup1_3());

         

        //GameFinish();
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_1());

        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_2());

        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_3());

        //yield return new WaitForSeconds(6f);
        //StartCoroutine(SpawnGroup1_4());
        //StartCoroutine(SpawnGroup1_1());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_5());
        //StartCoroutine(SpawnGroup1_2());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_3());
        //yield return new WaitForSeconds(6f);
        //StartCoroutine(SpawnGroup1_6());
        //yield return new WaitForSeconds(3f);
        //StartCoroutine(SpawnGroup1_1());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine (SpawnGroup1_2());
        //StartCoroutine (SpawnGroup1_4());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_7());
        //yield return new WaitForSeconds(3f);
        //StartCoroutine(SpawnGroup1_8());
        //StartCoroutine(SpawnGroup1_2());
        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_9());
        //yield return new WaitForSeconds(3f);
        //StartCoroutine(SpawnGroup1_10());
        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_11());
        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_4());
        //StartCoroutine(SpawnGroup1_1());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_12());
        //StartCoroutine(SpawnGroup1_5());
        //yield return new WaitForSeconds(6f);
        //StartCoroutine(SpawnGroup1_13());
        //StartCoroutine(SpawnGroup1_7());
        //yield return new WaitForSeconds(13f);
        ////wave 2
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_1());
        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_2());
        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_3());
        //yield return new WaitForSeconds(6f);
        //StartCoroutine(SpawnGroup1_4());
        //StartCoroutine(SpawnGroup1_1());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_5());
        //StartCoroutine(SpawnGroup1_2());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_3());
        //yield return new WaitForSeconds(6f);
        //StartCoroutine(SpawnGroup1_6());
        //yield return new WaitForSeconds(3f);
        //StartCoroutine(SpawnGroup1_1());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_2());
        //StartCoroutine(SpawnGroup1_4());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_7());
        //yield return new WaitForSeconds(3f);
        //StartCoroutine(SpawnGroup1_8());
        //StartCoroutine(SpawnGroup1_2());
        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_9());
        //yield return new WaitForSeconds(3f);
        //StartCoroutine(SpawnGroup1_10());
        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_11());
        //yield return new WaitForSeconds(5f);
        //StartCoroutine(SpawnGroup1_4());
        //StartCoroutine(SpawnGroup1_1());
        //yield return new WaitForSeconds(4f);
        //StartCoroutine(SpawnGroup1_12());
        //StartCoroutine(SpawnGroup1_5());
        //yield return new WaitForSeconds(6f);
        //StartCoroutine(SpawnGroup1_13());
        //StartCoroutine(SpawnGroup1_7());
        //yield return new WaitForSeconds(11f);
        //GameFinish();
    }
    public void GameFinish()
    {
        gameoverPanel.SetActive(true);
        gameOverText.SetActive(false);
        GameClearText.SetActive(true);
        Time.timeScale = 0.0f;
    }
    public void CreateNewRecordingAsset(string fileName)
    {
#if UNITY_EDITOR
        string fullPath = savePath + fileName + ".asset";

        // 如果已經存在，就不再建立
        var existing = AssetDatabase.LoadAssetAtPath<RecordedWaveData>(fullPath);
        if (existing != null)
        {
            Debug.LogWarning($"Recording asset already exists: {fullPath}");
            currentRecording = existing;
            return;
        }

        var newRecording = ScriptableObject.CreateInstance<RecordedWaveData>();
        AssetDatabase.CreateAsset(newRecording, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created new RecordedWaveData asset at: {fullPath}");
        currentRecording = newRecording;
#else
    Debug.LogError("CreateNewRecordingAsset only works in the Unity Editor.");
#endif
    }

}
