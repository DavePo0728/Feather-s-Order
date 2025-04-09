using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Net;
using TMPro;

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
    [Header("Tutorial Settings")]
    [SerializeField] 
    private Image tutorialImage;

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
    private Dictionary<string, System.Func<IEnumerator>> spawnGroupMap;
    private Dictionary<string, RecordedWaveData> recordedWaveMap;
    [SerializeField]
    private WaveSpawnController currentWaveController;
    ScenesManager scenesManager;
    SoundManager soundManager;


    public IEnumerator FadeAndSetTutorialImage(Sprite newSprite)
    {
        float duration = 0.5f;
        float time = 0f;

        // 淡出
        while (time < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            Color c = tutorialImage.color; // ← 這裡改用 tutorialImage
            c.a = alpha;
            tutorialImage.color = c;
            time += Time.deltaTime;
            yield return null;
        }

        // 替換圖片
        tutorialImage.sprite = newSprite;

        // 淡入
        time = 0f;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / duration);
            Color c = tutorialImage.color;
            c.a = alpha;
            tutorialImage.color = c;
            time += Time.deltaTime;
            yield return null;
        }

        Color finalColor = tutorialImage.color;
        finalColor.a = 1f;
        tutorialImage.color = finalColor;
    }
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
    public IEnumerator GetWave(string key)
    {
        // 若在 spawnGroupMap 裡，執行對應 Coroutine
        if (spawnGroupMap.TryGetValue(key, out var routine))
        {
            yield return routine();
            yield break;
        }

        // 若在 recordedWaveMap 裡，撥放錄製波次
        if (recordedWaveMap.TryGetValue(key, out var data))
        {
            yield return PlayRecordedWave(data);
            yield break;
        }

        Debug.LogWarning($"❌ 沒有找到名稱為 '{key}' 的波次資料！");
    }
    private void Awake()
    {
        spawnGroupMap = new Dictionary<string, System.Func<IEnumerator>>()
    {
        { "R", SpawnGroup_R },
        { "L", SpawnGroup_L },
        { "BlackBullet", SpawnGroup_BlackBullet },
        { "M_to_LB", SpawnGroup_M_to_LB },
        { "CT_to_RB_red", SpawnGroup_CT_to_RB_red },
        { "LB_to_RT", SpawnGroup_LB_to_RT },
        { "LC_to_R", SpawnGroup_LC_to_R },
        { "LT_RB", SpawnGroup_LT_RB },
        { "RB_LT", SpawnGroup_RB_LT },
        { "RC_to_LC", SpawnGroup_RC_to_LC },
        { "R_A3", SpawnGroup_R_A3 },
        { "L_RB_B", SpawnGroup_L_RB_B },
        { "L_M_R_B2", SpawnGroup_L_M_R_B2 },
        { "CT_A", SpawnGroup_CT_A },
        { "LT_RB_A", SpawnGroup_LT_RB_A },
        { "RB_LT_A", SpawnGroup_RB_LT_A },
        {"t1",TutorialWave1 },
        {"t2",TutorialWave2 },
        {"t3",TutorialWave3 },
        {"t4",TutorialWave4 },
        {"t5",TutorialWave5 },
    };
        recordedWaveMap = new Dictionary<string, RecordedWaveData>()
        {
            { "RecordWave0", recordedWaves[0] },
            { "RecordWave1", recordedWaves[1] },
            { "RecordWave2", recordedWaves[2] },
            { "RecordWave3", recordedWaves[3] },
        };
        scenesManager = GameObject.Find("SceneManager").GetComponent<ScenesManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
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

        if (!debug && currentWaveController != null)
        {
            StartCoroutine(currentWaveController.GenerateWave(this));
        }

        if (autoStartRecording && currentRecording != null)
        {
            StartRecording(currentRecording);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(SpawnGroup_R());
            //StartCoroutine(TestSpawn());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(SpawnGroup_L());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(SpawnGroup_BlackBullet());
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(SpawnGroup_M_to_LB());
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(SpawnGroup_CT_to_RB_red());
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(SpawnGroup_LB_to_RT());
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            StartCoroutine(SpawnGroup_LC_to_R());
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartCoroutine(SpawnGroup_LT_RB());
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(SpawnGroup_RB_LT());
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(SpawnGroup_RC_to_LC());
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            StartCoroutine(SpawnGroup_R_A3());
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartCoroutine(SpawnGroup_L_RB_B());
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartCoroutine(SpawnGroup_L_M_R_B2());
        }
        //特殊陣行
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            StartCoroutine(SpawnGroup_CT_A());
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            StartCoroutine(SpawnGroup_LT_RB_A());
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            StartCoroutine(SpawnGroup_RB_LT_A());
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
    public void BGMFadeOut()
    {
        soundManager.BGMFadeOut();
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
        enemyMove.moveTime = enemyData.data.singleMoveTime;
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
        enemyMove.moveTime = enemyData.data.singleMoveTime;
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
        enemyMove.moveTime = enemyData.data.singleMoveTime;
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
    IEnumerator TutorialWave1() {
        yield return new WaitForSeconds(2f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator TutorialWave2() {
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(2f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator TutorialWave3() {
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(3f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
    }
    IEnumerator TutorialWave4() {
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(2, 0, 0, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator TutorialWave5() {
        NewSpawn_WithRecord(0, 0, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 0, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(2, 0, 0, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_R() // R    1
    {
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_L() // L    2
    {
        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_BlackBullet() // blackbullet   3
    {
        NewSpawn_WithRecord(0, 2, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(2, 2, 1, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        NewSpawn_WithRecord(2, 5, 1, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 2, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 2, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(2f);
        NewSpawn_WithRecord(0, 2, 1, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(2f);
    }
    IEnumerator SpawnGroup_M_to_LB() // M_to_LB red   4
    {
        NewSpawn_WithRecord(0, 3, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(2, 9, 0, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 5, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_CT_to_RB_red() // CT_to_RB_red   5
    {
        NewSpawn_WithRecord(0, 6, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 7, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 8, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

    }
    IEnumerator SpawnGroup_LB_to_RT() // LB_to_RT red    6
    {
        NewSpawn_WithRecord(0, 9, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 9, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 9, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
    }
    IEnumerator SpawnGroup_LC_to_R() // LC_to_R     7
    {
        NewSpawn_WithRecord(0, 10, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(1, 10, 1, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 10, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());

    }
    IEnumerator SpawnGroup_LT_RB() //LT_RB    8
    {
        NewSpawn_WithRecord(0, 11, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(1, 11, 2, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 11, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());


    }
    IEnumerator SpawnGroup_RB_LT() //RB_LT     9
    {
        NewSpawn_WithRecord(0, 12, 3, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(1, 12, 2, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 12, 3, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());


    }
    IEnumerator SpawnGroup_RC_to_LC() //RC_to_LC    10
    {
        NewSpawn_WithRecord(0, 13, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 13, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 13, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

    }
    //特殊陣行
    IEnumerator SpawnGroup_R_A3() //R_A3*2    1
    {
        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(2, 1, 3, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        NewSpawn_WithRecord(2, 2, 3, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);


    }
    IEnumerator SpawnGroup_L_RB_B() // L_RB_B
    {
        NewSpawn_WithRecord(0, 11, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(3, 2, 2, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);
        NewSpawn_WithRecord(0, 11, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());

    }
    IEnumerator SpawnGroup_L_M_R_B2() //L_M_R_B2
    {
        NewSpawn_WithRecord(3, 15, 2, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        
        NewSpawn_WithRecord(1, 16, 3, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());

        NewSpawn_WithRecord(3, 17, 2, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

    }
    
    IEnumerator SpawnGroup_CT_A() // 3
    {
        NewSpawn_WithRecord(0, 6, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 7, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        NewSpawn_WithRecord(0, 8, 2, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

    }
    
    IEnumerator SpawnGroup_LT_RB_A() // 4
    {
        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 1, 2, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 1, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator SpawnGroup_RB_LT_A() // 5
    {
        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 0, 2, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

        NewSpawn_WithRecord(0, 0, 0, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA());
        yield return new WaitForSeconds(0.5f);

    }

    public void GameFinish()
    {
        scenesManager.LoadNextScene();
    }
    public int EnemyCount()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
    public void SetTutorialImage(Sprite show)
    {
        tutorialImage.sprite = show;
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
