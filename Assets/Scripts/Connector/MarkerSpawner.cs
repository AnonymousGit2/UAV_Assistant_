using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class for spawning and the Waypoint marker as well as to manage their optics and connections with the red lines
public class MarkerSpawner : MonoBehaviour
{

    public GameObject marker;
    public GameObject connectorGO;

    [SerializeField]
    private CoordinateTransformer coordTransformer;

    private List<GameObject> markerList;
    private List<GameObject> connectorList;

    // Start is called before the first frame update
    void Start()
    {
        markerList = new List<GameObject>();
        connectorList = new List<GameObject>();
        SpawnMarker(coordTransformer.getDroneOrigin()+Vector3.up*0.1f);
    }


    public void SpawnMarker(Vector3 position)
    {
        markerList.Add(Instantiate(marker, position, Quaternion.identity));

        if (markerList.Count == 1) {
            //this was the first marker so it is the StartMarker
            markerList[0].GetComponent<MarkerOptions>().SetStartMarker();
        } else
        if (markerList.Count == 2) {
            //this is the second marker, so it is the target marker, but the startMarker should not be transformed
            markerList[1].GetComponent<MarkerOptions>().SetTargetMarker();

            //Connect the two marker
            var temp = Instantiate(connectorGO, transform.position, Quaternion.identity);
            connectorList.Add(temp);
            temp.GetComponent<Connect>().setConnection(markerList[1].transform, markerList[0].transform);

        } else 
        if (markerList.Count > 1)
        {
           // save the used marker
            var currentMarker = markerList[markerList.Count - 1];
            var prevMarker = markerList[markerList.Count - 2];

            //Every marker after the third will be the targetmarker. The one before returns to be a pathmarker
            currentMarker.GetComponent<MarkerOptions>().SetTargetMarker();
            prevMarker.GetComponent<MarkerOptions>().SetPathMarker();

            //Connect the two marker
            var temp = Instantiate(connectorGO, transform.position, Quaternion.identity);
            connectorList.Add(temp);
            temp.GetComponent<Connect>().setConnection(currentMarker.transform, prevMarker.transform);

        }

    }

    public void RemoveLastMarker() {
        if (markerList.Count == 1)
        {
            
            Destroy(markerList[0]);
            markerList.RemoveAt(0);
        }
        else
        if (markerList.Count == 2)
        {
            Destroy(connectorList[0]);
            connectorList.RemoveAt(0);

            Destroy(markerList[1]);
            markerList.RemoveAt(1);
        }
        else
        if (markerList.Count > 2)
        {
            var lastCon = connectorList.Count - 1;
            Destroy(connectorList[lastCon]);
            connectorList.RemoveAt(lastCon);

            var lastMarker = markerList.Count - 1;
            Destroy(markerList[lastMarker]);
            markerList.RemoveAt(lastMarker);

            markerList[lastMarker - 1].GetComponent<MarkerOptions>().SetTargetMarker();
        }
        else {
            Debug.Log("EmptyList, so nothing to remove");
        }

    }

    public List<Vector3> getFlyPath() {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < markerList.Count; i++) {
            //Skip the first position as this is the Start point
            if (i == 0) {
                continue;
            }
            positions.Add(markerList[i].transform.position);
        }
        return positions;
    }
   
    public void SpawnMarkerHere()
    {
        SpawnMarker(transform.position);
    }
}
