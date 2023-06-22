using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Connector class to draw the red lines between the waypoint marker
public class Connect : MonoBehaviour
{
    private LineRenderer lr;
    public Transform pos1;
    public Transform pos2;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponentInChildren<LineRenderer>();
    }

    public void setConnection(Transform pos1, Transform pos2) {
        this.pos1.parent = pos1;
        this.pos1.position = pos1.position;
        this.pos2.parent = pos2;
        this.pos2.position = pos2.position;
    }

    public void setLooseConnection(Vector3 pos1, Vector3 pos2) {
        this.pos1.position = pos1;
        this.pos2.position = pos2;
    }


    public void removeConnection() { 
    
    }

    public LineRenderer GetLineRenderer() {
        return lr;
    }
    // Update is called once per frame
    void Update()
    {
        if (lr.GetPosition(0) != pos1.position - transform.position) {
            lr.SetPosition(0, pos1.position-transform.position);
        }
        if (lr.GetPosition(1) != pos2.position - transform.position)
        {
            lr.SetPosition(1, pos2.position-transform.position);
        }
    }
}
