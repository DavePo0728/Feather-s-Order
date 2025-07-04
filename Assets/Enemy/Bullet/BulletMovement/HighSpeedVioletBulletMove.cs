using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedVioletBulletMove : MonoBehaviour
{
   // [SerializeField]
   // float MaxSpeed;
    [SerializeField]
    float speed;
    [SerializeField]
    float lifeTime;
    GameObject lockedPlayer;
    public float spreadDuration;
    public float trackDuration;
    private bool isTracking = false;
    [SerializeField]
    public float maxRotationSpeed;
    EnemyBulletData enemyBulletData;
    private void Awake()
    {
        enemyBulletData = Resources.Load<EnemyBulletData>("BulletData/VioletBullet");
        speed = enemyBulletData.speed;
        lifeTime = enemyBulletData.lifeTime;
    }
    public void Initialize(Vector3 direction, GameObject player)
    {
        // 設定初始發射方向
        transform.rotation = Quaternion.LookRotation(direction);
        lockedPlayer = player;
        StartCoroutine(CountDownInactive());
        // 啟動發射流程
        StartCoroutine(StartTrackingAfterDelay());
    }

    void FixedUpdate()
    {
        // 持續向前移動
        transform.Translate(Vector3.forward * speed);
        if (transform.position.z < lockedPlayer.transform.position.z)
        {
            isTracking = false; // 如果子彈位置在玩家後面，停止追蹤
        }
        // 延遲後開始追蹤敵人
        if (isTracking && lockedPlayer != null)
        {
            Vector3 targetDirection = (lockedPlayer.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator StartTrackingAfterDelay()
    {
        yield return new WaitForSeconds(spreadDuration);
        isTracking = true; // 啟動追蹤
        yield return new WaitForSeconds(trackDuration);
        isTracking = false; // 停止追蹤

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Destroy(this.gameObject);
        }
    }
    IEnumerator CountDownInactive()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }
}
