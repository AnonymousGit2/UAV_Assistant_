using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{

    public static bool debugMode = false;

    public static bool inDebugMode() {
        return debugMode;
    }


    public void enableDebug() {
        debugMode = true;
    }

    public void disableDebug() {
        debugMode = false;
    }
    public void ToggelDebug() {
        debugMode = !debugMode;
        Debug.Log("Debugmode:" + debugMode);
    }



}
