using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//modify some simply fly commands so the drone flies correctly

public class CommandManager : MonoBehaviour
{

    [SerializeField]
    ZMQCommands zmqCommands;

    [SerializeField]
    [Range(0.0f,1.0f)]
    float overOffset = 0.4f;

    public void FlyTo(Vector3 pos,bool andBack = false) {
        zmqCommands.SendLiftOffCommand();
        zmqCommands.SendFlyUpwards(overOffset);
        zmqCommands.SendFlyToCommand(pos+new Vector3(0, overOffset, 0));
        zmqCommands.SendFlyDownwards(overOffset-0.1f);

        if (andBack)
        {
            zmqCommands.SendFlyUpwards(overOffset);
            zmqCommands.SendReturnToStartCommand(1.0f);
            zmqCommands.SendReturnToStartCommand(0.2f);
            //zmqCommands.SendStopCommand();
            zmqCommands.SendLandCommand();
        }

    }


}
