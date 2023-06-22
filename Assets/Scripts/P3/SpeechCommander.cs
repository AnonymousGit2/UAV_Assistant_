using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//Class for handling speech command inputs
public class SpeechCommander : MonoBehaviour
{
    //-----DEBUG
    [SerializeField]
    private Transform object1; //Name RedPin

    [SerializeField]
    private Transform object2; //Name BluePin


    [SerializeField]
    private DebugFlyer debFlyer;
    //-----DEBUG END


    //[SerializeField]
    //private CommandManager commManager;
    
    
    [SerializeField]
    ZMQCommands zmqCommands;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float hoverOffset = 0.4f;



    public UnityEvent bringEvent;
    public UnityEvent succTargetEvent;
    public UnityEvent failTargetEvent;

    [SerializeField]
    [Range(0.1f,4.0f)]
    private float speechTimeout = 0.5f;

    

    // Start is called before the first frame update
    void Start()
    {


        if (bringEvent == null)
            bringEvent = new UnityEvent();
        if (succTargetEvent == null)
            succTargetEvent = new UnityEvent();
        if (failTargetEvent == null)
            failTargetEvent = new UnityEvent();
    


    }


    private float timer;
    private bool isWaiting;
    private string flyTarget = "";
    // Update is called once per frame
    // Was used for compound commands but ommited due to complexity
    void Update()
    {
        if (isWaiting == true)
        {
            if (timer < speechTimeout)
            {

                if (flyTarget != "")
                {
                    //CloudVisionResultManager.ForceAdd("red", object1.position);
                    //CloudVisionResultManager.ForceAdd("blue", object2.position);

                    //normal
                    if (!CloudVisionResultManager.getPosition(flyTarget, out Vector3 target))
                    {
                        Debug.Log("Target not found");
                    }
                    else
                    {
                        FlyToPosition(target);
                        Debug.Log("Flying to: " + flyTarget);
                    }

                    //Debug
                    //CloudVisionResultManager.ForceAdd("red", object1.position);
                    //CloudVisionResultManager.ForceAdd("blue", object2.position);
                    //only fly to red at the moment
                    //Vector3 target = object1.position;


                    isWaiting = false;
                    flyTarget = "";
                    timer = 0;
                    Debug.Log("Speech Succeded");
                    //Invoke Event 
                    succTargetEvent.Invoke();
                }

                timer += Time.deltaTime;
            }
            else
            {
                isWaiting = false;
                Debug.Log("Speech failed, Try again");
                //Invoke Event 
                failTargetEvent.Invoke();
            }

            

        }
    }

            
  
   
    //Start fly command
    public void FlyDelayed() {
        timer = 0;
        isWaiting = true;

        //Invoke Event 
        bringEvent.Invoke();


    }

    //So if fly command is ongoing, set the target.
    public void SetTarget(string s)
    {
        //Only succeed if there exists a valid target
        if (isWaiting && CloudVisionResultManager.isValidated(s))
        {
            flyTarget = s;
        }
    }

    //Force the drone to fly to the target without further checks
    public void ForceFly(string s) {
        if (!CloudVisionResultManager.getPosition(s, out Vector3 target))
        {
            Debug.Log("Target not found");
        }
        else
        {
            FlyToPosition(target);
            Debug.Log("Flying to: " + flyTarget);
        }
        Debug.Log("Speech Succeded");
        //Invoke Event 
        succTargetEvent.Invoke();

    }

    //StartFlying
    public void FlyToPosition(Vector3 target)
    {
        if (Debugger.inDebugMode())
        {
            debFlyer.SetNewTarget(target);
            debFlyer.Fly();
        }
        else {
            FlyDroneTo(target, true);
        }
    }




    private void FlyDroneTo(Vector3 pos, bool andBack = false)
    {
        zmqCommands.SendLiftOffCommand();
        zmqCommands.SendFlyUpwards(hoverOffset);
        zmqCommands.SendFlyToCommand(pos + new Vector3(0, hoverOffset, 0));
        zmqCommands.SendFlyDownwards(hoverOffset - 0.1f);

        if (andBack)
        {
            zmqCommands.SendFlyUpwards(hoverOffset);
            zmqCommands.SendReturnToStartCommand(1.0f);
            zmqCommands.SendReturnToStartCommand(0.2f);
            //zmqCommands.SendStopCommand();
            zmqCommands.SendLandCommand();
        }

    }



}
