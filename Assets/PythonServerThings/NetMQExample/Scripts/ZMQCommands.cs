using UnityEngine;

public class ZMQCommands : MonoBehaviour
{

    //Server behaviour
    private CommandRequester _commandRequester;

    //Set up ip-adress
    [SerializeField]
    private string ip = "tcp://localhost:5555";

    //Use a coordinate transformer for HL to Drone coordinates
    [SerializeField]
    private CoordinateTransformer coordTransformer;

    private void Start()
    {
        if (ip != "")
        {
            _commandRequester = new CommandRequester(ip);
        }
        else
        {
            _commandRequester = new CommandRequester(InputField.ipAdress);
        }
        _commandRequester.Start();
    }
    
    private void OnDestroy()
    {
        _commandRequester.Stop();
    }

    // CommandWrapper
    public void EnqueueCommand(FlyCommand com)
    {
        _commandRequester.EnqueueCommand(com);
    }

    //Commands directly called and converted into commands more lowlevel commands
  
    public void SendFlyCommand()
    {
        EnqueueCommand(FlyCommand.FlyDistance(Vector3.one));
    }


    public void SendLiftOffCommand() 
    {
        EnqueueCommand(FlyCommand.LiftOff());
    }

    public void SendLandCommand()
    {
        EnqueueCommand(FlyCommand.Land());
    }
    public void SendStopCommand()
    {
        EnqueueCommand(FlyCommand.Stop());
    }
    public void SendFlyDistanceCommand(Vector3 distance)
    {
        EnqueueCommand(FlyCommand.FlyDistance(distance));
    }

    public void SendFlyUpwards(float valMeter) {
        EnqueueCommand(FlyCommand.FlyDistance(new Vector3(0,valMeter,0)));
    }

    public void SendFlyDownwards(float valMeter)
    {
        EnqueueCommand(FlyCommand.FlyDistance(new Vector3(0, -valMeter, 0)));
    }

    public void SendFlyForwardCommand(float valMeter)
    {
        EnqueueCommand(FlyCommand.FlyForwards(valMeter));
    }

    public void SendFlyBackwardCommand(float valMeter)
    {
        EnqueueCommand(FlyCommand.FlyBackwards(valMeter));
    }
    public void SendTurnLeftCommand(float degree)
    {
        EnqueueCommand(FlyCommand.FlyTurnLeft(degree));
    }
    public void SendTurnRightCommand(float degree)
    {
        EnqueueCommand(FlyCommand.FlyTurnRight(degree));
    }
    public void SendReconnect()
    {
        EnqueueCommand(FlyCommand.Reconnect());
    }
    public void SendFlyToCommand(Vector3 pos)
    {
        //transform to the new position
        Debug.Log("Send FlyTo:" + coordTransformer.GetDroneCoordinate(pos));
        EnqueueCommand(FlyCommand.FlyTo(coordTransformer.GetDroneCoordinate(pos)));

    }
    public void SendReturnToStartCommand(float heightOffset) {
        EnqueueCommand(FlyCommand.FlyTo(new Vector3(0, heightOffset, 0)));
    }

}