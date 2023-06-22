using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class for transforming the marker path into a set of positions the drone shall fly to one by one
public class PathFlyer : MonoBehaviour
{
    [SerializeField]
    private MarkerSpawner markerSpawner;

    private ZMQCommands zmqComms;
    private DebugFlyerStarter starter;

    private bool debugMode;
    // Start is called before the first frame update
    void Start()
    {
        debugMode = Debugger.inDebugMode();
        zmqComms = GetComponent<ZMQCommands>();
        starter = GetComponent<DebugFlyerStarter>();
    }


    public void FlyAlongPath()
    {
        if (debugMode)
        {
            Debug.Log("Flying Path in DebugMode");
            DebugFlyAlongPath(markerSpawner.getFlyPath());
            starter.GoPath();
            
        }
        else {
            FlyPath(markerSpawner.getFlyPath());

        }
    }

    //Calculates the positions of the fly path of the drone
    private void FlyPath(List<Vector3> positions, bool andBack = true)
    {
        //start the drone
        zmqComms.SendLiftOffCommand();

        //Fly along the path
        for (int i = 0; i < positions.Count; i++)
        {
            zmqComms.SendFlyToCommand(positions[i]);
        }

        //optionally fly back
        if (andBack)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                //we are already at that position so go a bit down and then fly back
                if (i == 0)
                {
                    zmqComms.SendFlyToCommand(positions[positions.Count - 1 - i] - Vector3.up * 0.1f);
                }

                zmqComms.SendFlyToCommand(positions[positions.Count - 1 - i]);

            }

        }

        //since one position here the start position is not in the path it has to be add manually
        zmqComms.SendReturnToStartCommand(0.5f);

        //then land and turn off
        zmqComms.SendLandCommand();

    }

    //Path for debugging since here a virtual drone flies along the path
    private void DebugFlyAlongPath(List<Vector3> positions, bool andBack = true)
    {
        //start the drone
        starter.EnqueueCommand(FlyCommand.LiftOff());
        

        //Fly along the path
        for (int i = 0; i < positions.Count; i++)
        {
            starter.EnqueueCommand(FlyCommand.FlyTo(positions[i]));
        }

        //optionally fly back
        if (andBack)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                //we are already at that position so go a bit down and then fly back
                if (i == 0)
                {
                    starter.EnqueueCommand(FlyCommand.FlyTo(positions[positions.Count - 1 - i] - Vector3.up * 0.1f));
                }

                starter.EnqueueCommand(FlyCommand.FlyTo(positions[positions.Count - 1 - i]));
 
            }
        }

        //since one position here the start position is not in the path it has to be add manually
        starter.EnqueueReturnToStart();

        //then land and turn off
        starter.EnqueueCommand(FlyCommand.Land());

    }


}
