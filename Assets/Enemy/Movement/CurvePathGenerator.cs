using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePathGenerator : MonoBehaviour
{
    public static CurvePathGenerator pathInstance;
    [Header("Bezier Curve")]
    Vector3 curveStartPoint;

    Vector3 curveEndPoint;

    Vector3 bezierControlPoint;
    //[SerializeField]
    //float curveHeight;
    [SerializeField]
    float landingRadius;
    [SerializeField]
    int resolution;
    Vector3[] path;
    LineRenderer debugLine;
    [SerializeField]
    Material lineMaterial;
    private void Awake()
    {
        pathInstance = this;

    }
    public void DrawDebugLine(GameObject target)
    {
        debugLine = target.GetComponent<LineRenderer>();
        if (debugLine == null)
        {
            debugLine = target.AddComponent<LineRenderer>();
        }
        if (path == null)
        {
            Debug.Log("Path is null");
            return;
        }
        debugLine.positionCount = path.Length;
        debugLine.startWidth = 1f;
        debugLine.endWidth = 1f;
        debugLine.material = lineMaterial;
        debugLine.SetPositions(path);
        
    }
    public void SetPosition(Transform target,Transform endPoint,float curveHeight)
    {
        curveStartPoint = target.position;
        curveEndPoint = GetLandingPosY(endPoint);
        bezierControlPoint = (curveStartPoint + curveEndPoint) * 0.5f + (Vector3.up * curveHeight);
    }
    Vector3 GetLandingPosY(Transform endTransform)
    {
        Vector3 randomPosition = Random.insideUnitSphere * landingRadius + endTransform.position;
        randomPosition.y = endTransform.position.y;

        return randomPosition;
    }
    public Vector3 GetLandingPosZ(Vector3 endPosition,float landingRadius)
    {
        Vector3 randomPosition = Random.insideUnitSphere * landingRadius + endPosition;
        randomPosition.z = endPosition.z;

        return randomPosition;
    }
    public Vector3[] GetPath()
    {
        path = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            var t = (i + 1) / (float)resolution;
            path[i] = GetBezierPoint(t, curveStartPoint, bezierControlPoint, curveEndPoint);
            //Debug.Log(path[i]);
        }
        return path;
    }
    static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }
}
