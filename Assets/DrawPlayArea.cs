using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPlayArea : MonoBehaviour
{
    [SerializeField]
    Camera playerCamera,Cam2;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(transform.position.x,transform.position.y-5f,transform.position.z+500), new Vector3(360,60,1000));
        if(Cam2 != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(new Vector3(Cam2.transform.position.x, Cam2.transform.position.y - 5f, Cam2.transform.position.z + 500), new Vector3(360, 60, 1000));

        }
    }
}
