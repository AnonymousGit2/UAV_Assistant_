using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Debug drone handler. Manages position and targets.
public class DebugFlyer : MonoBehaviour
{
    private bool isFlying;
    private bool isFlyingBack;
    private Vector3 start;
    private Vector3 target;
    private Vector3 flightStart;
    float timeElapsed;
    float lerpDuration = 3;

    [SerializeField]
    CoordinateTransformer coordinateTransformer;
    
    
    Queue<FlyCommand> commandqueue;

    private void Start()
    {
        commandqueue = new Queue<FlyCommand>();
        SetStart(coordinateTransformer.getDroneOrigin());
    }


    public void SetStart(Vector3 pos) {
        start = pos;
        transform.position = start;
    }

    public void SetNewTarget(Vector3 pos) {
        target = pos;
        transform.LookAt(target);
    }

    public void Fly() {
        FlyElegant();
    }

    //Fly to a position but not directly, instead first up then hover over it then a bit down and then the same way back.
    private void FlyElegant(bool andBack = true) {
        isFlying = true;
        flightStart = transform.position;

        //start the drone
        EnqueueCommand(FlyCommand.FlyTo(start + new Vector3(0, target.y - start.y + 0.3f, 0)));

        EnqueueCommand(FlyCommand.FlyTo(target + new Vector3(0,  0.3f, 0)));

        //since one position here the start position is not in the path it has to be add manually
        EnqueueCommand(FlyCommand.FlyTo(target));

        EnqueueCommand(FlyCommand.FlyTo(target + new Vector3(0,  0.3f, 0)));

        EnqueueCommand(FlyCommand.FlyTo(start + new Vector3(0, target.y - start.y + 0.3f, 0)));
        EnqueueCommand(FlyCommand.FlyTo(start));

        //then land and turn off
        isFlyingBack = true;
        FlyCommand currentCom = commandqueue.Dequeue();
        target = DecryptCommand(currentCom);
        transform.LookAt(target);


    }

    //Fly alongside the positions available in the queue
    public void FlyQueue() {
        flightStart = transform.position;
        isFlying = true;
        start = transform.position;
        FlyCommand currentCom = commandqueue.Dequeue();
        target = DecryptCommand(currentCom);
        transform.LookAt(target);
    }

    //Handle the different low level commands and convert them to position commands for the virtual drone
    private Vector3 DecryptCommand(FlyCommand com) {
        switch (com.command) {
            case CommandType.Start:
                return transform.position + Vector3.up * 0.3f;
            case CommandType.FlyToPos:
                return com.position;
            case CommandType.Land:
                isFlyingBack = true;
                return transform.position - Vector3.up * 0.3f;
            default:
                return Vector3.zero;
        }
    
    }

   //Fly behaviour
    void Update()
    {
        //if yes the drone started or is flying
        if (isFlying)
        {
            //the target the drone is flying onto
            if (timeElapsed < lerpDuration)
            {
                LerpPosition(Mathf.Lerp(0, 1, timeElapsed / lerpDuration));
                timeElapsed += Time.deltaTime;
            }
            //the target is reached but in the queue is another target
            else if(commandqueue.Count >0) {
                LerpPosition(1);
                start = target;
                target = DecryptCommand(commandqueue.Dequeue());
                timeElapsed = 0;
            } 
            //all commands are done and the drone is at the last point now
            //this is skipped if the last command was the land command
            else if (!isFlyingBack)
            {
                LerpPosition(1);
                isFlyingBack = true;
                start = target;
                target = flightStart;
                transform.LookAt(target);
                timeElapsed = 0;
            }
            //all commands are done and the drone is back or landed. Reset everything
            else
            {
                isFlying = false;
                isFlyingBack = false;
                var temp = start;
                start = target;
                target = temp;
                transform.LookAt(target);
                timeElapsed = 0;

            }
        }
    }

    // Add command to the queue
    public void EnqueueCommand(FlyCommand com) {
        if (Debugger.inDebugMode()&& commandqueue== null) {
            commandqueue = new Queue<FlyCommand>();
            Debug.LogError("Debug Flyer is not working at the moment");
        }

        commandqueue.Enqueue(com);
        Debug.Log("Comm added: " + com.command + " Pos: " + com.position);
    }

    //Lerp helper
    private void LerpPosition(float lerpfactor) {
        transform.position = start + (target - start) * lerpfactor;
    }
}
