using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

//The possible Fly commands serializable
public enum CommandType {FlyToPos,Log,Stop,Start,Land,FlyForward, FlyBackward, FlyTurnLeft, FlyTurnRight,FlyDirection,Reconnect};


//Convert ´Project specific fly commands to serializable ones
public class FlyCommand
{
    public CommandType command;
    public Vector3 position;
    public float value;

    public FlyCommand(CommandType type, Vector3 pos, float val) {
        command = type;
        position = pos;
        value = val;
    }
    public CommandType GetCommand() {
        return command;
    }


    public static FlyCommand FlyTo(Vector3 pos) {
        return new FlyCommand(CommandType.FlyToPos, pos, 0);
    }
    public static FlyCommand LiftOff()
    {
        return new FlyCommand(CommandType.Start, Vector3.zero, 0);
    }
    public static FlyCommand Land()
    {
        return new FlyCommand(CommandType.Land, Vector3.zero, 0);
    }
    public static FlyCommand Stop()
    {
        return new FlyCommand(CommandType.Stop, Vector3.zero, 0);
    }
    public static FlyCommand FlyDistance(Vector3 dist) {
        return new FlyCommand(CommandType.FlyDirection, dist, 0);
    
    }

    public static FlyCommand FlyForwards(float val) 
    {
        return new FlyCommand(CommandType.FlyForward, Vector3.zero, val);
    }
    public static FlyCommand FlyBackwards(float val)
    {
        return new FlyCommand(CommandType.FlyBackward, Vector3.zero, val);
    }
    public static FlyCommand FlyTurnLeft(float degree)
    {
        return new FlyCommand(CommandType.FlyTurnLeft, Vector3.zero, degree);
    }
    public static FlyCommand FlyTurnRight(float degree)
    {
        return new FlyCommand(CommandType.FlyTurnRight, Vector3.zero, degree);
    }
    public static FlyCommand Reconnect()
    {
        return new FlyCommand(CommandType.Reconnect, Vector3.zero, 0);
    }


    //public static FlyCommand FlyDirection(CommandType type, float val) {
    //    switch (type) {
    //        case CommandType.FlyForward:
    //            return new FlyCommand(CommandType.FlyForward, Vector3.zero, val);
    //        case CommandType.FlyBackward:
    //            return new FlyCommand(CommandType.FlyBackward, Vector3.zero, val);
    //        case CommandType.FlyTurnLeft:
    //            return new FlyCommand(CommandType.FlyTurnLeft, Vector3.zero, val);
    //        case CommandType.FlyTurnRight:
    //            return new FlyCommand(CommandType.FlyTurnRight, Vector3.zero, val);
    //        default:
    //            throw new Exception("Unsupported Fly Command");
    //    }
    //}


    public string GetJSON()
    {
        return JsonUtility.ToJson(this);
    }

    public static FlyCommand FromJson(string json)
    {
        return JsonUtility.FromJson<FlyCommand>(json);
    }


    public override string ToString()
    {
        return "CommandType " + command;//+ "\nPosition: " + position + "\nDirection " + direction +"\nDuration"+ duration.ToString();
    }

}




    public class ExampleJSON : MonoBehaviour
{

    
}
