using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;



#if UNITY_EDITOR
using UnityEditor;
#endif
public class WaveManager : MonoBehaviour
{
    [Space(10)]
    [SerializeField]
    TypeBPathList pathList;
    [SerializeField]
    CustomPathDataList customPathDataList;

    [Header("UI")]
    [SerializeField]
    TMP_Text waveText;
    [Header("Tutorial Settings")]
    public Image tutorialImage;
    [SerializeField]
    bool debug = false;
    public EnemyDataList enemyDataList;
    public EnemyData[] enemyDatas => enemyDataList.enemyDatas;
    public SpawnDataList spawnDataList;
    public SpawnData[] spawnDatas => spawnDataList.spawnDatas;
    //public GunDataList gunDataList;
    public SpawnGroupList spawnNormalGroupList;
    public SpawnEmptyGroupList spawnEmptyGroupList;
    [Header("Recording")]
    [SerializeField] 
    private bool isRecording = false;
    [SerializeField]
    private RecordedWaveData currentRecording;
    [SerializeField] 
    private float recordStartTime;
    [SerializeField] 
    private string savePath = "Assets/WaveRecordings/";
    [SerializeField] 
    private RecordingWaveList recordedWaveDataList;
    [Header("Debug UI")]
    [SerializeField] 
    private bool showDebugUI = true;
    [SerializeField] 
    private Vector2 debugUIPosition = new Vector2(10, 10);
    [SerializeField] 
    private Vector2 debugUISize = new Vector2(300, 200);
    [SerializeField] 
    private GUIStyle debugTextStyle;
    [SerializeField] 
    private bool autoStartRecording = false;
    public List<RecordedWaveData> recordedWaves;
    private int currentRecordingIndex = 0;

