using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPlayArea : MonoBehaviour
{
    [SerializeField]
    Camera playerCamera,Cam2;
    [SerializeField]
    bool parkour;
    private void OnDrawGizmos()
    {
        if(playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - 6.5f, transform.position.z + 500), new Vector3(90, 23, 1000));
        }
        if (Cam2 != null)
        {
            Gizmos.color = Color.cyan;
            if (parkour)
            {
                Gizmos.DrawWireCube(new Vector3(transform.position.x + 5f, transform.position.y + 10f, transform.position.z + 500), new Vector3(34, 26, 1000));
            }
            else
            {
                Gizmos.DrawWireCube(new Vector3(Cam2.transform.position.x + 5f, Cam2.transform.position.y + 13.5f, Cam2.transform.position.z + 500), new Vector3(34, 26, 1000));

            }
        }
    }
}
