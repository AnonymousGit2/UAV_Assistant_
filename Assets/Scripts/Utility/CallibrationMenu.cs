using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Class for handling the visibility of the callibration menu. 
public class CallibrationMenu : MonoBehaviour
{



    [SerializeField]
    GameObject nullCoord;
    [SerializeField]
    GameObject coordinateDirection;


    public void setCallibrationInvisible() {
        nullCoord.transform.GetChild(0).gameObject.SetActive(false);
        coordinateDirection.transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void setCallibrationVisible()
    {
        nullCoord.transform.GetChild(0).gameObject.SetActive(true);
        coordinateDirection.transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(true);
    }
    


}
