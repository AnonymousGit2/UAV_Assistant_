using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//NOT USED- Was for Testing a more perfomant scanning
public abstract class ImageRequestThread
{
    private readonly Thread _runnerThread;

    protected ImageRequestThread()
    {
        // we need to create a thread instead of calling Run() directly because it would block unity
        // from doing other tasks like drawing game scenes
        _runnerThread = new Thread(Run);
    }
    protected bool Running { get; private set; }

    public bool isRunning() { return Running; }

    protected abstract void Run();

    public void Start()
    { 
        Running = true;
        _runnerThread.Start();

    }

    public void Stop()
    {
        AsyncToConsole.addMessage("Stopping Thread");
        Running = false;
        // block main thread, wait for _runnerThread to finish its job first, so we can be sure that 
        // _runnerThread will end before main thread end
        _runnerThread.Join();
        AsyncToConsole.addMessage("Thread Stopped");
    }


}