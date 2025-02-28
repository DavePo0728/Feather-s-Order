using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCircularMove : MonoBehaviour
{
    public Transform pivot;        // 旋轉中心
    public float radius ;           // 圓的半徑
    public float angularSpeed ;    // 旋轉速度（角度/秒）
    public float speed ;     // 沿 Z 軸的移動速度

    private float currentAngle ;    // 當前角度（度）
    Vector3 center;
    public void Initial()
    {
        radius = 5f;
        angularSpeed = 90f;
        speed = 5f;
        currentAngle = 0f;
    }
    void Start()
    {
        center = pivot ? pivot.position : new Vector3(2000,340,0);
    }
    void Update()
    {
        RotateWithObject();
    }
    void CalculateMove()
    {
        // 更新角度（以度為單位）
        currentAngle += angularSpeed * Time.deltaTime;

        // 計算 XY 平面上新位置
        float rad = currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * radius;
        float y = Mathf.Sin(rad) * radius;

        // 更新位置：沿 Z 軸持續向前移動
        float z = transform.position.z + speed * Time.deltaTime;

        transform.position = new Vector3(x, y, z);
    }
    void RotateWithObject()
    {
        

        // 以 center 為中心、以 Vector3.forward (Z 軸) 為軸心旋轉物體
        transform.RotateAround(center, Vector3.forward, angularSpeed * Time.deltaTime);

        // 沿世界座標系的 Z 軸向前移動
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
    }
    void OnDrawGizmos()
    {
        // 繪製圓形軌跡
        const int segments = 60;
        float angleStep = 360f / segments;
        Vector3 prevPos = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;
            Vector3 newPos = new Vector3(x, y, 0f);
            if (i > 0)
            {
                Gizmos.DrawLine(prevPos, newPos);
            }
            prevPos = newPos;
        }
    }
}
