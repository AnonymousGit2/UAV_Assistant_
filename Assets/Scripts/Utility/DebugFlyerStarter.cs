using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Helper for the virtual debug drone
public class DebugFlyerStarter : MonoBehaviour
{
    [SerializeField]
    Transform Startpoint;

    [SerializeField]
    Transform Endpoint;

    [SerializeField]
    DebugFlyer Flyer;

    public void SetStart() {
        Flyer.SetStart(Startpoint.position);
    }
    public void SetTarget()
    {
        Flyer.SetNewTarget(Endpoint.position);
    }
    public void Go()
    {
        Flyer.Fly();
    }

    public void GoPath() {
        Flyer.FlyQueue();
    }

    public void EnqueueCommand(FlyCommand com) {
        Flyer.EnqueueCommand(com);
    }

    public void EnqueueReturnToStart() {
        Flyer.EnqueueCommand(FlyCommand.FlyTo(Flyer.transform.position+Vector3.up*0.3f));
    }

}
