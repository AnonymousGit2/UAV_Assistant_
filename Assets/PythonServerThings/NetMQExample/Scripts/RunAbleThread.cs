﻿using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
///     The superclass that you should derive from. It provides Start() and Stop() method and Running property.
///     It will start the thread to run Run() when you call Start().
/// </summary>
public abstract class RunAbleThread
{
    private readonly Thread _runnerThread;
    protected Queue<string> queue;

    protected RunAbleThread()
    {
        // we need to create a thread instead of calling Run() directly because it would block unity
        // from doing other tasks like drawing game scenes
        _runnerThread = new Thread(Run);
    }

    protected bool Running { get; private set; }

    /// <summary>
    /// This method will get called when you call Start(). Programmer must implement this method while making sure that
    /// this method terminates in a finite time. You can use Running property (which will be set to false when Stop() is
    /// called) to determine when you should stop the method.
    /// </summary>
    protected abstract void Run();

    public void Start()
    {
        queue = new Queue<string>();
        Running = true;
        _runnerThread.Start();
        
    }

    public void Stop()
    {
        Running = false;
        // block main thread, wait for _runnerThread to finish its job first, so we can be sure that 
        // _runnerThread will end before main thread end
        _runnerThread.Join();
    }

    public void EnqueueCommand(FlyCommand com) {
        queue.Enqueue(com.GetJSON());
    }


    //Adapted from
    //https://github.com/off99555/Unity3D-Python-Communication




}