using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

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
    public void SetPosition(Vector3 target, Vector3 endPoint,float curveHeight)
    {
        curveStartPoint = target;
        curveEndPoint = GetLandingPosY(endPoint);
        bezierControlPoint = (curveStartPoint + curveEndPoint) * 0.5f + (Vector3.up * curveHeight);
    }
    Vector3 GetLandingPosY(Vector3 endTransform)
    {
        Vector3 randomPosition = Random.insideUnitSphere * landingRadius + endTransform;
        randomPosition.y = endTransform.y;

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
    public Vector3[] GetCurvePath(PathCreator vertexPath) 
    {
        path = new Vector3[vertexPath.path.NumPoints];
        for (int i = 0; i < vertexPath.path.NumPoints; i++)
        {
            path[i] = vertexPath.path.GetPoint(i);
        }
        //Debug.Log(path.Length);
        return path;
    }
    public VertexPath GeneratePath(Vector2[] points, bool closedPath,Transform root)
    {
        // Create a closed, 2D bezier path from the supplied points array 
        // These points are treated as anchors, which the path will pass through 
        // The control points for the path will be generated automatically 
        BezierPath bezierPath = new BezierPath(points, closedPath, PathSpace.xyz);
        // Then create a vertex path from the bezier path, to be used for movement etc 
        return new VertexPath(bezierPath,root);
    }
}
