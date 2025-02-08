using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveTypeA : ILeaveBehaviour
{
    public Vector3 curveStartPoint;

    public Vector3 curveEndPoint;

    public Vector3 bezierControlPoint;
    //[SerializeField]
    public float curveHeight;
    //[SerializeField]
    public float landingRadius;
    //[SerializeField]
    public int resolution;
    Vector3[] path;
    LineRenderer debugLine;
    public void Leave(EnemyMove enemyMove)
    {

    }
    public void DrawDebugLine(GameObject enemy)
    {
        debugLine = enemy.GetComponent<LineRenderer>();
        if (debugLine == null)
        {
            debugLine = enemy.AddComponent<LineRenderer>();
        }
        if (path == null)
        {
            Debug.Log("Path is null");
            return;
        }
        debugLine.positionCount = path.Length;
        debugLine.SetPositions(path);
    }
    public void SetPosition(GameObject enemy)
    {
        curveStartPoint = enemy.transform.position;
        curveEndPoint = GetLandingPos(enemy.transform);
        bezierControlPoint = (curveStartPoint + curveEndPoint) * 0.5f + (Vector3.up * curveHeight);
    }
    Vector3 GetLandingPos(Transform enemyTranform)
    {
        Vector3 randomPosition = Random.insideUnitSphere * landingRadius + enemyTranform.position;
        randomPosition.y = enemyTranform.position.y;

        return randomPosition;
    }
    public void GetPath()
    {
        path = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            var t = (i + 1) / (float)resolution;
            path[i] = GetBezierPoint(t, curveStartPoint, bezierControlPoint, curveEndPoint);
            //Debug.Log(path[i]);
        }
    }
    static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }
}