    private Dictionary<string, System.Func<IEnumerator>> spawnGroupMap;
    private Dictionary<string, RecordedWaveData> recordedWaveMap;
    private Dictionary<string, SpawnGroup> spawnGroupDictionary;
    private Dictionary<string, SpawnEmptyGroup> spawnEmptyGroupDictionary;
    [SerializeField]
    private WaveSpawnController currentWaveController;
    public ScenesManager scenesManager;
    SoundManager soundManager;
    public bool tutorialMode = false;
    [SerializeField] private InputActionAsset inputActions;
    private InputAction continueAction,pressBAction;
    [SerializeField]
    GameObject preTutorialObject;
    Image preTutorialImage;
    public IEnumerator WaitForContinueInput()
    {
        bool pressed = false;

        void OnPressed(InputAction.CallbackContext ctx) => pressed = true;

        continueAction.performed += OnPressed;

        yield return new WaitUntil(() => pressed);

        // 防止太快觸發下一段
        yield return new WaitForSeconds(0.1f);

        continueAction.performed -= OnPressed;
    }
    public IEnumerator WaitForPressBInput()
    {
        bool pressed = false;

        void OnPressed(InputAction.CallbackContext ctx) => pressed = true;

        pressBAction.performed += OnPressed;

        yield return new WaitUntil(() => pressed);

        // 防止太快觸發下一段
        yield return new WaitForSeconds(0.2f);

        pressBAction.performed -= OnPressed;
    }
    public void PreteachImageFadeIn() 
    { 
        preTutorialObject.SetActive(true); // 確保物件在淡入前是啟用狀態
        StartCoroutine(scenesManager.Fade(preTutorialImage, 0f, 1f,1f));
    }
    public void PreteachImageFadeOut()
    {
        StartCoroutine(scenesManager.Fade(preTutorialImage, 1f, 0f,1f));
        preTutorialObject.SetActive(false); // 確保物件在淡出後是禁用狀態
    }
    public void FadeAndSetTutorialImage(Sprite newSprite)
    {
        // 替換圖片
        tutorialImage.sprite = newSprite;

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
            SpawnType spawnType = record.spawnType;

            if (entry == null || move == null || leave == null)
            {
                Debug.LogWarning("Replay failed: missing behaviour");
                continue;
            }

            NewSpawn(enemyDatas[record.enemyIndex], spawnDatas[record.spawnDataIndex], record.gunData, entry, move, leave, spawnType);
        }
    }
    private void OnGUI()
    {
        if (!showDebugUI) return;

        debugTextStyle.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(debugUIPosition.x, debugUIPosition.y, debugUISize.x, debugUISize.y), GUI.skin.box);

        GUILayout.Label("<b><size=16>🎛️ Wave Debug UI</size></b>", debugTextStyle);

        if (recordedWaveDataList.recordingWaves.Count > 0)
        {
            GUILayout.Label($"當前錄製資料：<b>{currentRecording.name}</b>", debugTextStyle);

            if (GUILayout.Button("🔁 切換錄製資料 (F4)"))
            {
                currentRecordingIndex = (currentRecordingIndex + 1) % recordedWaveDataList.recordingWaves.Count;
                currentRecording = recordedWaveDataList.recordingWaves[currentRecordingIndex];
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

        public static void Record(RecordedWaveData data, int enemyIndex, int spawnIndex, GunDataList gunData, IEntryBehaviour entry, IMoveBehaviour move, ILeaveBehaviour leave,SpawnType spawnType)
        {
            if (!isRecording || data == null) return;

            var record = new RecordedWaveData.SpawnRecord
            {
                timestamp = Time.time - recordStartTime,
                enemyIndex = enemyIndex,
                spawnDataIndex = spawnIndex,
                gunData = gunData,
                entryTypeName = entry.GetType().Name,
                moveTypeName = move.GetType().Name,
                leaveTypeName = leave.GetType().Name,
                spawnType = spawnType
            };
            data.spawnRecords.Add(record);
        }

        public static IEnumerator Replay(RecordedWaveData data, System.Action<int, int, GunDataList, IEntryBehaviour, IMoveBehaviour, ILeaveBehaviour,SpawnType> spawnAction)
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
                SpawnType spawnType = record.spawnType;

                if (entry == null || move == null || leave == null)
                {
                    Debug.LogWarning("Replay failed: could not instantiate behaviours.");
                    continue;
                }

                spawnAction(record.enemyIndex, record.spawnDataIndex, record.gunData, entry, move, leave,spawnType);
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

    public void NewSpawn_WithRecord(int enemyIndex, int spawnIndex, GunDataList gunDataList,
    IEntryBehaviour entry, IMoveBehaviour move, ILeaveBehaviour leave,SpawnType spawnType)
    {

        NewSpawn(enemyDatas[enemyIndex], spawnDatas[spawnIndex],gunDataList , entry, move, leave,spawnType);

        if (!isRecording || currentRecording == null) return;

        var record = new RecordedWaveData.SpawnRecord
        {
            timestamp = Time.time - recordStartTime,
            enemyIndex = enemyIndex,
            spawnDataIndex = spawnIndex,
            gunData = gunDataList,
            entryTypeName = entry.GetType().Name,
            moveTypeName = move.GetType().Name,
            leaveTypeName = leave.GetType().Name,
            spawnType = spawnType
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
            SpawnType spawnType = record.spawnType;

            if (entry == null || move == null || leave == null)
            {
                Debug.LogWarning($"[Replay] Failed to spawn. entry: {entry}, move: {move}, leave: {leave}");
                continue;
            }

            Debug.Log($"[Replay] Spawn enemy at {record.timestamp:F2}s (waited {waitTime:F2}s)");

            NewSpawn(enemyDatas[record.enemyIndex],spawnDatas[record.spawnDataIndex],record.gunData,entry, move, leave,spawnType);
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
        if (spawnGroupDictionary.TryGetValue(key, out var group))
        {
            waveText.text = key;
            yield return group.GenerateGroup(this);
            yield break;
        }
        //// 若在 spawnGroupMap 裡，執行對應 Coroutine
        //if (spawnGroupMap.TryGetValue(key, out var routine))
        //{
        //    waveText.text = key;
        //    yield return routine();
        //    yield break;
        //}
        if(spawnEmptyGroupDictionary.TryGetValue(key, out var emptyGroup))
        {
            waveText.text = key;
            yield return emptyGroup.GenerateGroup(this);
            yield break;
        }
        //若在 recordedWaveMap 裡，撥放錄製波次
        if (recordedWaveMap.TryGetValue(key, out var data))
        {
            waveText.text = key;
            yield return PlayRecordedWave(data);
            yield break;
        }

        Debug.LogWarning($"❌ 沒有找到名稱為 '{key}' 的波次資料！");
    }
    private void Awake()
    {
        //spawnGroupMap = new Dictionary<string, System.Func<IEnumerator>>()
        //{
        //{ "R", SpawnGroup_R },
        //{ "L", SpawnGroup_L },
        //{ "BlackBullet", SpawnGroup_BlackBullet },
        //{ "M_to_LB", SpawnGroup_M_to_LB },
        //{ "CT_to_RB_red", SpawnGroup_CT_to_RB_red },
        //{ "LB_to_RT", SpawnGroup_LB_to_RT },
        //{ "LC_to_R", SpawnGroup_LC_to_R },
        //{ "LT_RB", SpawnGroup_LT_RB },
        //{ "RB_LT", SpawnGroup_RB_LT },
        //{ "RC_to_LC", SpawnGroup_RC_to_LC },
        //{ "R_A3", SpawnGroup_R_A3 },
        //{ "L_RB_B", SpawnGroup_L_RB_B },
        //{ "L_M_R_B2", SpawnGroup_L_M_R_B2 },
        //{ "CT_A", SpawnGroup_CT_A },
        //{ "LT_RB_A", SpawnGroup_LT_RB_A },
        //{ "RB_LT_A", SpawnGroup_RB_LT_A },
        //{"t1",TutorialWave1 },
        //{"t2",TutorialWave2 },
        //{"t3",TutorialWave3 },
        //{"t4",TutorialWave4 },
        //{"t5",TutorialWave5 },
        //{"EmptyWaveDelay",EmptyWaveDelay },
        //{"EmptyWave",EmptyWave },
        //};
        spawnGroupDictionary = new Dictionary<string, SpawnGroup>();
        recordedWaveMap = new Dictionary<string, RecordedWaveData>();
        spawnEmptyGroupDictionary = new Dictionary<string, SpawnEmptyGroup>();
        AddListToDictionary();
        AddRecordListToDictionary();
        AddEmptyWaveListToDictionary();
        //recordedWaveMap = new Dictionary<string, RecordedWaveData>()
        //{
        //    { "RecordWave0", recordedWaves[0] },
        //    { "RecordWave1", recordedWaves[1] },
        //    { "RecordWave2", recordedWaves[2] },
        //    { "RecordWave3", recordedWaves[3] },
        //};
        //spawnGroupDictionary = new Dictionary<string, SpawnGroup>() //新的字典
        //{
        //    { "SpawnT1", spawnNormalGroupList.spawnGroupDatas[0] },
        //    { "SpawnT2", spawnNormalGroupList.spawnGroupDatas[1] },
        //    { "SpawnT3", spawnNormalGroupList.spawnGroupDatas[2] },
        //    { "SpawnT4", spawnNormalGroupList.spawnGroupDatas[3] },
        //    { "SpawnT5", spawnNormalGroupList.spawnGroupDatas[4] },
        //    { "SpawnL2", spawnNormalGroupList.spawnGroupDatas[5] },
        //    { "SpawnR1", spawnNormalGroupList.spawnGroupDatas[6] },
        //    { "SpawnBlackBullet", spawnNormalGroupList.spawnGroupDatas[7] },
        //    //{ "SpawnT9", spawnGroupList.spawnGroupDatas[8] },
        //    //{ "SpawnT10", spawnGroupList.spawnGroupDatas[9] },
        //    //{ "SpawnT11", spawnGroupList.spawnGroupDatas[10] },
        //    //{ "SpawnT12", spawnGroupList.spawnGroupDatas[11] },
        //};
        scenesManager = GameObject.Find("SceneManager").GetComponent<ScenesManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        if (tutorialMode)
        {
            preTutorialObject = GameObject.Find("PreTeach");
            preTutorialImage = preTutorialObject.GetComponent<Image>();
            continueAction = inputActions.FindActionMap("GameScene").FindAction("Continue");
            continueAction.Enable();
            pressBAction = inputActions.FindActionMap("GameScene").FindAction("PressB");
            pressBAction.Enable();
        }
        pathList = Resources.Load<TypeBPathList>("PathData/TypeBPathData/TypeBPathList");
        customPathDataList = Resources.Load<CustomPathDataList>("PathData/TypeCPathList");
        if (debugTextStyle == null)
        {
            debugTextStyle = new GUIStyle(GUI.skin.label);
            debugTextStyle.richText = true;
            debugTextStyle.fontSize = 14;
        }
    }
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
        if(Input.GetKey(KeyCode.LeftShift)&&Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(GetWave("WaveRe_01"));
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(spawnNormalGroupList.spawnGroupDatas[0].GenerateGroup(this));
            //StartCoroutine(TestSpawn());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(spawnNormalGroupList.spawnGroupDatas[1].GenerateGroup(this));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(spawnNormalGroupList.spawnGroupDatas[2].GenerateGroup(this));
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(spawnNormalGroupList.spawnGroupDatas[3].GenerateGroup(this));
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(spawnNormalGroupList.spawnGroupDatas[4].GenerateGroup(this));
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(spawnNormalGroupList.spawnGroupDatas[5].GenerateGroup(this));
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
            if (recordedWaveDataList.recordingWaves.Count == 0) return;
            currentRecordingIndex = (currentRecordingIndex + 1) % recordedWaveDataList.recordingWaves.Count;
            currentRecording = recordedWaveDataList.recordingWaves[currentRecordingIndex];
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
    public enum SpawnType
    {
        TypeA, // 普通路線
        TypeB, // PathCreator  固定路線
        TypeC  // Custom Node Path 順序節點路線
    }
    //spawn A
    public void NewSpawn(EnemyData enemyData,SpawnData spawnData,GunDataList activeGunDataList,IEntryBehaviour entryBehaviour, IMoveBehaviour moveABehaviour, ILeaveBehaviour leaveBehaviour,SpawnType spawnType)
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
        enemyMove.initialParalyzeTime = enemyData.data.paralyzeTime;
        enemyHp.corrupted = enemyData.data.corrupted;
        enemyHp.haveshield = enemyData.data.haveShield;
        enemyHp.maxShieldHp = enemyData.data.shieldHp;
        enemyHp.maxCorruptionValue = enemyData.data.corruptionMaxValue;
        enemyMove.initialParalyzeTime = enemyData.data.paralyzeTime;
        enemyMove.maxParalyzeTime = enemyData.data.paralyzeMaxTime;
        enemyMove.paralyzeAddTime = enemyData.data.paralyzeAddTime;
        enemyMove.paralyzeMaxCount = enemyData.data.paralyzeMaxCount;
        enemyMove.paralyzeTimeStackMultiplier = enemyData.data.paralyzeTimeStackMultiplier;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        EnemyShootingController enemyShootingController = temp.transform.Find("Guns").GetComponent<EnemyShootingController>();
        if (enemyShootingController == null)
        {
            Debug.LogError("EnemyShootingController not found!");
            return;
        }
        enemyShootingController.gunDataList = activeGunDataList;
        if (spawnData.data.curveHeight == 0)
        {
            spawnData.data.curveHeight = Random.Range(-100f, 100f);
        }
        else
        {
            enemyMove.curveHeight = spawnData.data.curveHeight;
        }
        switch (spawnType)
        {
            case SpawnType.TypeA:
                break;
            case SpawnType.TypeB:
                enemyMove.moveB_PathList = CurvePathGenerator.pathInstance.GetCurvePath(pathList.typeBPathCreatorPathList[spawnData.data.pathNum]);
                break;
            case SpawnType.TypeC:
                var path = customPathDataList.customPathDataList[spawnData.data.customPathNum];
                enemyMove.moveC_PathList = Vector3PointGenerator.instance.GetMoveCPathList(path.pathX.Count, path);
                break;
        }
        IEntryBehaviour entry = entryBehaviour;
        IMoveBehaviour move = moveABehaviour;
        ILeaveBehaviour leave = leaveBehaviour;
        enemyMove.SetBehaviours(entry, move, leave);
    }
    public IEntryBehaviour CreateEntryBehaviour(EntryType type)
    {
        return type switch
        {
            EntryType.EntryTypeA => new EntryTypeA(),
            _ => null
        };
    }

    public IMoveBehaviour CreateMoveBehaviour(MoveType type)
    {
        return type switch
        {
            MoveType.MoveTypeA => new MoveTypeA(),
            MoveType.MoveTypeB => new MoveTypeB(),
            MoveType.MoveTypeC => new MoveTypeC(),
            MoveType.MoveTypeD => new MoveTypeD(),
            MoveType.MoveTypeE => new MoveTypeE(),
            _ => null
        };
    }

    public ILeaveBehaviour CreateLeaveBehaviour(LeaveType type)
    {
        return type switch
        {
            LeaveType.LeaveTypeA => new LeaveTypeA(),
            _ => null
        };
    }
    // MoveTypeA :小範圍隨機移動
    // MoveTypeB :固定軌道移動
    // MoveTypeC :大範圍節點移動​
    // MoveTypeD :直進直出
    // MoveTypeE :自訂進停退時間
    IEnumerator EmptyWaveDelay() // 空波次(延遲)
    {
        yield return new WaitForSeconds(10f);
    }
    IEnumerator EmptyWave() // 空波次
    {
        yield return null;
    }
    IEnumerator TutorialWave1() {
        //yield return new WaitForSeconds(5f);
        //NewSpawn_WithRecord(0, 15, new List<int> { 4 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(),SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 16, new List<int> { 4 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //yield return new WaitForSeconds(3f);
        //NewSpawn_WithRecord(0, 15, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 16, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator TutorialWave2() {
        //yield return new WaitForSeconds(4f);
        //NewSpawn_WithRecord(0, 15, new List<int> { 5 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 16, new List<int> { 5 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(2f);
        //NewSpawn_WithRecord(0, 17, new List<int> { 5 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);
        
    }
    IEnumerator TutorialWave3() {
        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 5 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 5 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(3f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 5 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 5 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return null;
    }
    IEnumerator TutorialWave4() {
        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(2, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator TutorialWave5() {
        //NewSpawn_WithRecord(0, 0, new List<int> { 1 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 1 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 1 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 0, new List<int> { 1 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(2, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_R() // R    1
    {
        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_L() // L    2
    {
        //NewSpawn_WithRecord(0, 1, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 1, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 1, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_BlackBullet() // blackbullet   3
    {
        //NewSpawn_WithRecord(0, 2, new List<int> { 1 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(2, 2, new List<int> { 1 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        //NewSpawn_WithRecord(2, 5, new List<int> { 1 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 2, new List<int> { 1 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 2, new List<int> { 1 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(2f);
        //NewSpawn_WithRecord(0, 2, new List<int> { 1 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(2f);
    }
    IEnumerator SpawnGroup_M_to_LB() // M_to_LB red   4
    {
        //NewSpawn_WithRecord(0, 3, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //NewSpawn_WithRecord(2, 9, new List<int> { 0 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        //NewSpawn_WithRecord(0, 5, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_CT_to_RB_red() // CT_to_RB_red   5
    {
        //NewSpawn_WithRecord(0, 6, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //NewSpawn_WithRecord(0, 7, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //NewSpawn_WithRecord(0, 8, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);

    }
    IEnumerator SpawnGroup_LB_to_RT() // LB_to_RT red    6
    {
        //NewSpawn_WithRecord(0, 9, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 9, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 9, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return null;
    }
    IEnumerator SpawnGroup_LC_to_R() // LC_to_R     7
    {
        //NewSpawn_WithRecord(0, 10, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(1, 10, new List<int> { 1 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 10, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return null;
    }
    IEnumerator SpawnGroup_LT_RB() //LT_RB    8
    {
        //NewSpawn_WithRecord(0, 11, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(1, 11, new List<int> { 2 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 11, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return null;
    }
    IEnumerator SpawnGroup_RB_LT() //RB_LT     9
    {
        //NewSpawn_WithRecord(0, 12, new List<int> { 3 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(1, 12, new List<int> { 2 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 12, new List<int> { 3 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return null;
    }
    IEnumerator SpawnGroup_RC_to_LC() //RC_to_LC    10
    {
        //NewSpawn_WithRecord(0, 13, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 13, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 13, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);

    }
    //特殊陣行
    IEnumerator SpawnGroup_R_A3() //R_A3*2    1
    {
    //    NewSpawn_WithRecord(0, 1, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
    //    yield return new WaitForSeconds(0.5f);

    //    NewSpawn_WithRecord(2, 1, new List<int> { 3 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
    //    NewSpawn_WithRecord(2, 2, new List<int> { 3 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
    //    yield return new WaitForSeconds(0.5f);

    //    NewSpawn_WithRecord(0, 1, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator SpawnGroup_L_RB_B() // L_RB_B
    {
        //NewSpawn_WithRecord(0, 11, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(3, 2, new List<int> { 2 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);
        //NewSpawn_WithRecord(0, 11, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return null;
    }
    IEnumerator SpawnGroup_L_M_R_B2() //L_M_R_B2
    {
        //NewSpawn_WithRecord(3, 15, new List<int> { 2 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        
        //NewSpawn_WithRecord(1, 16, new List<int> { 3 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);

        //NewSpawn_WithRecord(3, 17, new List<int> { 2 }, new EntryTypeA(), new MoveTypeA(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);

    }
    
    IEnumerator SpawnGroup_CT_A() // 3
    {
        //NewSpawn_WithRecord(0, 6, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //NewSpawn_WithRecord(0, 7, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //NewSpawn_WithRecord(0, 8, new List<int> { 2 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);

    }
    
    IEnumerator SpawnGroup_LT_RB_A() // 4
    {
        //NewSpawn_WithRecord(0, 1, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 1, new List<int> { 2 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 1, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator SpawnGroup_RB_LT_A() // 5
    {
        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 0, new List<int> { 2 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
        //yield return new WaitForSeconds(0.5f);

        //NewSpawn_WithRecord(0, 0, new List<int> { 0 }, new EntryTypeA(), new MoveTypeD(), new LeaveTypeA(), SpawnType.TypeA);
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
    public void AddEmptyWaveListToDictionary()
    {
        spawnEmptyGroupDictionary.Clear();
        for (int i = 0; i < spawnEmptyGroupList.spawnEmptyGroupDatas.Count; i++)
        {
            if (spawnEmptyGroupDictionary.ContainsKey(spawnEmptyGroupList.spawnEmptyGroupDatas[i].name))
            {
                Debug.LogWarning($"Key '{spawnEmptyGroupList.spawnEmptyGroupDatas[i].name}' already exists in the dictionary.");
                return;
            }
            spawnEmptyGroupDictionary.Add(spawnEmptyGroupList.spawnEmptyGroupDatas[i].name, spawnEmptyGroupList.spawnEmptyGroupDatas[i]);
        }
    }
    public void AddListToDictionary()
    {
        spawnGroupDictionary.Clear();
        for (int i = 0; i < spawnNormalGroupList.spawnGroupDatas.Length; i++)
        {
            if (spawnGroupDictionary.ContainsKey(spawnNormalGroupList.spawnGroupDatas[i].name))
            {
                Debug.LogWarning($"Key '{spawnNormalGroupList.spawnGroupDatas[i].name}' already exists in the dictionary.");
                return;
            }
            spawnGroupDictionary.Add(spawnNormalGroupList.spawnGroupDatas[i].name, spawnNormalGroupList.spawnGroupDatas[i]);
        }
    }public void AddRecordListToDictionary()
    {
        recordedWaveMap.Clear();
        for (int i = 0; i < recordedWaveDataList.recordingWaves.Count; i++)
        {
            if (recordedWaveMap.ContainsKey(recordedWaveDataList.recordingWaves[i].name))
            {
                Debug.LogWarning($"Key '{recordedWaveDataList.recordingWaves[i].name}' already exists in the dictionary.");
                return;
            }
            recordedWaveMap.Add(recordedWaveDataList.recordingWaves[i].name, recordedWaveDataList.recordingWaves[i]);
        }
    }
}
