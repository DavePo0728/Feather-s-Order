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
    public Vector3PointGenerator vector3PointGenerator;
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

    public void NewSpawn(int enemyIndex, int spawnIndex, GunDataList gunDataList,
    IEntryBehaviour entry, IMoveBehaviour move, ILeaveBehaviour leave, SpawnType spawnType)
    {

        NewSpawn(enemyDatas[enemyIndex], spawnDatas[spawnIndex], gunDataList, entry, move, leave, spawnType);
    }
    public IEnumerator GetWave(string key)
    {
        if (spawnGroupDictionary.TryGetValue(key, out var group))
        {
            waveText.text = key;
            yield return group.GenerateGroup(this);
            yield break;
        }
        if(spawnEmptyGroupDictionary.TryGetValue(key, out var emptyGroup))
        {
            waveText.text = key;
            yield return emptyGroup.GenerateGroup(this);
            yield break;
        }
        Debug.LogWarning($"❌ 沒有找到名稱為 '{key}' 的波次資料！");
    }
    private void Awake()
    {
        spawnGroupDictionary = new Dictionary<string, SpawnGroup>();
        spawnEmptyGroupDictionary = new Dictionary<string, SpawnEmptyGroup>();
        AddListToDictionary();
        AddEmptyWaveListToDictionary();
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
    }
    public void StartGenerateWave()
    {
        if (!debug && currentWaveController != null)
        {
            StartCoroutine(currentWaveController.GenerateWave(this));
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
        if (vector3PointGenerator == null)
        {
            Debug.Log("Vector3PointGenerator找不到");
        }
        //Debug.Log($"[NewSpawn] Spawn enemy: {enemyData.data.enemy.name} at {spawnData.data.spawnPosition}");
        Vector3 spawnPoint = vector3PointGenerator.GetPoint((int)spawnData.data.spawnPosition.x, (int)spawnData.data.spawnPosition.y, (int)spawnData.data.spawnPosition.z);
        Vector3 endPoint = vector3PointGenerator.GetPoint((int)spawnData.data.endPosition.x, (int)spawnData.data.endPosition.y, (int)spawnData.data.endPosition.z);
        Vector3 leavePoint = vector3PointGenerator.GetPoint((int)spawnData.data.LeavePositon.x, (int)spawnData.data.LeavePositon.y, (int)spawnData.data.LeavePositon.z);
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
        enemyHp.corruptionDecreaseTime = enemyData.data.corruptionDecreaseTime;
        enemyHp.corruptionDecreaseSpeed = enemyData.data.corruptionDecreaseSpeed;
        enemyMove.curveType = spawnData.data.curveType;
        enemyMove.endPoint = endPoint;
        enemyMove.leavePoint = leavePoint;
        enemyMove.startAttackPoint = spawnData.data.startAttackPoint;
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
                enemyMove.randomMoveRadius = spawnData.data.randomMoveRadius;
                //Debug.Log(enemyMove.randomMoveRadius);
                break;
            case SpawnType.TypeB:
                enemyMove.moveB_PathList = CurvePathGenerator.pathInstance.GetCurvePath(pathList.typeBPathCreatorPathList[spawnData.data.pathNum]);
                break;
            case SpawnType.TypeC:
                var path = customPathDataList.customPathDataList[spawnData.data.customPathNum];
                enemyMove.moveC_PathList = Vector3PointGenerator.instance.GetMoveCPathList(path.pathX.Count, path);
                enemyMove.loopStartIndex = path.loopStartIndex;
                enemyMove.loopEndIndex = path.loopEndIndex;
                enemyMove.loopTime = path.loopTime;
                enemyMove.loopType = path.loopType;
                foreach (var point in path.moveTimeValues)
                {
                    enemyMove.pointIndex.Add(point.endPointIndex);
                    enemyMove.betweenPointWaitTime.Add(point.pointWaitTime);
                    enemyMove.endPointWaitTime.Add(point.endpointWaitTime);
                    enemyMove.pointMoveTime.Add(point.moveTime);
                }
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
    }
}
