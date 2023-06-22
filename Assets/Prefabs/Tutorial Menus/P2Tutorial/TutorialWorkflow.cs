using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialWorkflow : MonoBehaviour
{
    public UnityEvent onFirstGrabTask;
    public UnityEvent onPinPush;

    private string currentTask = "pinPush";
    // Start is called before the first frame update
    void Start()
    {
        if (onFirstGrabTask == null) {
            onFirstGrabTask = new UnityEvent();
        }
        if (onPinPush == null)
        {
            onPinPush = new UnityEvent();
        }
    }


    public void SetTask(string task) {
        currentTask = task;
    }

    public void grabObjectCheck() {
        if (currentTask == "grab") {
            onFirstGrabTask.Invoke();
            currentTask = "";
        }  
    }

    public void PinButtonCheck() {
        if (currentTask == "pinPush")
        {
            onPinPush.Invoke();
            currentTask = "";
        }
    }
}
