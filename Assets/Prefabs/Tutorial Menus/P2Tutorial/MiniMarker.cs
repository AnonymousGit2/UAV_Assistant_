using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMarker : MonoBehaviour
{
    [SerializeField]
    private GameObject startMarker;

    [SerializeField]
    private GameObject pathMarker;

    [SerializeField]
    private GameObject targetMarker;

    [SerializeField]
    private GameObject connector;

    private List<GameObject> connectorList;

    private void Start()
    {
        connectorList = new List<GameObject>();
    }


    public void ConnectStartToPath() {
        var temp = Instantiate(connector, transform.position, Quaternion.identity);
        connectorList.Add(temp);
        LineRenderer lR = temp.GetComponentInChildren<LineRenderer>();
        lR.widthMultiplier = 0.01f;
        temp.GetComponent<Connect>().setConnection(startMarker.transform, pathMarker.transform);
    }
    public void ConnectPathToTarget()
    {
        var temp = Instantiate(connector, transform.position, Quaternion.identity);
        connectorList.Add(temp);
        LineRenderer lR = temp.GetComponentInChildren<LineRenderer>();
        lR.widthMultiplier = 0.01f;
        temp.GetComponent<Connect>().setConnection(pathMarker.transform, targetMarker.transform);
    }

    private void OnDisable()
    {
        foreach (GameObject go in connectorList) {
            Destroy(go);
        }
    }


}
