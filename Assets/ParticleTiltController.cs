using UnityEngine;

public class ParticleTiltController : MonoBehaviour
{
    public ParticleSystem ps;
    public Transform playerTransform;
    public float maxTiltOffset = 30f; // 左右最大偏移角度（角度制）
    public float baseAngle = 90f;     // 預設朝向角度

    void Update()
    {
        if (ps == null || playerTransform == null) return;

        // 玩家在本地 X 軸的移動速度（左右）
        float horizontalSpeed = playerTransform.InverseTransformDirection(
            playerTransform.GetComponent<Rigidbody>().velocity
        ).x;

        // 計算 Y 軸傾斜角度
        float tiltOffset = Mathf.Clamp(horizontalSpeed * maxTiltOffset, -maxTiltOffset, maxTiltOffset);
        float totalTiltAngle = baseAngle + tiltOffset;

        // 轉成弧度給 Particle System
        float tiltRadiansY = totalTiltAngle * Mathf.Deg2Rad;

        var main = ps.main;
        main.startRotationY = new ParticleSystem.MinMaxCurve(tiltRadiansY);
    }
}
