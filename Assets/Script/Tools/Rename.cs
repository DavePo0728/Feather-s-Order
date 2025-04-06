using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class Rename : MonoBehaviour
{
    public List<GameObject> renameList;
    [SerializeField]
    List<Vector3> originRecord;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < renameList.Count; i++)
        {
            //originRecord.Add(renameList[i].transform.localPosition);
            //renameList[i].transform.position =originRecord[i];
            renameList[i].transform.position = new Vector3(renameList[i].transform.position.x +12f, renameList[i].transform.position.y, renameList[i].transform.position.z);
            //renameList[i].transform.position = new Vector3(renameList[i].transform.position.x+140f, renameList[i].transform.position.y+470f, renameList[i].transform.position.z-600f);
            Debug.Log(renameList[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
