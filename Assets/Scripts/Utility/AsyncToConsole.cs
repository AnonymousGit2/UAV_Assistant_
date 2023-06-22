using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class to convert async constol writings to the holographic console used in augmented reality.
public class AsyncToConsole : MonoBehaviour
{

    private static List<string> messages;
    // Start is called before the first frame update
    void Start()
    {
        messages = new List<string>();
    }

    public static void addMessage(string s)
    {
        messages.Add(s);
    }

    public static void addException(Exception e) {
        messages.Add(e.ToString());
        throw e;
    }

    void Update()
    {
        if (messages.Count != 0)
        {
            var pos = messages.Count - 1;
            Debug.Log(messages[pos]);
            messages.RemoveAt(pos);
        }
    }


}
