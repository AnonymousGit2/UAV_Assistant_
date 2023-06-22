using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//Class for Sending commands to the ZMQ server
public class CommandRequester : RunAbleThread
{

    private string ip = "tcp://localhost:5555";
    public CommandRequester() { 
    
    }
    public CommandRequester(string ip) {
        this.ip = ip;
    }

    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        //nope not any more... somehow it freezes sometimes after one use...
        using (RequestSocket client = new RequestSocket())
        {
            Debug.Log("Connect to:"+ip);
            if (ip.Length < 3) {
                client.Connect("tcp://localhost:5555");
            }
            else
            {
                client.Connect(ip);
            }
            
            while (Running)
            {
                if (queue.Count != 0)
                {
                    string messageToSend = queue.Dequeue();
                    string message = "";
                    //client.SendFrame(x.GetJSON());
                    client.SendFrame(messageToSend);
                    // ReceiveFrameString() blocks the thread until you receive the string, but TryReceiveFrameString()
                    // do not block the thread, you can try commenting one and see what the other does, try to reason why
                    // unity freezes when you use ReceiveFrameString() and play and stop the scene without running the server
                    //                string message = client.ReceiveFrameString();
                    //                Debug.Log("Received: " + message);
                    bool gotMessage = false;
                    while (Running)
                    {
                        
                        gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                        if (gotMessage)
                        {
                            break;
                        }
                        Thread.Sleep(10);

                    }

                    if (gotMessage)
                    {
                        AsyncToConsole.addMessage("Received " + message);
                    }

                }
                Thread.Sleep(500);
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    // Adapted from:
    //https://github.com/off99555/Unity3D-Python-Communication



}
