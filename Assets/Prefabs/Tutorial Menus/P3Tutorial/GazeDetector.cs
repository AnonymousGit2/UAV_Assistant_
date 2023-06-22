using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GazeDetector: MonoBehaviour
{

    string last = "";
    private void Update()
    {
        if (CoreServices.InputSystem.GazeProvider.GazeTarget!= null && name != last) {
            last = CoreServices.InputSystem.GazeProvider.GazeTarget.name;
            Debug.Log(last);
        }
        

    }


   

}
