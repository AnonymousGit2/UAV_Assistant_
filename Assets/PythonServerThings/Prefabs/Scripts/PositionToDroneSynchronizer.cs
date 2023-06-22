using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//NOT USED
public class PositionToDroneSynchronizer : MonoBehaviour
{
    // Start is called before the first frame update
    public ZMQCommands commandsSender;
    public float synchDelay;

    private float timer = 0;
    private bool synchEnabled = false;
    private Vector3 lastPos = Vector3.zero;

    public void EnableSynch() {
        synchEnabled = true;
        timer = 0;
    }

    public void StopSynch() {
        synchEnabled = false;
        timer = 0;
    }


    void Start()
    {
        if (commandsSender == null) {
            Debug.LogError("Command Sender needs to be set");

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (synchEnabled) {
            timer += Time.deltaTime;
            if (timer >= synchDelay) {
                //Do the thing
                //Send each time the position of the current object to the drone
                //But only if the position to the last update has changed
                if (lastPos != transform.position)
                {
                    commandsSender.SendFlyToCommand(transform.position);
                }
                lastPos = transform.position;
                timer = 0;
            }
        }
    }
}
