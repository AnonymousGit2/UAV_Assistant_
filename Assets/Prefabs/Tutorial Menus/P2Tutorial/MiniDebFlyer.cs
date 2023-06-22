using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniDebFlyer : MonoBehaviour
{
    private bool isFlying;
    private bool isFlyingBack;
    private Vector3 start;
    private Vector3 target;
    private Vector3 flightStart;
    float timeElapsed;
    float lerpDuration = 3;

    Queue<FlyCommand> commandqueue;


    [SerializeField]
    private Transform pos1;
    [SerializeField]
    private Transform pos2;
    [SerializeField]
    private Transform posStart;
    [SerializeField]
    private Transform posFin;

    private void Start()
    {
        commandqueue = new Queue<FlyCommand>();
        SetStart(transform.position);
    }


    public void SetStart(Vector3 pos)
    {
        start = pos;
        transform.position = start;
    }

    public void SetNewTarget(Vector3 pos)
    {
        target = pos;
        transform.LookAt(target);
    }

    public void FlyShortPath() {
        EnqueueCommand(FlyCommand.FlyTo(posStart.position));
        EnqueueCommand(FlyCommand.FlyTo(pos1.position));
        EnqueueCommand(FlyCommand.FlyTo(pos2.position));
        EnqueueCommand(FlyCommand.FlyTo(posFin.position));
        EnqueueCommand(FlyCommand.FlyTo(pos2.position));
        EnqueueCommand(FlyCommand.FlyTo(pos1.position));
        EnqueueCommand(FlyCommand.FlyTo(posStart.position));
        //EnqueueCommand(FlyCommand.FlyTo(flightStart));
      
        FlyQueue();
    }

    public void Fly()
    {
        FlyElegant();
    }

    private void FlyElegant(bool andBack = true)
    {
        isFlying = true;
        flightStart = transform.position;

        //start the drone
        EnqueueCommand(FlyCommand.FlyTo(start + new Vector3(0, target.y + 0.3f, 0)));

        EnqueueCommand(FlyCommand.FlyTo(target + new Vector3(0, target.y + 0.3f, 0)));

        //since one position here the start position is not in the path it has to be add manually
        EnqueueCommand(FlyCommand.FlyTo(target));

        EnqueueCommand(FlyCommand.FlyTo(target + new Vector3(0, target.y + 0.3f, 0)));

        EnqueueCommand(FlyCommand.FlyTo(start + new Vector3(0, target.y + 0.3f, 0)));
        EnqueueCommand(FlyCommand.FlyTo(start));

        //then land and turn off
        isFlyingBack = true;
        FlyCommand currentCom = commandqueue.Dequeue();
        target = DecryptCommand(currentCom);
        transform.LookAt(target);


    }

    public void FlyQueue()
    {
        flightStart = transform.position;
        isFlying = true;
        start = transform.position;
        FlyCommand currentCom = commandqueue.Dequeue();
        target = DecryptCommand(currentCom);
        transform.LookAt(target);
    }


    private Vector3 DecryptCommand(FlyCommand com)
    {
        switch (com.command)
        {
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
            else if (commandqueue.Count > 0)
            {
                LerpPosition(1);
                start = target;
                target = DecryptCommand(commandqueue.Dequeue());
                transform.LookAt(target);
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


    public void EnqueueCommand(FlyCommand com)
    {
        commandqueue.Enqueue(com);
        Debug.Log("Comm added: " + com.command + " Pos: " + com.position);
    }

    private void LerpPosition(float lerpfactor)
    {
        transform.position = start + (target - start) * lerpfactor;
    }


}
